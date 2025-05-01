using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuView : UIView
{
    [SerializeField] private GameObject _mainMenuCamera;
    [SerializeField] private GameObject _gameViewCamera;
    [SerializeField] private MissionView _missionView;
    [SerializeField] private DescriptionView _descriptionView;
    [SerializeField] private Text _startButtonText;

    private bool _isGameStarted = false;

    private Color normalColor = new Color(255f/255f, 191f/255f, 0f/255f);
    private Color changeColor = Color.red;

    private void Start()
    {
        _gameViewCamera.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        if (UIManager.Instance._isGameStarted)
        {
            _startButtonText.text = "재시작";
        }
        else
        {
            _startButtonText.text = "게임 시작";
        }
    }

    public void OnClickStartButton()
    {
        _mainMenuCamera.SetActive(false);

        UIManager.Instance.ShowGameView();
        GameMain.Instance.StartGame();
    }

    public void OnClickDescriptionButton()
    {
        UIManager.Instance.PushDescriptionButton();
    }

    public void OnClickOptionButton()
    {
        UIManager.Instance.PushOptionButton();
    }
}
