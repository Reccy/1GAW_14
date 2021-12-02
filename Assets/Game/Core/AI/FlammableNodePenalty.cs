using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Reccy.ScriptExtensions;
using Reccy.DebugExtensions;

public class FlammableNodePenalty : MonoBehaviour
{
    [SerializeField] private uint m_flammablePenalty = 3000;
    [SerializeField] private uint m_standardPenalty = 1000;
    [SerializeField] private float m_flammableDangerRadius = 5.0f;
    [SerializeField] private int m_updatesPerSecond = 10;
    [SerializeField] private AnimationCurve m_penaltyFalloff;

    private HashSet<Flammable> m_set;

    public static FlammableNodePenalty Instance;

    private void Awake()
    {
        #region SINGLETON
        if (Instance != null)
            Debug.LogWarning("Multiple FlammableNodePenalty objects!!!!!");

        Instance = this; // hacky """""singleton"""""
        #endregion

        m_set = new HashSet<Flammable>();

        float workRate = 1.0f / m_updatesPerSecond;
        InvokeRepeating("DoWork", 0, workRate);
    }

    private void DoWork()
    {
        var workItem = new AstarWorkItem(ctx =>
        {
            var gg = AstarPath.active.data.gridGraph;

            gg.GetNodes(node =>
            {
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

        AstarPath.active.AddWorkItem(workItem);
    }

    private (Flammable flammable, float closest) GetClosestFlammable(Vector3 position)
    {
        float closest = float.MaxValue;
        Flammable flammable = null;

        foreach (var flame in m_set)
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
        m_set.Add(flame);
    }

    public void Untrack(Flammable flame)
    {
        m_set.Remove(flame);
    }
}
