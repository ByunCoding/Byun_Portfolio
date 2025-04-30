using UnityEngine;

public enum WeaponType
{
    Sword,
    Rifle
}

public enum WeaponState
{
    Ready, // �ൿ ����
    Attacking, // ���� ��
    Reloading, // ������ ��
    Destroyed  // �ı��� ����
}

public class WeaponController : MonoBehaviour
{
    public WeaponType weaponType;
    public WeaponState weaponState = WeaponState.Ready;

    public float _attackDamage;
    protected int _criticalHit; // ġ��Ÿ Ȯ��

    public int _durability; // ���� ������
    protected int _maxDurability; // �ִ� ������

    protected float _attackCooldown;
    protected float _reloadingCooldown;
    public bool _ignoreDurability = false;
    public bool _isSeletected = false;

    protected bool _isDestroyed; // �ı� ����
    protected bool _isAttacking; // ���� �� ����
    protected bool _isReloading; // ������ �� ����
    protected bool _isEquipped; // ���� ����
    protected bool _isEquipping; // ���� ������ ����
    protected bool _isPickable; // �ݱ� ���� ����

    protected float _swordAttackingSpeed; // sword �ֵθ��� �ӵ�
    protected float _rifleFiringSpeed; // �߻� �ӵ�
    protected float _reloadTime; // ������ �ð�

    public Vector3 positionOffset;

    void Start()
    {
        InitialRandomStats();
        weaponState = WeaponState.Ready;
    }

    protected virtual void Update()
    {
        if(weaponState == WeaponState.Attacking)
        {
            _attackCooldown -= Time.deltaTime;
            if(_attackCooldown <= 0)
            {
                weaponState = WeaponState.Ready;
            }
        }

        if (weaponState == WeaponState.Reloading)
        {
            _reloadingCooldown -= Time.deltaTime;
            if (_reloadingCooldown <= 0)
            {
                weaponState = WeaponState.Ready;
            }
        }
    }

    protected virtual void InitialRandomStats()
    {
        _criticalHit = Random.Range(1, 20);

        if (weaponType == WeaponType.Sword)
        {
            //_attackDamage = 200f;
            _attackDamage = Random.Range(15.0f, 25.0f);
            _swordAttackingSpeed = Random.Range(1.0f, 3.0f);
            _attackCooldown = _swordAttackingSpeed;
        }
        else if (weaponType == WeaponType.Rifle)
        {
            //_attackDamage = 200f;
            _attackDamage = Random.Range(15.0f, 20.0f);
            _rifleFiringSpeed = Random.Range(0.05f, 0.25f);
            _reloadTime = Random.Range(1.0f, 2.0f);
            _attackCooldown = _rifleFiringSpeed;
            _reloadingCooldown = _reloadTime;
        }

        _durability = Random.Range(10, 20);

        _isDestroyed = false;
        _isEquipped = false;
        _isEquipping = false;
        _isPickable = true;
    }

    protected virtual void Durability(int amount)
    {
        if (_ignoreDurability) return;
        if (_isDestroyed) return;

        if(weaponType == WeaponType.Sword)
        {
            _durability -= amount;
        }
        else if(weaponType == WeaponType.Rifle)
        {
            return;
        }

        if (_durability <= 0)
        {
            _isDestroyed = true;
            weaponState = WeaponState.Destroyed;
            Destroy(gameObject);
        }
    }

    protected virtual float CriticalChance()
    {
        int randomValue = Random.Range(1, 100);

        if (randomValue <= _criticalHit)
        {
            return _attackDamage *= 2.0f;
        }

        return _attackDamage;
    }

    public virtual bool CanAttack()
    {
        return weaponState == WeaponState.Ready && !_isDestroyed;
    }

    public virtual void Attack()
    {
        if(CanAttack())
        {
            Durability(1);

            weaponState = WeaponState.Attacking;
            if(weaponType == WeaponType.Sword)
            {
                _attackCooldown = _swordAttackingSpeed;
            }
            else if(weaponType == WeaponType.Rifle)
            {
                _attackCooldown = _rifleFiringSpeed;
            }
        }
    }

    public virtual void Reload()
    {
        if(weaponType == WeaponType.Rifle && weaponState == WeaponState.Ready)
        {
            weaponState = WeaponState.Reloading;
            _reloadingCooldown = _reloadTime;
        }
    }

    public virtual bool IsPickable()
    {
        return _isPickable && !_isEquipped && !_isDestroyed;
    }

    public virtual void Equip()
    {
        if(!_isEquipped && _isPickable)
        {
            _isEquipped = true;
            _isEquipping = true;
            _isPickable = false;
        }
    }

    public virtual void Unequip()
    {
        if (_isEquipped)
        {
            _isEquipped = false;
            _isPickable = true;
        }
    }




}