using UnityEngine;
using UnityEngine.UI;

public class ProcessView : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    public void OnClickBackButton()
    {
        DescriptionView parentView = GetComponentInParent<DescriptionView>();
        if (parentView != null)
        {
            parentView.OnClickDescriptionBackButton();
        }
        else
        {
            UIManager.Instance.PushBackButton();
        }
    }
}
