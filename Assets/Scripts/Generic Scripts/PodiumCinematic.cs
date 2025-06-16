using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PodiumCinematic : MonoBehaviour
{
    private CinemachineSplineDolly cinemachineSplineDolly;
    private bool isCinematicFinished;

    public UnityEvent OnCameraReady;

    private void Start()
    {
        cinemachineSplineDolly = GetComponent<CinemachineSplineDolly>();
    }

    private void Update()
    {
        if (cinemachineSplineDolly.CameraPosition >= 3 && !isCinematicFinished)
        {
            Debug.Log("Podium Cutscene Finished");
            isCinematicFinished = true;
            OnCameraReady.Invoke();
        }
    }
}
