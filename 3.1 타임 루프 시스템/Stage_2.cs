using UnityEngine;

public class Stage_2 : Stage
{
    // 스테이지 2: 지정된 특정 무기로 적 1마리 처치
    // 적: 무작위 랜덤
    // 무기: 무작위 랜덤

    public WeaponController _selectedWeapon;

    private int _killCount = 0;

    protected override void Start()
    {
        base.Start();

        _weaponsSpawner.MakeSwords();
        _weaponsSpawner.MakeRifles();
        _enemiesSpawner.MakeGolems(2);
        _enemiesSpawner.MakeZombies(3);
    }

    protected override void StartStage()
    {
        base.StartStage();

        Invoke("RandomSelectWeapon", 0.5f);
    }

    protected override bool CheckClear()
    {
        return _killCount >= 1;
    }

    protected override void EndStage()
    {
        base.EndStage();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Do()
    {
    }

    public void EnemyOneKilled(WeaponController selectedWeapon)
    {
        if(selectedWeapon == _selectedWeapon || (selectedWeapon != null && selectedWeapon._isSeletected))
        {
            _killCount++;

            if (CheckClear())
            {
                EndStage();
            }
        } 
    }

    private void RandomSelectWeapon()
    {

        GameObject[] swords = GameObject.FindGameObjectsWithTag("Sword");

        if(swords.Length > 0)
        {
            int randomIndex = Random.Range(0, swords.Length);
            GameObject selectedSword = swords[randomIndex];
            WeaponController selectedsword = selectedSword.GetComponent<WeaponController>();

            _selectedWeapon = selectedsword;
            _selectedWeapon._ignoreDurability = true;
            _selectedWeapon._isSeletected = true;

            LightSelectedWeapon();
        }
    }

    private void LightSelectedWeapon()
    {
        if (_selectedWeapon != null)
        {
            Light pointLight = _selectedWeapon.gameObject.GetComponent<Light>();
            if (pointLight == null)
            {
                pointLight = _selectedWeapon.gameObject.AddComponent<Light>();
            }

            pointLight.type = LightType.Point;
            pointLight.intensity = 3f;
            pointLight.range = 10f;
            pointLight.color = Color.white;
        }
    }
}
