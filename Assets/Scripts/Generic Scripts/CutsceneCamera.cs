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
    [SerializeField] private CutsceneAnimation penguinAnimScript;

    private bool isTransitioning = false;
    private float transitionProgress = 0.0f;
    private bool finishedCutscene;
    private Vector3 startPos;
    private Quaternion startRot;
    private int knots;
    private bool cutsceneSkipped;

    public UnityEvent OnCameraReady;

    private void Start()
    {
        //Debug.Log(cinemachineSplineDolly.Spline.Splines[0].Count);
        knots = cinemachineSplineDolly.Spline.Splines[0].Count;
        AudioManager.PlayMusic(ESoundType.Music, "MUSIC_GAME", 0.0f); //Just stop the music here when you start
    }

    private void Update()
    {
        PenguinWalking();

        if (cinemachineSplineDolly.CameraPosition >= (knots - 1) && !finishedCutscene)
        {
            finishedCutscene = true;
            Invoke(nameof(CutsceneFinished), delayCameraSwap);
        }

        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            cutsceneCamera.fieldOfView = Mathf.Lerp(60, 27, transitionProgress);
            cutsceneCamera.transform.SetPositionAndRotation(Vector3.Lerp(startPos, MainCamera.transform.position, transitionProgress),
                                                            Quaternion.Lerp(startRot, MainCamera.transform.rotation, transitionProgress));
            if (transitionProgress >= 1)
            {
                GameReadyToStart();
            }
        }

        //Skips Cutscene (Can assign it to other keybinds, just call this function)
        if (Input.GetKey(KeyCode.Space) && !cutsceneSkipped)
        {
            OnGrab();
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
        AudioManager.PlayMusic(ESoundType.Music, "MUSIC_GAME", 0.5f);
    }

    private void PenguinWalking()
    {

        if (cinemachineSplineDolly.CameraPosition >= 2f && !penguinAnimScript.IsAnimating)
        {
            penguinAnimScript.gameObject.SetActive(true);
            penguinAnimScript.StartAnimation();
        }

        if (cinemachineSplineDolly.CameraPosition >= 6)
        {
            penguinAnimScript.gameObject.SetActive(false);
        }
    }

    private void OnGrab()
    {
        if (!cutsceneSkipped)
        {
            cutsceneSkipped = true;
            CutsceneFinished();
        }
    }
}
