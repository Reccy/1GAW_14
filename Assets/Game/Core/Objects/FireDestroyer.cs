using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDestroyer : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_flameFX;
    [SerializeField] private Flammable m_flammable;
    [SerializeField] private float m_timeUntilDestroyed = 1.0f;

    private void Awake()
    {
        if (m_flammable == null)
            m_flammable = GetComponentInChildren<Flammable>();

        m_flameFX = m_flammable.FX;
        m_flammable.OnEnflamed += HandleOnEnflamed;
    }

    private void HandleOnEnflamed()
    {
        StartCoroutine(OnFlameCoroutine());
    }

    private IEnumerator OnFlameCoroutine()
    {
        yield return new WaitForSeconds(m_timeUntilDestroyed);

        m_flameFX.transform.parent = null;
        m_flameFX.Stop();

        gameObject.SetActive(false);

        while (m_flameFX.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
        Destroy(m_flameFX.gameObject);
    }
}
