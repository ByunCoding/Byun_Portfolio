using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text buttonText;
    private Color normalColor = new Color(255f / 255f, 191f / 255f, 0f / 255f);
    private Color changeColor = Color.red;

    void Start()
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = changeColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }

    private void OnDisable()
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }
}