using UnityEngine;

public class Stage_6 : Stage
{
    // 스테이지 6: 제한시간 내에 단 한 마리 적 처치
    //           (_laptime이 8~12분 중 랜덤으로 주어지고, 그 중에 킬을 해야만 하는 시간 1분 주어지고, 다른 시간에는 데미지 입거나 입히면 게임오버)
    // 적: 무작위 랜덤
    // 무기: 무작위 랜덤

    private int _killCount = 0;

    private bool _activeKill = false;
    private float _activeKillTotalTime = 60f;
    private float _activeKillStartTime = 0;


    protected override void Start()
    {
        base.Start();

        _weaponsSpawner.MakeSwords();
        _weaponsSpawner.MakeRifles();
        _enemiesSpawner.MakeMonsters();
    }

    protected override void StartStage()
    {
        base.StartStage();

        _activeKillStartTime = Random.Range(_activeKillTotalTime, _lapTime);
        Invoke("StartKillTime", _activeKillStartTime);
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


    public void KillTime()
    {
        if (_activeKill)
        {
            _killCount++;
            if (CheckClear())
            {
                EndStage();
            }
        }
        else
        {
            GameMain.Instance.GameOver();
        }
    }

    public void GetDamaged()
    {
        GameMain.Instance.GameOver();
    }

    private void StartKillTime()
    {
        Debug.Log("StartKillTime");
        _activeKill = true;
        Invoke("EndKillTime", _activeKillTotalTime);
    }

    private void EndKillTime()
    {
        Debug.Log("EndKillTime");
        _activeKill = false;
        if(_killCount < 1)
        {
            GameMain.Instance.GameOver();
        }
    }

    public override void Do() { }
}
