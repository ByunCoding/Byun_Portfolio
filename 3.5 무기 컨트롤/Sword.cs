using Unity.VisualScripting;
using UnityEngine;

public class Sword : WeaponController
{
    private bool _damaging; // 데미지 주는 중

    private Collider _swordCollider;

    private GameObject _swordEffect;

    [SerializeField] private GameObject _gethitEffect;

    protected override void InitialRandomStats()
    {
        base.InitialRandomStats();
    }

    private void Swing()
    {
        if (CanAttack())
        {
            _damaging = false;

            Attack();
        }
    }

    private void Awake()
    {
        if(_swordEffect == null)
        {
            Transform effectTransform = transform.Find("SwordEffect");
            if(effectTransform != null)
            {
                _swordEffect = effectTransform.gameObject;
            }
        }

        if(_swordEffect != null)
        {
            _swordEffect.SetActive(false);
        }

        if(_swordCollider == null)
        {
            _swordCollider = GetComponent<Collider>();
        }
    }

    public void EnableSwordCollider()
    {
        if(_swordCollider != null)
        {
            _swordCollider.enabled = true;
        }
    }

    public void DisableSwordCollider()
    {
        if(_swordCollider != null)
        {
            _swordCollider.enabled = false;
        }
    }

    public void EnableEffect()
    {
        if (_swordEffect != null)
        {
            _swordEffect.SetActive(true);
        }
    }

    public void DisableEffect()
    {
        if (_swordEffect != null)
        {
            _swordEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (weaponState == WeaponState.Attacking)
        {
            Vector3 gethitPoint = other.ClosestPoint(this.transform.position);

            if (other.CompareTag("Golem"))
            {
                Golems golems = other.GetComponent<Golems>();
                if (golems != null && golems._currentHealth > 0)
                {
                    if(_swordCollider != null && _swordCollider.enabled)
                    {
                        golems.TakeDamaged(CriticalChance(), this);
                        _damaging = true;
                        PlayGetHitEffectPoint(gethitPoint);

                        AudioManager.Instance.PlaySwordHitSound();
                    }
                }
            }
            else if (other.CompareTag("Zombie"))
            {
                FemaleZombie femaleZombie = other.GetComponent<FemaleZombie>();
                if (femaleZombie != null && !femaleZombie._isDead)
                {
                    femaleZombie.TakeDamaged(CriticalChance(), this);
                    _damaging = true;
                    PlayGetHitEffectPoint(gethitPoint);

                    AudioManager.Instance.PlaySwordHitSound();
                }

                ModernZombie modernZombie = other.GetComponent<ModernZombie>();
                if (modernZombie != null && !modernZombie._isDead)
                {
                    modernZombie.TakeDamaged(CriticalChance(), this);
                    _damaging = true;
                    PlayGetHitEffectPoint(gethitPoint);

                    AudioManager.Instance.PlaySwordHitSound();
                }

                GhoulZombie ghoulZombie = other.GetComponent<GhoulZombie>();
                if (ghoulZombie != null && !ghoulZombie._isDead)
                {
                    ghoulZombie.TakeDamaged(CriticalChance(), this);
                    _damaging = true;
                    PlayGetHitEffectPoint(gethitPoint);

                    AudioManager.Instance.PlaySwordHitSound();
                }

                HungryZombie hungryZombie = other.GetComponent<HungryZombie>();
                if (hungryZombie != null && !hungryZombie._isDead)
                {
                    hungryZombie.TakeDamaged(CriticalChance(), this);
                    _damaging = true;
                    PlayGetHitEffectPoint(gethitPoint);

                    AudioManager.Instance.PlaySwordHitSound();
                }
            }
        }

        if (Stage_3.Instance != null && Stage_3.Instance._isDropZone && other.CompareTag("Ground"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Destroy(this.gameObject, 1.5f);
                    Stage_3.Instance.DropZoneWeaponCount();
                }
            }
        }
    }

    private void PlayGetHitEffectPoint(Vector3 point)
    {
        if (_gethitEffect != null)
        {
            GameObject effectInstance = Instantiate(_gethitEffect, point, Quaternion.identity);
            effectInstance.SetActive(true);

            ParticleSystem effect = effectInstance.GetComponent<ParticleSystem>();
            if(effect != null)
            {
                effect.Play();
            }
        }
    }
}