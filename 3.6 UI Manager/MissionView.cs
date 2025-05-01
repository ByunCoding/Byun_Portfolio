using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MissionView : UIView
{
    [SerializeField] private Text _missionText;

    void Start()
    {
    }

    void Update()
    {
        MissionText();
    }

    void MissionText()
    {
        if (GameMain.Instance != null && GameMain.Instance._currentStage != null)
        {
            Stage currentStage = GameMain.Instance._currentStage;
            if (currentStage is Stage_1)
            {
                _missionText.text = "3마리 좀비 처치";
            }
            else if (currentStage is Stage_2)
            {
                _missionText.text = "지정된 무기를 찾아서 적 1마리 처치\n" +
                                    "지정된 무기: 빛이 나는 무기";
            }
            else if (currentStage is Stage_3)
            {
                _missionText.text = "생성된 무기들을 지정된 위치에 가져다 놓기\n" +
                                    "지정된 위치: 빛이 있음";
            }
            else if (currentStage is Stage_4)
            {
                _missionText.text = "적 모두 죽이기";
            }
            else if (currentStage is Stage_5)
            {
                _missionText.text = "적의 공격을 한 번도 받지 않고 제한시간 동안 살아남기";
            }
            else if (currentStage is Stage_6)
            {
                _missionText.text = "제한시간 중 공격시간 1분이 주어지면 단 1마리 처치\n" +
                                    "그 외 시간 공격을 당하면 게임오버";
            }
        }
    }

    public override void Show()
    {
        base.Show();
    }

    public override void UnShow()
    {
        base.UnShow();
    }
}
