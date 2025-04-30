using TreeEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Rifle : WeaponController
{
    //private Transform _cameraTransform;

    public AudioListener _audioListener;
    public Camera _rifleCamera;

    private CameraCtrl _cameraCtrl;
    private Camera _aimCamera;

    [SerializeField] private GameObject _bulletEffect;
    [SerializeField] private GameObject _bulletHitEffect;
    
    [SerializeField] public Transform _bulletPos;
    [SerializeField] private float _bulletRange = 200.0f;

    [SerializeField] private Transform _lefthand;
    //[SerializeField] private Transform _righthand;

    public int _maxBullet;
    public int _bulletCount;
    private int _maxReloads;
    public int _reloadsCount;

    //public void SetCameraTransform(Transform cameraTransform)
    //{
    //    _cameraTransform = cameraTransform;
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Vector3 start = _bulletPos.position;
    //    Vector3 end = _bulletPos.position + _bulletPos.forward * _bulletRange;
    //    Gizmos.DrawLine(start, end);
    //}


    private void Awake()
    {
        if (_bulletEffect != null)
        {
            _bulletEffect.SetActive(false);
        }

        _cameraCtrl = FindAnyObjectByType<CameraCtrl>();
    }

    protected override void Update()
    {
        base.Update();

        if(_cameraCtrl != null && _aimCamera == null && _cameraCtrl._aimCameraTr != null)
        {
            _aimCamera = _cameraCtrl._aimCameraTr.GetComponent<Camera>();
        }

        //Debug.DrawRay(_bulletPos.position, _bulletPos.forward * _bulletRange, Color.green);

        if (!_isEquipped) return;

        if (weaponState == WeaponState.Reloading)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) || _bulletCount <= 0)
        {
            Reload();
        }
    }

    protected override void InitialRandomStats()
    {
        base.InitialRandomStats();

        _maxBullet = 200;
        //_maxBullet = Random.Range(10, 25);
        _bulletCount = _maxBullet;
        _maxReloads = _maxBullet * Random.Range(2, 4);
        _reloadsCount = _maxReloads;
    }

    public void Shoot()
    {
        Player player = GetComponentInParent<Player>();
        if (player.CurrentState == PlayerState.Run)
        {
            return;
        }

        if (weaponState == WeaponState.Reloading)
        {
            return;
        }

        if (CanAttack() && _bulletCount > 0)
        {
            Attack();
            FireShooting();
            _bulletCount--;

            PlayRifleShootingSound();

            if (_bulletCount <= 0)
            {
                Reload();
            }
        }
    }

    private void FireShooting()
    {
        Vector3 shootOrigin;
        Vector3 shootDirection;

        if (_cameraCtrl != null && _cameraCtrl._isAiming && _aimCamera != null)
        {
            Ray ray = new Ray(_aimCamera.transform.position, _aimCamera.transform.forward);

            shootOrigin = _bulletPos.position;

            shootDirection = _aimCamera.transform.forward;

            Debug.DrawRay(_aimCamera.transform.position, _aimCamera.transform.forward * _bulletRange, Color.green, 2.0f);
        }
        else
        {
            shootOrigin = _bulletPos.position;
            shootDirection = _bulletPos.forward;
        }

        if (_bulletEffect != null)
        {
            GameObject bulletEffect = Instantiate(_bulletEffect, shootOrigin, Quaternion.LookRotation(-shootDirection));
            bulletEffect.SetActive(true);
            Destroy(bulletEffect, 0.15f);
        }

        EnemyHit(shootOrigin, shootDirection);
    }

    private void EnemyHit(Vector3 origin, Vector3 direction)
    {
        bool enemyHit = false;

        Debug.DrawRay(origin, direction * _bulletRange, Color.green, 2.0f);

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, _bulletRange);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Golem"))
            {
                Golems golems = hit.collider.GetComponent<Golems>();
                if (golems != null)
                {
                    golems.TakeDamaged(CriticalChance(), this);
                    enemyHit = true;
                }
            }
            else if (hit.collider.CompareTag("Zombie"))
            {
                HungryZombie hungryZombie = hit.collider.GetComponent<HungryZombie>();
                if (hungryZombie != null)
                {
                    hungryZombie.TakeDamaged(CriticalChance(), this);
                    enemyHit = true;
                }

                ModernZombie modernZombie = hit.collider.GetComponent<ModernZombie>();
                if (modernZombie != null)
                {
                    modernZombie.TakeDamaged(CriticalChance(), this);
                    enemyHit = true;
                }

                FemaleZombie femaleZombie = hit.collider.GetComponent<FemaleZombie>();
                if (femaleZombie != null)
                {
                    femaleZombie.TakeDamaged(CriticalChance(), this);
                    enemyHit = true;
                }

                GhoulZombie ghoulZombie = hit.collider.GetComponent<GhoulZombie>();
                if (ghoulZombie != null)
                {
                    ghoulZombie.TakeDamaged(CriticalChance(), this);
                    enemyHit = true;
                }
            }

            if (_bulletHitEffect != null && enemyHit)
            {
                GameObject bulletHitEffect = Instantiate(_bulletHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHitEffect, 0.5f);
            }
        }

    }

    public override void Reload()
    {
        if (weaponState == WeaponState.Ready)
        {
            base.Reload();
            int reloadingBulletCount = _maxBullet - _bulletCount;
            int reloadedBulletCount = Mathf.Min(reloadingBulletCount, _reloadsCount);

            _bulletCount += reloadedBulletCount;
            _reloadsCount -= reloadedBulletCount;

            _reloadsCount = Mathf.Max(_reloadsCount, 0);

            if (_reloadsCount <= 0 && _bulletCount <= 0)
            {
                weaponState = WeaponState.Destroyed;
                Destroy(gameObject);
            }
        }
    }

    public Transform GetLeftHand()
    {
        return _lefthand;
    }

    private void PlayRifleShootingSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRifleShootingSound();
        }
    }

    public bool IsReloading()
    {
        return weaponState == WeaponState.Reloading;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Stage_3.Instance != null && Stage_3.Instance._isDropZone && other.CompareTag("Ground"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Destroy(this.gameObject, 1.5f);
                    Stage_3.Instance.DropZoneWeaponCount();
                }
            }
        }
    }

    //public Transform GetRightHand()
    //{
    //    return _righthand;
    //}
}
