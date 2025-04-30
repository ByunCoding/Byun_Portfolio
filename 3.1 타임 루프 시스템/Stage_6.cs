using UnityEngine;

public class Stage_6 : Stage
{
    // �������� 6: ���ѽð� ���� �� �� ���� �� óġ
    //           (_laptime�� 8~12�� �� �������� �־�����, �� �߿� ų�� �ؾ߸� �ϴ� �ð� 1�� �־�����, �ٸ� �ð����� ������ �԰ų� ������ ���ӿ���)
    // ��: ������ ����
    // ����: ������ ����

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
