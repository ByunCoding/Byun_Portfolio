using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameView : UIView
{
    [SerializeField] private Text _lapTimeText;

    [SerializeField] private GameObject _mainMenuCamera;
    [SerializeField] private GameObject _gameViewCamera;

    [SerializeField] private GameObject _missionView;
    [SerializeField] private GameObject _minimapView;

    private float _warningTime = 60f;

    void Start()
    {
        _mainMenuCamera.SetActive(false);
        _gameViewCamera.SetActive(true);
        _missionView.SetActive(true);
    }

    void Update()
    {
        if(GameMain.Instance != null && GameMain.Instance._currentStage != null)
        {
            float timeValue = GameMain.Instance._currentStage.CurrentLapTime;
            int minutes = Mathf.FloorToInt(timeValue / 60f);
            int seconds = Mathf.FloorToInt(timeValue % 60f);

            _lapTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if(timeValue <= _warningTime)
            {
                _lapTimeText.color = Color.red;
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
