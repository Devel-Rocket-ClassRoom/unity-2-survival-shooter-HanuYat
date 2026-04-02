using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die
    }

    public GunData gunData;

    public Transform target;
    public ParticleSystem hitParticles;

    public AudioClip deathClip;
    public AudioClip hitClip;

    public float traceDistance = 100f;
    public float attackInterval = 1f;
    
    private float _lastAttackTime;
    private float _damage;

    public LayerMask targetLayer;

    private NavMeshAgent _agent;
    private Animator _enemyAnimator;
    private AudioSource _enemyAudioSource;
    private Collider _enemyCollider;

    private Status _currentStatus;

    public Status CurrentStatus
    {
        get => _currentStatus;

        set
        {
            var prevStatus = _currentStatus;
            _currentStatus = value;

            switch (_currentStatus)
            {
                case Status.Idle:
                    _agent.isStopped = true;
                    break;

                case Status.Trace:
                    _agent.isStopped = false;
                    break;

                case Status.Attack:
                    _agent.isStopped = true;
                    break;

                case Status.Die:
                    _agent.isStopped = true;
                    _enemyAnimator.SetTrigger("Die");
                    _enemyCollider.enabled = false;
                    break;
            }
        }
    }

    public void Setup(EnemyData enemyData)
    {
        gameObject.SetActive(false);

        startingHp = enemyData.maxHp;
        _damage = enemyData.damage;
        _agent.speed = enemyData.speed;

        gameObject.SetActive(true);
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAnimator = GetComponent<Animator>();
        _enemyAudioSource = GetComponent<AudioSource>();
        _enemyCollider = GetComponent<Collider>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _agent.enabled = true;
        _agent.isStopped = false;
        _agent.ResetPath();
        if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
        {
            _agent.Warp(hit.position);
        }

        _enemyCollider.enabled = true;
        CurrentStatus = Status.Idle;
    }

    private void Update()
    {
        switch (_currentStatus)
        {
            case Status.Idle:
                UpdateIdle();
                break;

            case Status.Trace:
                UpdateTrace();
                break;

            case Status.Attack:
                UpdateAttack();
                break;

            case Status.Die:
                UpdateDie();
                break;
        }
    }

    private void UpdateIdle()
    {
    }

    private void UpdateTrace()
    {
    }

    private void UpdateAttack()
    {
    }

    private void UpdateDie()
    {
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        if (IsDead)
        {
            _enemyAudioSource.PlayOneShot(deathClip);
            CurrentStatus = Status.Die;
        }
        else
        {
            _enemyAudioSource.PlayOneShot(hitClip);
            hitParticles.transform.position = hitPoint;
            hitParticles.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitParticles.Play();
        }
    }
}