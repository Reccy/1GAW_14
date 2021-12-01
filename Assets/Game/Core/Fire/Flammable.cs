using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    [SerializeField] private bool m_enflamed;

    [SerializeField] private ParticleSystem m_fireFX;

    public delegate void OnEnflamedEvent();
    public OnEnflamedEvent OnEnflamed;

    public delegate void OnExtinguishedEvent();
    public OnExtinguishedEvent OnExtinguished;

    public bool OnFire => m_enflamed;

    private void Awake()
    {
        if (m_enflamed)
        {
            m_enflamed = false;
            Enflame();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var otherFlammable = collision.gameObject.GetComponent<Flammable>();

        if (otherFlammable == null)
            return;

        if (otherFlammable.m_enflamed)
        {
            Enflame();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var otherFlammable = collision.gameObject.GetComponent<Flammable>();

        if (otherFlammable == null)
            return;

        if (otherFlammable.m_enflamed)
        {
            Enflame();
        }
    }

    public void Enflame()
    {
        if (m_enflamed)
            return;

        m_enflamed = true;
        m_fireFX.Play();

        if (OnEnflamed != null)
            OnEnflamed();
    }

    public void Extinguish()
    {
        if (!m_enflamed)
            return;

        m_enflamed = false;
        m_fireFX.Stop();

        if (OnExtinguished != null)
            OnExtinguished();
    }
}
