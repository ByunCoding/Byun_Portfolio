using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static GameMain Instance { set; get; }

    [SerializeField] private Stage[] _stagePrefabs;

    private int _currentStageIndex = 0;
    public Stage _currentStage;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {

    }

    public void StartGame()
    {
        _currentStageIndex = 0;
        PlayingStage(_currentStageIndex);
    }

    public void StageClear()
    {
        _currentStageIndex++;

        if (_currentStageIndex >= _stagePrefabs.Length)
        {
            _currentStageIndex = 0;
        }

        PlayingStage(_currentStageIndex);
    }

    private void PlayingStage(int index)
    {
        if (_currentStage != null)
        {
            Destroy(_currentStage.gameObject);
        }

        DestroyGameObjects();

        if (MapManager.Instance != null)
        {
            MapManager.Instance.SelectRandomMap();
        }

        if (index < _stagePrefabs.Length)
        {
            _currentStage = Instantiate(_stagePrefabs[index]);
            _currentStage.PlayGame();

            if(StageManager.Instance != null)
            {
                StageManager.Instance.CurrentStage = _currentStage;
            }
        }

        if(UIManager.Instance != null)
        {
            UIManager.Instance.ShowMissionPopUpView();
        }
    }

    public void GameOver()
    {
        _currentStageIndex = 0;
        PlayingStage(_currentStageIndex);
    }

    private void DestroyGameObjects()
    {
        GameObject playedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (playedPlayer != null)
        {
            Destroy(playedPlayer);
        }

        GameObject[] playedPotions = GameObject.FindGameObjectsWithTag("Potion");
        foreach(GameObject potions in playedPotions)
        {
            Destroy(potions);
        }

        GameObject[] playedSwords = GameObject.FindGameObjectsWithTag("Sword");
        foreach (GameObject swords in playedSwords)
        {
            Destroy(swords);
        }

        GameObject[] playedRifles = GameObject.FindGameObjectsWithTag("Rifle");
        foreach (GameObject rifles in playedRifles)
        {
            Destroy(rifles);
        }

        GameObject[] playedGolems = GameObject.FindGameObjectsWithTag("Golem");
        foreach (GameObject golems in playedGolems)
        {
            Destroy(golems);
        }

        GameObject[] playedZombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject zombies in playedZombies)
        {
            Destroy(zombies);
        }

        GameObject dropZone = GameObject.Find("DropZone");
        if (dropZone != null)
        {
            Destroy(dropZone);
        }
    }
}
