using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveForce : MonoBehaviour
{
    [SerializeField] private float m_force = 5.0f;

    private Collider2D m_collider;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D[] hits = new RaycastHit2D[20];
        int resultCount = m_collider.Cast(Vector2.zero, hits);

        for (int i = 0; i < resultCount; ++i)
        {
            var hit = hits[i];
            var dir = (hit.transform.position - transform.position).normalized;

            hit.rigidbody.AddForce(dir * m_force);
        }
    }
}
