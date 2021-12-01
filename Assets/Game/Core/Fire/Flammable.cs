using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    [SerializeField] private bool m_enflamed;

    [SerializeField] private ParticleSystem m_fireFX;

    private void Awake()
    {
        if (m_enflamed)
        {
            m_enflamed = false;
            Enflame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            Enflame();
        }
    }

    private void Enflame()
    {
        if (m_enflamed)
            return;

        m_enflamed = true;
        m_fireFX.Play();
    }

    private void Extinguish()
    {
        if (!m_enflamed)
            return;

        m_enflamed = false;
        m_fireFX.Stop();
    }
}
