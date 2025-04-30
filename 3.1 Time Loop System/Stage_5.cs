using UnityEngine;

public class Stage_5 : Stage
{
    // �������� 5: ���� ������ �� ���� ���� �ʰ� ���ѽð� ���� ����
    // ��: ���� & �� ���ļ� �������� 5~10���� ����.
    // ����: 5�� �̻� ����

    private bool _getDamaged = false;

    protected override void Start()
    {
        base.Start();

        _weaponsSpawner.MakeSwords();
        _weaponsSpawner.MakeRifles();
        _enemiesSpawner.MixMakeMonsters();
    }

    protected override void StartStage()
    {
        base.StartStage();
    }

    protected override bool CheckClear()
    {
        return !_getDamaged;
    }

    protected override void EndStage()
    {
        _isStart = false;

        if(!_getDamaged && GameMain.Instance != null)
        {
            Invoke("DelayedStageClear", 3f);
        }
        else if(_getDamaged && GameMain.Instance != null)
        {
            Invoke("DelayedStageGameOver", 3f);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Do()
    {
    }

    public void GetDamaged()
    {
        _getDamaged = true;

        EndStage();
    }
}
