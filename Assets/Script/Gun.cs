using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunData gunData;
    public Transform fireTransform;
    public ParticleSystem gunParticles;

    public LayerMask targetLayer;

    private AudioSource _gunAudioSource;
    private LineRenderer _gunLineEffect;
    private Coroutine _coShot;

    private float _lastFireTime;
    private float _fireDistance = 50f;

    private void Awake()
    {
        _gunAudioSource = GetComponent<AudioSource>();

        _gunLineEffect = GetComponent<LineRenderer>();
        _gunLineEffect.positionCount = 2;
        _gunLineEffect.enabled = false;
    }

    private void OnEnable()
    {
        _lastFireTime = 0f;
    }

    public void Fire()
    {
        if (Time.time > _lastFireTime + gunData.timeBetFrie)
        {
            _lastFireTime = Time.time;
            Shot();
        }
    }

    private void Shot()
    {
        Vector3 hitPosition = Vector3.zero;

        Ray ray = new Ray(fireTransform.position, fireTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _fireDistance, targetLayer))
        {
            hitPosition = hit.point;

            var target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * _fireDistance;
        }

        if (_coShot != null)
        {
            StopCoroutine(_coShot);
            _coShot = null;
        }

        _coShot = StartCoroutine(CoShotEffect(hitPosition));
    }

    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        gunParticles.Play();

        _gunAudioSource.PlayOneShot(gunData.shotClip);

        _gunLineEffect.SetPosition(0, fireTransform.position);
        _gunLineEffect.SetPosition(1, hitPosition);
        _gunLineEffect.enabled = true;

        yield return new WaitForSeconds(0.03f);

        _gunLineEffect.enabled = false;

        _coShot = null;
    }
}