using UnityEngine;
using UnityEngine.UI;

public class DescriptionView : UIView
{
    [SerializeField] private GameObject _gameStoryView;
    [SerializeField] private GameObject _processView;
    [SerializeField] private GameObject _controlsView;

    [SerializeField] private Button _gameStoryButton;
    [SerializeField] private Button _processButton;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _backButton;

    private GameObject _currentView = null;

    void Start()
    {
        _gameStoryView.SetActive(false);
        _processView.SetActive(false);
        _controlsView.SetActive(false);
    }

    public void OnClickBackButton()
    {
        UIManager.Instance.PushBackButton();
    }

    public void OnClickDescriptionBackButton()
    {
        if(_currentView != null)
        {
            _currentView.SetActive(false);
        }
    }

    public void OnClickGameStoryButton()
    {
        OpenView(_gameStoryView);
    }

    public void OnClickProcessButton()
    {
        OpenView(_processView);
    }

    public void OnClickControlsButton()
    {
        OpenView(_controlsView);
    }

    private void OpenView(GameObject newView)
    {
        if(_currentView != null)
        {
            _currentView.SetActive(false);
        }

        newView.SetActive(true);
        _currentView = newView;
    }

    public override void Show()
    {
        base.Show();
    }

    public override void UnShow()
    {
        base.UnShow();

        if (_gameStoryView != null)
        {
            _gameStoryView.SetActive(false);
        }
        if (_processView != null)
        {
            _processView.SetActive(false);
        }
        if (_controlsView != null)
        {
            _controlsView.SetActive(false);
        }
    }

}