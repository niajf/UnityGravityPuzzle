using UnityEngine;

public class OutStage : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.GameOver();
        }
    }
}
