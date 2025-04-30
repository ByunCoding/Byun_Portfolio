using UnityEngine;

public class Stage_4 : Stage
{
    // �������� 4: ������ ���� ��� ���̱�
    // ��: ���� & �� ���ļ� �������� 5~10���� ����.
    // ����: 5�� �̻� ����

    private int _killCount = 0;
    private int _totalMonstersCount = 0;

    protected override void Start()
    {
        base.Start();

        _weaponsSpawner.MakeSwords();
        _weaponsSpawner.MakeRifles();
        _enemiesSpawner.MixMakeMonsters();

        int zombieCount = GameObject.FindGameObjectsWithTag("Zombie").Length;
        int golemCount = GameObject.FindGameObjectsWithTag("Golem").Length;
        _totalMonstersCount = zombieCount + golemCount;
    }

    protected override void StartStage()
    {
        base.StartStage();
    }

    protected override bool CheckClear()
    {
        return _killCount >= _totalMonstersCount;
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

    public void AllKilledCount()
    {
        _killCount++;
        if (CheckClear())
        {
            EndStage();
        }
    }
}
