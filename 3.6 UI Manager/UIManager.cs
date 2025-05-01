using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private GameView _gameView;
    [SerializeField] private DescriptionView _descriptionView;
    [SerializeField] private OptionView _optionView;
    [SerializeField] private MissionView _missionView;
    [SerializeField] private MiniMapView _minimapView;

    [SerializeField] private GameObject _gameClearText;
    [SerializeField] private GameObject _gameOverText;

    [SerializeField] private Image _aimImage;

    private Stack<UIView> _uiStack = new Stack<UIView>();
    private UIView _currentUIView = null;

    public bool _isGameStarted = false;
    private bool _isGamePause = false;

    static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _mainMenuView.Show();
        _gameView.UnShow();
        _missionView.UnShow();
        _descriptionView.UnShow();
        _optionView.UnShow();
        _minimapView.UnShow();

        _currentUIView = _mainMenuView;

        _gameClearText.SetActive(false);

        _aimImage.gameObject.SetActive(false);
    }

    void PushUIView(UIView view, bool isShow = false)
    {
        if (!isShow)
        {
            view.UnShow();
        }

        _uiStack.Push(view);
    }

    void PopUIView()
    {
        if (_uiStack.Count <= 0) return;
        _currentUIView.UnShow();
        _currentUIView = _uiStack.Pop();
        _currentUIView.Show();
    }

    public void ShowMainMenuView()
    {
        PushUIView(_currentUIView);
        _mainMenuView.Show();
        _missionView.UnShow();
        _minimapView.UnShow();
        _currentUIView = _mainMenuView;

        _isGamePause = true;
        Time.timeScale = 0f;
    }

    public void ShowGameView()
    {
        PushUIView(_currentUIView);
        _isGameStarted = true;
        _gameView.Show();
        _missionView.UnShow();
        _minimapView.Show();
        _currentUIView = _gameView;

        _isGamePause = false;
        Time.timeScale = 1.0f;
    }

    public void ShowMissionView()
    {
        PushUIView(_currentUIView);
        _missionView.Show();
        _minimapView.UnShow();
        _currentUIView = _missionView;

        _isGamePause = true;
        Time.timeScale = 0f;
    }

    public void ShowMissionPopUpView()
    {
        _minimapView.gameObject.SetActive(false);
        _missionView.Show();
        
        _isGamePause = true;
        Time.timeScale = 0.0f;

        DG.Tweening.DOVirtual.DelayedCall(3f, () =>
        {
            _missionView.UnShow();
            _minimapView.gameObject.SetActive(true);
            _isGamePause = false;
            Time.timeScale = 1.0f;
        }, true);
    }

    public void ShowDescriptionView()
    {
        PushUIView(_currentUIView, true);
        _descriptionView.Show();
        _currentUIView = _descriptionView;

        _isGamePause = true;
        Time.timeScale = 0f;
    }

    public void ShowOptionView()
    {
        PushUIView(_currentUIView, true);
        _optionView.Show();
        _currentUIView = _optionView;

        _isGamePause = true;
        Time.timeScale = 0f;
    }

    public void ShowGameClearText()
    {
        _gameClearText.SetActive(true);
    }

    public void HideGameClearText()
    {
        _gameClearText.SetActive(false);
    }

    public void ShowGameOverText()
    {
        _gameOverText.SetActive(true);
    }

    public void HideGameOverText()
    {
        _gameOverText.SetActive(false);
    }

    public void PushMenuButton()
    {
        if (_currentUIView == _gameView)
        {
            ShowMainMenuView();
        }
        else if(_currentUIView == _mainMenuView)
        {
            ShowGameView();
        }
    }

    public void PushDescriptionButton()
    {
        if (_currentUIView == _mainMenuView)
        {
            ShowDescriptionView();
        }
        else
        {
            ShowMainMenuView();
        }
    }

    public void PushOptionButton()
    {
        if (_currentUIView == _mainMenuView)
        {
            ShowOptionView();
        }
        else
        {
            ShowMainMenuView();
        }
    }

    public void PushBackButton()
    {
        if (_currentUIView == _descriptionView)
        {
            ShowMainMenuView();
        }
        else if (_currentUIView == _optionView)
        {
            ShowMainMenuView();
        }
        else
        {
            ShowDescriptionView();
        }
    }

    public void PrevUIView()
    {
        PopUIView();
    }

    public bool IsGamePause()
    {
        return _isGamePause;
    }

    public void SetAimImageActive(bool isActive)
    {
        _aimImage.gameObject.SetActive(isActive);
    }

}