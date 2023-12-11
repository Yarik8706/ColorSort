using UnityEngine;

public class ApplicationControl : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 2;
    }
}