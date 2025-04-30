using Unity.VisualScripting;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    private Stage _currentStage;

    public Stage CurrentStage
    {
        set
        {
            _currentStage = value;
        }

        get
        {
            return _currentStage;
        }
    }

    public void PlayGame()
    {
        _currentStage.PlayGame();
    }

    public void Do()
    {
        _currentStage.Do();
    }
}
