using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    [SerializeField] private Animator introBobAnimator;

    [SerializeField] private InputAction skipInput;

    private bool finishedCutscene;
    private bool startedSlowingDown;
    private Vector3 startPos;
    private Quaternion startRot;
    private int knots;
    private bool cutsceneSkipped;

    public UnityEvent OnCameraReady;

    private void Start()
    {
        //Debug.Log(cinemachineSplineDolly.Spline.Splines[0].Count);
        knots = cinemachineSplineDolly.Spline.Splines[0].Count;
        AudioManager.PlayMusic(ESoundType.Music, "Icy_Showdown_Intro", 0.6f);

        skipInput.performed += SkipCutscene;
        skipInput.Enable();
    }

    private void Update()
    {
        PenguinWalking();
       
        if (cinemachineSplineDolly.CameraPosition >= (knots - 3) && !startedSlowingDown)
        {
            BobReveal();      
        }

        if (cinemachineSplineDolly.CameraPosition >= (knots - 1) && !finishedCutscene)
        {
            finishedCutscene = true;
            Invoke(nameof(CutsceneFinished), delayCameraSwap);
        }
    }

    private void BobReveal()
    {
        startedSlowingDown = true;
        introBobAnimator.SetTrigger("StartIntro");
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        autodolly.Speed = 0.35f;
    }

    public void CutsceneFinished()
    {
        Debug.Log("Cutscene Finished");
        cutsceneCamera.gameObject.GetComponent<CinemachineBrain>().enabled = false;
        cinemachineSplineDolly.GetComponent<CinemachineCamera>().enabled = false;

        startPos = cutsceneCamera.transform.position;
        startRot = cutsceneCamera.transform.rotation;

        Scheduler.Instance.Lerp(LerpCamera, transitionDuration, GameReadyToStart);
        AudioManager.PlayMusic(ESoundType.Music, "Icy_Showdown", 0.6f);
    }

    private void LerpCamera(float t)
    {
        cutsceneCamera.fieldOfView = Mathf.Lerp(60, 27, t);
        cutsceneCamera.transform.SetPositionAndRotation(Vector3.Lerp(startPos, MainCamera.transform.position, t),
                                                        Quaternion.Lerp(startRot, MainCamera.transform.rotation, t));
    }

    public void GameReadyToStart()
    {
        Debug.Log("Camera Ready to Play");
        cutsceneCamera.enabled = false;
        MainCamera.enabled = true;

        OnCameraReady?.Invoke();
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

    private void SkipCutscene(InputAction.CallbackContext ctx)
    {
        if (!cutsceneSkipped)
        {
            introBobAnimator.SetFloat("SpeedMult", 5f / transitionDuration);
            introBobAnimator.SetTrigger("StartIntro");
            cutsceneSkipped = true;
            CutsceneFinished();
        }
    }

    private void OnDestroy()
    {
        skipInput.performed -= SkipCutscene;
    }
}
