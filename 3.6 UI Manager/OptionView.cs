using UnityEngine;
using UnityEngine.UI;

public class OptionView : UIView
{
    public Slider _backgroundMusicSlider;
    public Slider _gameStartMusicSlider;
    public Slider _swordHitSoundSlider;

    //public Button _graphicsIncreaseButton;
    //public Button _graphicsDecreaseButton;
    //public Text _graphicsOptionText;

    private int _graphicsSettingIndex = 0;

    private void Start()
    {
        _backgroundMusicSlider.value = 50f;
        _gameStartMusicSlider.value = 50f;
        _swordHitSoundSlider.value = 50f;

        _backgroundMusicSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChange);
        _gameStartMusicSlider.onValueChanged.AddListener(OnGameStartMusicVolumeChange);
        _swordHitSoundSlider.onValueChanged.AddListener(OnSwordHitSoundVolumeChange);
    }

    public void OnBackgroundMusicVolumeChange(float value)
    {
        AudioManager.Instance.BackgroundMusicVolume(value / 100f);
    }

    public void OnGameStartMusicVolumeChange(float value)
    {
        AudioManager.Instance.GameStartMusicVolume(value / 100f);
    }

    public void OnSwordHitSoundVolumeChange(float value)
    {
        AudioManager.Instance.SwordHitSoundVolume(value / 100f);
    }

    public void OnRifleShootingVolumeChange(float value)
    {
        AudioManager.Instance.RifleShootingSoundVolume(value / 100f);
    }

    public void OnClickBackButton()
    {
        UIManager.Instance.PushBackButton();
    }
}
