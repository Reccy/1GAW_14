using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.ScriptExtensions;

public class FlamethrowerProjectile : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private List<Sprite> m_fireSprites;

    [SerializeField] private Color m_colorRange1;
    [SerializeField] private Color m_colorRange2;

    private Rigidbody2D m_rb;
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.velocity = transform.up * m_speed;

        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        m_spriteRenderer.sprite = m_fireSprites.SelectRandom();
        m_spriteRenderer.color = Color.Lerp(m_colorRange1, m_colorRange2, Random.Range(0, 1));
    }
}
