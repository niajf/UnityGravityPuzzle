using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.GameOver();
        }
    }
}