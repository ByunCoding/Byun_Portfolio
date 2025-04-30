using UnityEngine;

public class Stage_5 : Stage
{
    // 스테이지 5: 적의 공격을 한 번도 받지 않고 제한시간 동안 생존
    // 적: 좀비 & 골렘 합쳐서 랜덤으로 5~10마리 정도.
    // 무기: 5개 이상 생성

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
