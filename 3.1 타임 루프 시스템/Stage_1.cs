using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Stage_1 : Stage
{
    // �������� 1: ���� �����ǰ�, ���� 3���� ���̱�
    // ��: ���� 3����
    // ����: 5�� �̻� ����

    private int _weaponSpawnCount = 5;
    private int _zombieCount = 3;
    private int _enemyKillCount = 3;

    private int _killCount = 0;

    protected override void Start()
    {
        base.Start();

        _weaponsSpawner.MakeSwords(5);
        _weaponsSpawner.MakeRifles();
        _enemiesSpawner.MakeZombies(3);
    }

    protected override void StartStage()
    {
        _killCount = 0;
        base.StartStage();
    }

    protected override bool CheckClear()
    {
        return _killCount >= 3;
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

    public void IncreaseKilledCount()
    {
        _killCount++;
        if (CheckClear())
        {
            EndStage();
        }
    }
}
