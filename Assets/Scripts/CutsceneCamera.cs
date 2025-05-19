using Unity.Cinemachine;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float delayCameraSwap = 0.5f;
    [SerializeField] private float transitionDuration = 2.0f;

    [Header("Please Assign")]
    [SerializeField] private CinemachineSplineDolly cinemachineSplineDolly;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera cutsceneCamera;

    private bool isTransitioning = false;
    private float transitionProgress = 0.0f;
    private bool finishedCutscene;
    private Vector3 startPos;
    private Quaternion startRot;
    private int knots;
    private void Start()
    {
        Debug.Log(cinemachineSplineDolly.Spline.Splines[0].Count);
        knots = cinemachineSplineDolly.Spline.Splines[0].Count;
    }

    private void Update()
    {
        if (cinemachineSplineDolly.CameraPosition >= (knots - 1) && !finishedCutscene) 
        {
            finishedCutscene = true;
            Invoke("CutsceneFinished", delayCameraSwap);
        }

        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            cutsceneCamera.transform.position = Vector3.Lerp(startPos, MainCamera.transform.position, transitionProgress);
            cutsceneCamera.transform.rotation = Quaternion.Lerp(startRot, MainCamera.transform.rotation, transitionProgress);

            if (transitionProgress >= 1)
            {
                GameReadyToStart();
            }
        }

        //Skips Cutscene (Can assign it to other keybinds, just call this function)
        if (Input.GetKey(KeyCode.Space)) 
        {
            CutsceneFinished();
        }
    }

    public void CutsceneFinished()
    {
        Debug.Log("Cutscene Finished");
        cutsceneCamera.gameObject.GetComponent<CinemachineBrain>().enabled = false;

        startPos = cutsceneCamera.transform.position;
        startRot = cutsceneCamera.transform.rotation;

        isTransitioning = true;
        transitionProgress = 0.0f;
    }

    public void GameReadyToStart()
    {
        Debug.Log("Camera Ready to Play");
        isTransitioning = false;
        cutsceneCamera.enabled = false;
        MainCamera.enabled = true;
    }
}
