using Unity.VisualScripting;
using UnityEngine;

public class DropZoneTrigger : MonoBehaviour
{
    public Stage_3 stage_3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword") || other.CompareTag("Rifle"))
        {
            stage_3._isDropZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sword") || other.CompareTag("Rifle"))
        {
            stage_3._isDropZone = false;
        }
    }
}
