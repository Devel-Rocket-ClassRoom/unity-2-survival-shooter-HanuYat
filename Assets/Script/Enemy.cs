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
        if (target != null)
        {
            var livingEntity = target.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                if (livingEntity.IsDead)
                {
                    return;
                }
            }
        }

        if (Vector3.Distance(transform.position, target.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }
    }

    private void UpdateTrace()
    {
        if (target != null)
        {
            if (target.CompareTag("Player"))
            {
                if (_enemyCollider.isTrigger)
                {
                    CurrentStatus = Status.Attack;
                    return;
                }
            }
        }

        if(target == null || Vector3.Distance(transform.position, target.position) > traceDistance)
        {
            CurrentStatus = Status.Idle;
            return;
        }

        _agent.SetDestination(target.position);
    }

    private void UpdateAttack()
    {
        if (target == null)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        var lookAt = target.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);

        if (Time.time > _lastAttackTime + attackInterval)
        {
            _lastAttackTime = Time.time;

            var livingEntity = target.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                if (!livingEntity.IsDead)
                {
                    livingEntity.OnDamage(_damage, transform.position, -transform.forward);
                    return;
                }
                else
                {
                    CurrentStatus = Status.Idle;
                    return;
                }
            }
        }
    }

    private void UpdateDie()
    {
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!IsDead)
        {
            base.OnDamage(damage, hitPoint, hitNormal);

            hitParticles.transform.position = hitPoint;
            hitParticles.transform.forward = hitNormal;
            hitParticles.Play();

            _enemyAudioSource.PlayOneShot(hitClip);
        }
    }

    public override void Die()
    {
        if (IsDead)
        {
            return;
        }

        base.Die();
        _enemyAudioSource.PlayOneShot(deathClip);
        CurrentStatus = Status.Die;
    }
}