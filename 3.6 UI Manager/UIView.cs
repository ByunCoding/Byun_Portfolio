using UnityEngine;

public class UIView : MonoBehaviour
{
    public virtual void UnShow()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }
}
