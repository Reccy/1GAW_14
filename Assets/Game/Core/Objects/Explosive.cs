using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private Flammable m_flammable;
    [SerializeField] private Collider2D m_explosionTrigger;
    [SerializeField] private ParticleSystem m_explosionFX;
    [SerializeField] private float m_explosionTimer = 3.0f;

    private void Awake()
    {
        if (m_flammable == null)
            m_flammable = GetComponentInChildren<Flammable>();

        if (m_explosionFX == null)
            m_explosionFX = GetComponentInChildren<ParticleSystem>();

        m_flammable.OnEnflamed += HandleOnEnflamed;
    }

    private void HandleOnEnflamed()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(m_explosionTimer);

        m_explosionTrigger.enabled = true;
        m_explosionFX.Play();

        yield return new WaitForFixedUpdate();

        m_explosionFX.transform.parent = null;
        gameObject.SetActive(false);

        while (m_explosionFX.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        Destroy(m_explosionFX);
    }
}
