using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneCamera : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float delayCameraSwap = 0.5f;
    [SerializeField] private float transitionDuration = 2.0f;

    [Header("Please Assign")]
    [SerializeField] private CinemachineSplineDolly cinemachineSplineDolly;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private GameObject penguinExample;

    private bool isTransitioning = false;
    private float transitionProgress = 0.0f;
    private bool finishedCutscene;
    private Vector3 startPos;
    private Quaternion startRot;
    private int knots;
    private bool cutsceneSkipped;
    private bool penguinWalking;
    private Vector3 penguinStartPos;
    private Vector3 penguinEndPos;
    private float walkPogression;

    public UnityEvent OnCameraReady;

    private void Start()
    {
        Debug.Log(cinemachineSplineDolly.Spline.Splines[0].Count);
        knots = cinemachineSplineDolly.Spline.Splines[0].Count;
    }

    private void Update()
    {
        PenguinWalking();

        if (cinemachineSplineDolly.CameraPosition >= (knots - 1) && !finishedCutscene) 
        {
            finishedCutscene = true;
            Invoke("CutsceneFinished", delayCameraSwap);
        }

        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            cutsceneCamera.fieldOfView = Mathf.Lerp(60, 27, transitionProgress);
            cutsceneCamera.transform.position = Vector3.Lerp(startPos, MainCamera.transform.position, transitionProgress);
            cutsceneCamera.transform.rotation = Quaternion.Lerp(startRot, MainCamera.transform.rotation, transitionProgress);

            if (transitionProgress >= 1)
            {
                GameReadyToStart();
            }
        }

        //Skips Cutscene (Can assign it to other keybinds, just call this function)
        if (Input.GetKey(KeyCode.Space) && !cutsceneSkipped) 
        {
            cutsceneSkipped = true;
            CutsceneFinished();
        }
    }

    public void CutsceneFinished()
    {
        Debug.Log("Cutscene Finished");
        cutsceneCamera.gameObject.GetComponent<CinemachineBrain>().enabled = false;
        cinemachineSplineDolly.GetComponent<CinemachineCamera>().enabled = false;

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

        OnCameraReady?.Invoke();
    }

    private void PenguinWalking()
    {
        if (cinemachineSplineDolly.CameraPosition >= 2f)
        {
            penguinExample.SetActive(true);
        }

        if (cinemachineSplineDolly.CameraPosition >= 2.5f && cinemachineSplineDolly.CameraPosition <= 3.9f && !penguinWalking)
        {
            penguinWalking = true;
            penguinStartPos = penguinExample.transform.position;
            penguinEndPos = new Vector3(penguinStartPos.x + 1.5f, penguinStartPos.y, penguinStartPos.z);
        }
        if (penguinWalking)
        {
            walkPogression += Time.deltaTime / 1;
            penguinExample.transform.position = Vector3.Lerp(penguinStartPos, penguinEndPos, walkPogression);
        }

        if (cinemachineSplineDolly.CameraPosition >= 6 && penguinWalking)
        {
            penguinExample.SetActive(false);
        }
    }
}
