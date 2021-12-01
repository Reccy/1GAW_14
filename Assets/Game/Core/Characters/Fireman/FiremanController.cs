using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class FiremanController : MonoBehaviour
{
    #region INPUT
    private Player m_player;
    private Mouse m_mouse;
    private const int PLAYER_ID = 0;

    private Vector2 m_inputMovement;
    private bool m_inputFire;

    private void ReadInput()
    {
        m_inputMovement = m_player.GetAxis2D("MoveHorizontal", "MoveVertical");

        m_inputFire = m_player.GetButton("Fire");
    }

    private void ClearInput()
    {
        m_inputMovement = default(Vector2);
        m_inputFire = default(bool);
    }
    #endregion INPUT

    [SerializeField] private float m_speedMult = 3.0f;
    private Rigidbody2D m_rb;
    private Transform m_aimTarget;

    [SerializeField] private ParticleSystem m_projectileParticleSys;
    [SerializeField] private Transform m_projectileOrigin;
    [SerializeField] private Flammable m_flamethrower;

    private Flammable m_selfFire;

    private MMFeedbacks m_feedbacks;

    private void Awake()
    {
        m_player = ReInput.players.GetPlayer(PLAYER_ID);
        m_mouse = m_player.controllers.Mouse;

        m_selfFire = GetComponent<Flammable>();

        m_feedbacks = GetComponent<MMFeedbacks>();
        m_rb = GetComponent<Rigidbody2D>();
        m_aimTarget = FindObjectOfType<MouseCursor>().transform;
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        Move();
        Aim();
        Fire();
        ClearInput();
    }

    private void Move()
    {
        m_rb.velocity = m_inputMovement * m_speedMult;
    }

    private void Aim()
    {
        var direction = (m_aimTarget.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    private void Fire()
    {
        if (!m_inputFire)
        {
            m_flamethrower.Extinguish();
            return;
        }

        m_flamethrower.Enflame();

        m_feedbacks.PlayFeedbacks();
    }
}
