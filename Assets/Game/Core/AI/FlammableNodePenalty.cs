using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Reccy.ScriptExtensions;
using Reccy.DebugExtensions;

public class FlammableNodePenalty : MonoBehaviour
{
    [Header("Flammable Settings")]
    [SerializeField] private uint m_flammablePenalty = 3000;
    [SerializeField] private uint m_standardPenalty = 1000;
    [SerializeField] private int m_flammableDangerRadius = 5;
    [SerializeField] private int m_updatesPerSecond = 10;
    [SerializeField] private AnimationCurve m_penaltyFalloff;

    private HashSet<Flammable> m_flammables;

    public static FlammableNodePenalty Instance;

    private void Awake()
    {
        #region SINGLETON
        if (Instance != null)
            Debug.LogWarning("Multiple FlammableNodePenalty objects!!!!!");

        Instance = this; // hacky """""singleton"""""
        #endregion

        m_flammables = new HashSet<Flammable>();

        float workRate = 1.0f / m_updatesPerSecond;
        InvokeRepeating("DoWork", 0, workRate);
    }

    private void DoWork()
    {
        AstarPath.active.FlushWorkItems();

        AstarPath.active.AddWorkItem(ctx => {
            var gg = AstarPath.active.data.gridGraph;

            // Clear node penalty
            gg.GetNodes(node =>
            {
                node.Penalty = m_standardPenalty;
            });

            // Update node penalty
            foreach (var flammable in m_flammables)
            {
                if (!flammable.OnFire)
                    continue;

                float radius = m_flammableDangerRadius;

                for (int x = 0; x <= m_flammableDangerRadius; ++x)
                {
                    for (int y = 0; y <= m_flammableDangerRadius; ++y)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) > radius)
                            continue;

                        UpdatePenalty(x, y, flammable);

                        if (x != 0)
                            UpdatePenalty(-x, y, flammable);

                        if (y != 0)
                            UpdatePenalty(x, -y, flammable);

                        if (x != 0 && y != 0)
                            UpdatePenalty(-x, -y, flammable);
                    }
                }
            }
        });

        /*
        AstarPath.active.data.gridGraph.GetNodes(node => {
            AstarPath.active.AddWorkItem(ctx => {
                var flamdist = GetClosestFlammable((Vector3)node.position);

                if (flamdist.flammable == null)
                {
                    node.Penalty = m_standardPenalty;
                    return;
                }

                var flammable = flamdist.flammable;
                var closest = flamdist.closest;

                if (closest > m_flammableDangerRadius)
                {
                    node.Penalty = m_standardPenalty;
                    return;
                }

                var t = Mathf.InverseLerp(0, m_flammableDangerRadius, closest);
                var tRemapped = m_penaltyFalloff.Evaluate(t);

                var result = Mathf.Lerp(m_flammablePenalty, m_standardPenalty, tRemapped);

                node.Penalty = (uint)(result);
            });
        });
        */
    }

    private void UpdatePenalty(int x, int y, Flammable flammable)
    {
        var gg = AstarPath.active.data.gridGraph;
        var center = AstarPath.active.GetNearest(flammable.transform.position).position;
        var node = AstarPath.active.GetNearest(new Vector3(center.x + x, center.y + y, 0)).node;

        if (node == null)
            return;

        var distance = Vector2.Distance(flammable.transform.position, (Vector3)node.position);
        var t = Mathf.InverseLerp(0, m_flammableDangerRadius, distance);
        var tRemapped = m_penaltyFalloff.Evaluate(t);
        var result = Mathf.Lerp(m_flammablePenalty, m_standardPenalty, tRemapped);

        node.Penalty = (uint)Mathf.Max((uint)result, node.Penalty);
    }

    private (Flammable flammable, float closest) GetClosestFlammable(Vector3 position)
    {
        float closest = float.MaxValue;
        Flammable flammable = null;

        foreach (var flame in m_flammables)
        {
            if (!flame.OnFire)
                continue;

            var dist = Vector3.Distance(position, flame.transform.position);

            if (dist < closest)
            {
                closest = dist;
                flammable = flame;
            }
        }

        return (flammable, closest);
    }

    public void Track(Flammable flame)
    {
        m_flammables.Add(flame);
    }

    public void Untrack(Flammable flame)
    {
        m_flammables.Remove(flame);
    }
}
