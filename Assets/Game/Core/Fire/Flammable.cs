using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    [SerializeField] private bool m_enflamed;

    [SerializeField] private ParticleSystem m_fireFX;
    [SerializeField] private bool m_immediateEnflame = false;
    [SerializeField] private bool m_immediateExtinguish = false;
    [SerializeField] private bool m_listenToCollisions = true;
    [SerializeField] private bool m_listenToTriggers = true;

    public ParticleSystem FX => m_fireFX;

    private readonly float ENFLAME_DELAY = 0.2f;
    private readonly float EXTINGUISH_DELAY = 0.02f;

    private Coroutine m_enflameDelayCoroutine = null;
    private Coroutine m_extinguishCoroutine = null;

    public delegate void OnEnflamedEvent();
    public OnEnflamedEvent OnEnflamed;

    public delegate void OnExtinguishedEvent();
    public OnExtinguishedEvent OnExtinguished;

    public bool OnFire => m_enflamed;

    [SerializeField] private bool m_debugMode = false;

    private void Awake()
    {
        if (m_enflamed)
        {
            m_enflamed = false;
            Enflame();
        }
    }

    private void OnEnable()
    {
        var fnp = FlammableNodePenalty.Instance;

        if (fnp == null)
            Debug.LogError("FlammableNodePenalty is null?!?!?!?!?");

        fnp.Track(this);
    }

    private void OnDisable()
    {
        var fnp = FlammableNodePenalty.Instance;

        if (fnp == null) // If it's null during OnDisable, game is shutting down
            return;

        fnp.Untrack(this);
    }

    private void OnCollisionEnter2D(Collision2D collision) => HandleCollision2D(collision);
    private void OnCollisionStay2D(Collision2D collision) => HandleCollision2D(collision);
    private void OnTriggerEnter2D(Collider2D collision) => HandleTrigger2D(collision);
    private void OnTriggerStay2D(Collider2D collision) => HandleTrigger2D(collision);

    private void HandleCollision2D(Collision2D collision)
    {
        if (!m_listenToCollisions)
            return;

        var otherFlammable = collision.gameObject.GetComponent<Flammable>();

        if (otherFlammable == null)
            return;

        if (otherFlammable.m_enflamed)
        {
            Enflame();
        }
    }

    private void HandleTrigger2D(Collider2D collision)
    {
        if (!m_listenToTriggers)
            return;

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
        if (m_enflamed || m_enflameDelayCoroutine != null)
            return;

        m_enflameDelayCoroutine = StartCoroutine(EnflameCoroutine());
    }

    private IEnumerator EnflameCoroutine()
    {
        if (!m_immediateEnflame)
            yield return new WaitForSeconds(ENFLAME_DELAY);

        if (m_debugMode)
            Debug.Log("ENFLAME!");

        m_enflamed = true;

        if (m_fireFX)
            m_fireFX.Play();

        if (OnEnflamed != null)
            OnEnflamed();
    }

    public void Extinguish()
    {
        if (!m_enflamed || m_extinguishCoroutine != null)
            return;

        m_extinguishCoroutine = StartCoroutine(ExtinguishCoroutine());
    }

    private IEnumerator ExtinguishCoroutine()
    {
        if (!m_immediateExtinguish)
            yield return new WaitForSeconds(EXTINGUISH_DELAY);

        if (m_debugMode)
            Debug.Log("EXTINGUISHED!");

        m_enflamed = false;

        if (m_fireFX)
            m_fireFX.Stop();

        if (OnExtinguished != null)
            OnExtinguished();
    }
}
