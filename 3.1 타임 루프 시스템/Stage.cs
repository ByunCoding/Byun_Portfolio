using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : MonoBehaviour
{
    protected EnemiesSpawner _enemiesSpawner;
    protected PlayerSpawner _playerSpawner;
    protected PotionSpawner _potionSpawner;
    protected WeaponsSpawner _weaponsSpawner;

    public Bounds _landBounds;

    public float _lapTime;
    //protected float _minStageTime = 0.05f * 60;
    //protected float _maxStageTime = 0.1f * 60;
    //protected float _minStageTime = 1 * 60;
    //protected float _maxStageTime = 4 * 60;
    protected float _minStageTime = 8 * 60;
    protected float _maxStageTime = 12 * 60;
    public float _spendTime = 0f;

    private bool _isClear = false;
    public bool _isStart = false;
    private bool _isGameOver = false;

    public float CurrentLapTime
    {
        get
        {
            return _spendTime;
        }
    }

    protected virtual void Start()
    {
        _enemiesSpawner = GetComponent<EnemiesSpawner>();
        _playerSpawner = GetComponent<PlayerSpawner>();
        _potionSpawner = GetComponent<PotionSpawner>();
        _weaponsSpawner = GetComponent<WeaponsSpawner>();

        if (MapManager.Instance != null)
        {
            _landBounds = MapManager.Instance.LandBounds;
        }

        _playerSpawner.MakePlayer();
        _potionSpawner.MakePotions();
    }

    public virtual void PlayGame()
    {
        StartStage();
        PlayGameStartMusic();
    }

    private void PlayGameStartMusic()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.GameStartMusic();
        }
    }



    protected virtual void StartStage()
    {
        _lapTime = Random.Range(_minStageTime, _maxStageTime);

        _isStart = true;
        _isClear = false;
        _spendTime = _lapTime;
        _isGameOver = false;

        PlayGameStartMusic();
    }


    protected virtual bool CheckClear()
    {
        return true;
    }

    protected virtual void EndStage()
    {
        _isStart = false;
        bool isClear = CheckClear();

        if(isClear && GameMain.Instance != null)
        {
            UIManager.Instance.ShowGameClearText();
            Invoke("DelayedStageClear", 3f);
        }
        else
        {
            UIManager.Instance.ShowGameOverText();
            Invoke("DelayedStageGameOver", 3f);
        }
    }

    protected virtual void Update()
    {
        if (_isStart)
        {
            _spendTime -= Time.deltaTime;
            //Debug.Log($"½Ã°£ = {_spendTime}");
            if (_spendTime <= 0)
            {
                _spendTime = 0;
                EndStage();
            }
        }

        if (!_isGameOver && GameObject.FindGameObjectsWithTag("Player") == null)
        {
            _isGameOver = true;
            Invoke("DelayedStageGameOver", 3f);
        }
    }

    public void DelayedStageClear()
    {
        UIManager.Instance.HideGameClearText();
        GameMain.Instance.StageClear();
    }

    public void DelayedStageGameOver()
    {
        UIManager.Instance.HideGameOverText();
        GameMain.Instance.GameOver();
    }

    public virtual void Do() { }
}
