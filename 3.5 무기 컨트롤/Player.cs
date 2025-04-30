using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Jump,
    GetHit,
    Die,
    Attack
}

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _healthBarPrefab;
    private Slider _hpBar;

    [SerializeField] private Transform _rightHandPos;

    private CameraCtrl _cameraCtrl;
    private AudioListener _audioListener;

    private float _walkSpeed = 4.0f;
    private float _runSpeed = 8.0f;
    private float _rotSpeed = 200.0f;
    private float _jumpForce = 5.0f;
    private float _groundCheckDistance = 0.5f;
    private bool _isGrounded;
    private float _moveSpeed;
    private bool _isMoving = false;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerState _currentState = PlayerState.Idle;

    public WeaponController _currentWeapon;
    private WeaponController _pickingWeapon;
    private Potions _pickingPotion;
    private float _pickupRange = 1.0f;
    private bool _isPickingUp = false;

    private int _currentAttack = 0;
    private bool _isAttacking = false;
    private float _lastAttackTime = 0f;
    private float _comboResetTime = 3.0f;

    private float _gethitCoolTime = 4.0f;
    private float _nextGetHitTime = 0f;
    public float _maxHealth = 100.0f;
    public float _currentHealth;
    public bool _isDead = false;

    private Dictionary<string, Transform> _rifleTransforms = new Dictionary<string, Transform>();

    public PlayerState CurrentState
    {
        get
        { 
            return _currentState; 
        }
    }

    void Start()
    {
        _cameraCtrl = FindAnyObjectByType<CameraCtrl>();
        _audioListener = GetComponent<AudioListener>();

        _animator = GetComponent<Animator>();
        _animator.SetLayerWeight(1, 0f);

        _rigidbody = GetComponent<Rigidbody>();
        //RenderSettings.ambientLight = Color.black;
        //RenderSettings.ambientIntensity = 0.0f;
        //RenderSettings.reflectionIntensity = 0.0f;

        _currentHealth = _maxHealth;

        GameObject hpBarInstance = Instantiate(_healthBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        hpBarInstance.transform.SetParent(this.transform, true);

        _hpBar = hpBarInstance.GetComponentInChildren<Slider>();

        if (_hpBar != null)
        {
            _hpBar.maxValue = _maxHealth;
            _hpBar.value = _currentHealth;
        }

    }


    void FixedUpdate()
    {
        CheckGrounded();
        Move();
        UpdateAnimation();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(!TryPickupPotion())
            {
                if (_currentWeapon == null)
                {
                    TryPickupWeapon();
                }
                else
                {
                    DropWeapon();
                }
            }
        }

        Aiming();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_currentWeapon != null && _currentWeapon.gameObject.CompareTag("Rifle"))
            {
                Rifle rifle = _currentWeapon as Rifle;
                if(rifle != null && rifle._bulletCount < rifle._maxBullet)
                {
                    _animator.SetTrigger("RifleReload");
                    _currentWeapon.Reload();
                }
            }
        }

        if (_currentWeapon != null && _currentWeapon.gameObject.CompareTag("Rifle"))
        {
            if (_currentState == PlayerState.Run)
            {
                _animator.SetBool("RifleShoot", false);
            }
            else
            {
                _animator.SetBool("RifleShoot", Input.GetMouseButton(0));
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && _currentWeapon != null)
            {
                Attack();
            }
        }

        if(_currentWeapon != null && _currentWeapon.weaponState == WeaponState.Destroyed)
        {
            _currentWeapon = null;
            if (_isAttacking)
            {
                _isAttacking = false;
                _currentAttack = 0;
            }
        }

        if(!_isAttacking && Time.time > _lastAttackTime + _comboResetTime && _currentAttack != 0)
        {
            _currentAttack = 0;
        }

        if (_currentWeapon != null && _currentWeapon.gameObject.CompareTag("Rifle"))
        {
            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
            {
                Jump();
            }
        }

        if (_hpBar != null)
        {
            _hpBar.value = _currentHealth;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.PushMenuButton();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.Instance.ShowMissionView();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            UIManager.Instance.ShowGameView();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;
        if (_isAttacking) return;

        Golems golems = other.GetComponentInParent<Golems>();
        FemaleZombie femaleZombie = other.GetComponentInParent<FemaleZombie>();
        GhoulZombie ghoulZombie = other.GetComponentInParent<GhoulZombie>();
        ModernZombie modernZombie = other.GetComponentInParent<ModernZombie>();
        HungryZombie humgryZombie = other.GetComponentInParent<HungryZombie>();


        if ((golems != null && golems.IsAttack())
            || (femaleZombie != null && femaleZombie.IsAttack())
            || (ghoulZombie != null && ghoulZombie.IsAttack())
            || (modernZombie != null && modernZombie.IsAttack())
            || (humgryZombie != null && humgryZombie.IsAttack()))
        {
            if (Time.time >= _nextGetHitTime)
            {
                _nextGetHitTime = Time.time + 4.0f;
                _currentState = PlayerState.GetHit;
            }
        }
    }

    public void GetDamaged(float damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        Stage currentStage = GameMain.Instance._currentStage;
        if (currentStage is Stage_5 stage_5)
        {
            stage_5.GetDamaged();
        }
        else if(currentStage is Stage_6 stage_6)
        {
            stage_6.GetDamaged();
        }

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
            return;
        }

        if (_hpBar != null)
        {
            _hpBar.value = _currentHealth;
        }
    }

    private void Die()
    {
        _currentState = PlayerState.Die;

        GameMain.Instance.GameOver();
    }

    private void Attack()
    {
        if (_currentWeapon == null || !_currentWeapon.CanAttack()) return;

        _isAttacking = true;
        _lastAttackTime = Time.time;

        if (_currentWeapon.gameObject.CompareTag("Rifle"))
        {
            _animator.SetTrigger("RifleShoot");
        }
        else if(_currentWeapon.gameObject.CompareTag("Sword"))
        {
            if (Time.time > _lastAttackTime + _comboResetTime)
            {
                _currentAttack = 0;
            }

            _currentAttack++;

            if (_currentAttack > 4)
            {
                _currentAttack = 1;
            }

            _animator.SetTrigger("Attack" + _currentAttack);
        }

        _currentWeapon.Attack();
    }

    public void EffectOn()
    {
        if (_currentWeapon != null)
        {
            Sword sword = _currentWeapon as Sword;
            if (sword != null)
            {
                sword.EnableEffect();
            }
        }
    }

    public void EffectOff()
    {
        if (_currentWeapon != null)
        {
            Sword sword = _currentWeapon as Sword;
            if (sword != null)
            {
                sword.DisableEffect();
            }
        }
    }

    public void SwordColliderOn()
    {
        if (_currentWeapon != null && _currentWeapon is Sword sword) 
        { 
            sword.EnableSwordCollider(); 
        }
    }

    public void SwordColliderOff()
    {
        if (_currentWeapon != null && _currentWeapon is Sword sword)
        {
            sword.DisableSwordCollider();
        }
    }

    public void OnAttackAnimationStart()
    {
        _isAttacking = true;
    }


    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
        _lastAttackTime = Time.time;

        if (!_isDead && Input.GetMouseButton(0))
        {
            Attack();
        }
    }

    private void OnGetHitAnimationEnd()
    {
        _currentState = PlayerState.Idle;

        if (Input.GetMouseButton(0))
        {
            Attack();
        }
    }

    private void TryPickupWeapon()
    {
        if (_currentWeapon != null || _isPickingUp) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupRange);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Sword"))
            {
                WeaponController weapon = col.GetComponent<Sword>();

                if (weapon != null && weapon.IsPickable())
                {
                    _animator.SetTrigger("Pick");
                    _pickingWeapon = weapon;
                    _isPickingUp = true;
                    break;
                }
            }

            if (col.CompareTag("Rifle"))
            {
                WeaponController weapon = col.GetComponent<Rifle>();

                if (weapon != null && weapon.IsPickable())
                {
                    _animator.SetTrigger("Pick");
                    _pickingWeapon = weapon;
                    _isPickingUp = true;
                    break;
                }
            }

        }
    }

    private bool TryPickupPotion()
    {
        if (_isPickingUp) return false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupRange);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Potion"))
            {
                Potions potion = col.GetComponent<Potions>();

                if (potion != null && potion.IsPickable())
                {
                    _animator.SetTrigger("Pick");
                    _pickingPotion = potion;
                    _isPickingUp = true;
                    return true;
                }
            }
        }

        return false;
    }

    public void OnPickAnimationMoment()
    {
        if(_pickingWeapon != null)
        {
            EquipWeapon(_pickingWeapon);
            _pickingWeapon = null;
        }
        
        if(_pickingPotion != null)
        {
            _pickingPotion.UsePotion();
            _pickingPotion = null;
        }

        _isPickingUp = false;
    }

    private void DropWeapon()
    {
        if(_currentWeapon != null)
        {
            _currentWeapon.Unequip();
            _currentWeapon.transform.SetParent(null, false);

            Vector3 dropPos = transform.position + transform.forward * 0.5f + transform.up * 0.2f;

            NavMeshHit hit;
            if(NavMesh.SamplePosition(dropPos, out hit, 5f, NavMesh.AllAreas))
            {
                dropPos = hit.position;
            }

            _currentWeapon.transform.position = dropPos;
            if(_currentWeapon.gameObject.CompareTag("Sword"))
            {
                _currentWeapon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
            else if (_currentWeapon.gameObject.CompareTag("Rifle"))
            {
                string rifleType = _currentWeapon.gameObject.name;

                if (rifleType.Contains("Rifle_1"))
                {
                    _currentWeapon.transform.rotation = Quaternion.Euler(17.441f, 2.8913f, -27.017f);
                }
                else if (rifleType.Contains("Rifle_2"))
                {
                    _currentWeapon.transform.rotation = Quaternion.Euler(21.766f, -3.151f, -30.902f);
                }
                else if (rifleType.Contains("Rifle_3"))
                {
                    _currentWeapon.transform.rotation = Quaternion.Euler(20.717f, 0f, -38.368f);
                }
                else if (rifleType.Contains("Rifle_4"))
                {
                    _currentWeapon.transform.rotation = Quaternion.Euler(19.444f, 23.3f, -36.156f);
                }
                else if (rifleType.Contains("Rifle_5"))
                {
                    _currentWeapon.transform.rotation = Quaternion.Euler(13.54f, 0, -30.805f);
                }
                _animator.SetLayerWeight(1, 0f);

                Rigidbody rifleRb = _currentWeapon.GetComponent<Rigidbody>();
                if (rifleRb == null)
                {
                    rifleRb = _currentWeapon.gameObject.AddComponent<Rigidbody>();
                    rifleRb.isKinematic = true;
                }

                IKControl ikControl = GetComponent<IKControl>();
                if(ikControl != null)
                {
                    ikControl._ikActive = false;
                }
            }

            if(MapManager.Instance != null)
            {
                MapManager.Instance.AdjustOnNavMesh(_currentWeapon.gameObject, hit.position.y);

                if (_currentWeapon.gameObject.CompareTag("Rifle"))
                {
                    string rifleType = _currentWeapon.gameObject.name;

                    if(rifleType.Contains("Rifle_2") || rifleType.Contains("Rifle_3") || rifleType.Contains("Rifle_4") || rifleType.Contains("Rifle_5"))
                    {
                        Vector3 riflePos = _currentWeapon.transform.position;
                        riflePos.y = 0.06f;
                        _currentWeapon.transform.position = riflePos;
                    }
                }
            }

            Rigidbody rb = _currentWeapon.GetComponent<Rigidbody>();
            if (rb != null && Stage_3.Instance != null && Stage_3.Instance._isDropZone)
            {
                rb.isKinematic = false;
            }

            _currentWeapon = null;
        }
    }

    private void EquipWeapon(WeaponController weapon)
    {
        _currentWeapon = weapon;

        if (weapon.gameObject.CompareTag("Sword"))
        {
            _animator.SetLayerWeight(1, 0f);

            weapon.transform.SetParent(_rightHandPos, false);
            weapon.transform.localPosition = weapon.positionOffset;
            weapon.transform.Rotate(0f, 180f, 0f);
            //weapon.transform.localRotation = Quaternion.Euler(59.587f, 94.661f, 78.467f);
        }
        else if (weapon.gameObject.CompareTag("Rifle"))
        {
            _animator.SetLayerWeight(1, 1f);

            CatchRifleTransforms();

            Rigidbody rg = weapon.GetComponent<Rigidbody>();
            Destroy(rg);

            weapon.transform.SetParent(_rightHandPos, false);
            weapon.transform.localPosition = weapon.positionOffset;
            //weapon.transform.Rotate(0f, 180f, 0f);
            weapon.transform.localRotation = Quaternion.Euler(0,0,0);
            //weapon.transform.localRotation = Quaternion.Euler(59.587f, 94.661f, 78.467f);

            IKControl ikControl = GetComponent<IKControl>();
            if (ikControl != null)
            {
                Rifle rifle = weapon as Rifle;
                if(rifle != null)
                {
                    ikControl._ikActive = true;
                    ikControl.SetHandPos(rifle.GetLeftHand());
                }
            }

        }

        weapon.Equip();
    }

    private void CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, _groundCheckDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                _isGrounded = true;
                return;
            }
        }

        _isGrounded = false;
    }


    private void Move()
    {

        if (_isPickingUp || _isAttacking || _currentState == PlayerState.GetHit || _currentState == PlayerState.Die) return;

        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if (_animator.GetBool("Aiming"))
        {
            float rotateSpeedAiming = _rotSpeed * Time.fixedDeltaTime;
            transform.Rotate(Vector3.up * hor * rotateSpeedAiming);

            _currentState = PlayerState.Idle;
            return;
        }

        _isMoving = Mathf.Abs(ver) > 0;

        if (_isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _currentState = PlayerState.Run;
                _moveSpeed = _runSpeed * Time.fixedDeltaTime;
            }
            else
            {
                _currentState = PlayerState.Walk;
                _moveSpeed = _walkSpeed * Time.fixedDeltaTime;
            }
        }
        else
        {
            _currentState = PlayerState.Idle;
        }

        if(_currentWeapon != null && _currentWeapon.weaponType == WeaponType.Rifle)
        {
            _moveSpeed *= 0.9f;
        }

        _rigidbody.MovePosition(_rigidbody.position + transform.forward * ver * _moveSpeed);

        float rotateSpeed = _rotSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * hor * rotateSpeed);
    }

    private void Move2()
    {
        if (_isPickingUp || _isAttacking || _currentState == PlayerState.GetHit || _currentState == PlayerState.Die) return;

        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        _isMoving = Mathf.Abs(ver) > 0;


        if (_isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _currentState = PlayerState.Run;
                _moveSpeed = _runSpeed * Time.fixedDeltaTime;
            }
            else
            {
                _currentState = PlayerState.Walk;
                _moveSpeed = _walkSpeed * Time.fixedDeltaTime;
            }

            if (ver < 0)
            {
                Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                transform.rotation = targetRotation;

                _rigidbody.MovePosition(_rigidbody.position + transform.forward * Mathf.Abs(ver) * _moveSpeed);
            }
        }
        else
        {
            _currentState = PlayerState.Idle;
        }
        
        float rotateSpeed = _rotSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * hor * rotateSpeed);
    }
    

    private void Jump()
    {
        _currentState = PlayerState.Jump;
        _animator.SetTrigger("Jump");
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _isGrounded = false;

        if (_currentWeapon != null && _currentWeapon.gameObject.CompareTag("Rifle"))
        {
            _animator.SetTrigger("RifleJump");
        }
    }

    private void Aiming()
    {
        if (_currentWeapon != null && _currentWeapon.gameObject.CompareTag("Rifle")
            && _currentWeapon.weaponState == WeaponState.Reloading)
        {
            _animator.SetBool("Aiming", false);

            if (_cameraCtrl != null)
            {
                _cameraCtrl._isAiming = false;
            }
            return;
        }

        if (_currentWeapon == null || !_currentWeapon.CompareTag("Rifle"))
        {
            _animator.SetBool("Aiming", false);
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            _animator.SetBool("Aiming", true);

            if(_currentWeapon != null)
            {
                if(_rifleTransforms.TryGetValue("Rifle1", out Transform rifle1) && rifle1 != null)
                {
                    rifle1.localPosition = new Vector3(-0.9746313f, -3.028207f, 1.436547f);
                    rifle1.localRotation = Quaternion.Euler(-57.969f, -274.28f, 79.251f);
                }
                if (_rifleTransforms.TryGetValue("Rifle2", out Transform rifle2) && rifle2 != null)
                {
                    rifle2.localPosition = new Vector3(0.0662129f, 0.0227572f, 0.098902f);
                    rifle2.localRotation = Quaternion.Euler(43.515f, -101.027f, -80.331f);
                }
                if (_rifleTransforms.TryGetValue("Rifle3", out Transform rifle3) && rifle3 != null)
                {
                    rifle3.localPosition = new Vector3(-0.03888056f, -0.0925171f, 0.003160442f);
                    rifle3.localRotation = Quaternion.Euler(48.971f, -101.813f, -84.558f);
                }
                if (_rifleTransforms.TryGetValue("Rifle4", out Transform rifle4) && rifle4 != null)
                {
                    rifle4.localPosition = new Vector3(-0.01210897f, -0.04896785f, 0.006419036f);
                    rifle4.localRotation = Quaternion.Euler(37.118f, -99.578f, -79.551f);
                }
                if (_rifleTransforms.TryGetValue("Rifle5", out Transform rifle5) && rifle5 != null)
                {
                    rifle5.localPosition = new Vector3(-0.058f, -0.177f, 0.036f);
                    rifle5.localRotation = Quaternion.Euler(35.24f, -102.247f, -79.498f);
                }
            }

            if (_cameraCtrl != null)
            {
                _cameraCtrl._isAiming = true;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            _animator.SetBool("Aiming", false);

            if (_currentWeapon != null)
            {
                if (_rifleTransforms.TryGetValue("Rifle1", out Transform rifle1back) && rifle1back != null)
                {
                    rifle1back.localPosition = new Vector3(-3.14f, -1.43f, 1.33f);
                    rifle1back.localRotation = Quaternion.Euler(-28.093f, -263.569f, 69.554f);
                }
                if (_rifleTransforms.TryGetValue("Rifle2", out Transform rifle2back) && rifle2back != null)
                {
                    rifle2back.localPosition = new Vector3(0.069f, 0.016f, 0.069f);
                    rifle2back.localRotation = Quaternion.Euler(31.3363f, -83.703f, -63.625f);
                }
                if (_rifleTransforms.TryGetValue("Rifle3", out Transform rifle3back) && rifle3back != null)
                {
                    rifle3back.localPosition = new Vector3(-0.075f, -0.082f, 0.071f);
                    rifle3back.localRotation = Quaternion.Euler(37.793f, -81.005f, -67.084f);
                }
                if (_rifleTransforms.TryGetValue("Rifle4", out Transform rifle4back) && rifle4back != null)
                {
                    rifle4back.localPosition = new Vector3(-0.04828492f, -0.03307315f, 0.06328731f);
                    rifle4back.localRotation = Quaternion.Euler(34.932f, -84.271f, -70.45f);
                }
                if (_rifleTransforms.TryGetValue("Rifle5", out Transform rifle5back) && rifle5back != null)
                {
                    rifle5back.localPosition = new Vector3(-0.1504342f, -0.06202691f, 0.1536856f);
                    rifle5back.localRotation = Quaternion.Euler(28.676f, -81.005f,  -71.919f);
                }
            }

            _audioListener = GetComponent<AudioListener>();
            if (_cameraCtrl != null)
            {
                _cameraCtrl._isAiming = false;
            }
        }
    }

    private void CatchRifleTransforms()
    {
        if (_currentWeapon == null) return;

        _rifleTransforms.Clear();

        Transform rifle1 = _currentWeapon.transform.Find("Rifle1");
        if(rifle1 != null)
        {
            _rifleTransforms["Rifle1"] = rifle1;
        }

        Transform rifle2 = _currentWeapon.transform.Find("Rifle2");
        if (rifle2 != null)
        {
            _rifleTransforms["Rifle2"] = rifle2;
        }

        Transform rifle3 = _currentWeapon.transform.Find("Rifle3");
        if (rifle3 != null)
        {
            _rifleTransforms["Rifle3"] = rifle3;
        }

        Transform rifle4 = _currentWeapon.transform.Find("Rifle4");
        if (rifle4 != null)
        {
            _rifleTransforms["Rifle4"] = rifle4;
        }

        Transform rifle5 = _currentWeapon.transform.Find("Rifle5");
        if (rifle5 != null)
        {
            _rifleTransforms["Rifle5"] = rifle5;
        }
    }

    private void UpdateAnimation()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);

        _animator.SetBool("RifleWalk", false);
        _animator.SetBool("RifleRun", false);

        if (_currentWeapon != null && _currentWeapon.gameObject.CompareTag("Rifle"))
        {
            switch (_currentState)
            {
                case PlayerState.Idle:
                    break;

                case PlayerState.Walk:
                    _animator.SetBool("RifleWalk", true);
                    break;

                case PlayerState.Run:
                    _animator.SetBool("RifleRun", true);
                    break;

                case PlayerState.GetHit:
                    _animator.SetTrigger("GetHit");
                    _isAttacking = false;
                    _currentState = PlayerState.Idle;
                    break;

                case PlayerState.Die:
                    if (!_isDead)
                    {
                        _isDead = true;
                        _animator.SetTrigger("Die");
                        Destroy(this.gameObject, 4.0f);
                    }
                    break;
            }
        }
        else
        {
            switch (_currentState)
            {
                case PlayerState.Idle:
                    break;

                case PlayerState.Walk:
                    _animator.SetBool("Walk", true);
                    break;

                case PlayerState.Run:
                    _animator.SetBool("Run", true);
                    break;

                case PlayerState.Jump:
                    break;

                case PlayerState.GetHit:
                    _animator.SetTrigger("GetHit");
                    _isAttacking = false;
                    _currentState = PlayerState.Idle;
                    break;

                case PlayerState.Die:
                    if (!_isDead)
                    {
                        _isDead = true;
                        _animator.SetTrigger("Die");
                        Destroy(this.gameObject, 4.0f);
                    }
                    break;
            }
        }
    }
}
