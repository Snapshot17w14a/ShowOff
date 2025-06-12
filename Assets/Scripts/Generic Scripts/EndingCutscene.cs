using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class EndingCutscene : MonoBehaviour
{
    private GameObject[] curPlayers;
    //[SerializeField] private Transform[] jumpToPositions;
    [SerializeField] private Transform jumpToPos;
    [SerializeField] private Transform minecartPos;
    [SerializeField] private Transform minecartWalkToPos;
    [SerializeField] private Transform minecartPosOutisde;
    [SerializeField] private Transform minecartPosInside;
    [SerializeField] private float playerMoveSpeed = 1;
    [SerializeField] private float minecartMoveSpeed = 5;
    [SerializeField] private float cameraSpeed = 0.5f;
    public UnityEvent OnLeftCave;
    private int playersInMinecart;
    private bool isLoading;
    
    [SerializeField] private CinemachineSplineDolly cinemachineSplineDolly;
    SplineAutoDolly.FixedSpeed autodolly;

    private void Start()
    {
        curPlayers = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(MoveMinecartInside());
        autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
    }

    private void Update()
    {
        if (cinemachineSplineDolly.CameraPosition >= 2f && !isLoading)
        {
            isLoading = true;
            OnLeftCave.Invoke();
        }
    }
    private IEnumerator MoveMinecartInside()
    {
        Debug.Log("Minecart Moving");
        float timer = 0f;
        while (timer < minecartMoveSpeed)
        {
            float t = timer / minecartMoveSpeed;
            minecartPos.position = Vector3.Lerp(minecartPosOutisde.position, minecartPosInside.position, t);
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(RunPlayersOneByOne());
    }

    private IEnumerator RunPlayersOneByOne()
    {
        Debug.Log("Jump Now;");
        foreach (GameObject player in curPlayers)
        {
            player.transform.LookAt(minecartPos.position);
            float timer = 0f;
            Vector3 playerOriginalPos = player.transform.position;

            while (timer < playerMoveSpeed)
            {
                float t = timer / playerMoveSpeed;
                player.transform.position = Vector3.Lerp(playerOriginalPos, minecartWalkToPos.position, t);
                timer += Time.deltaTime;
                yield return null;
            }

            JumpInMinecart(player);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void JumpInMinecart(GameObject curPlayer)
    {
        curPlayer.transform.SetParent(minecartPos);
        Vector3 yeet = PathCalculator.CalculateRequiredVelocity(curPlayer.transform.position, jumpToPos.position, 0.65f);
        curPlayer.GetComponent<Rigidbody>().AddForce(yeet * 1.6f, ForceMode.Impulse);
        playersInMinecart++;
        Debug.Log("Player in Minecart");

        if (playersInMinecart == curPlayers.Length)
        {
            StartCoroutine(MoveMinecartOutside());
        }
    }

    private IEnumerator MoveMinecartOutside()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("Minecart Moving");
        autodolly.Speed = cameraSpeed;
        float timer = 0f;

        while (timer < minecartMoveSpeed)
        {
            float t = timer / minecartMoveSpeed;
            minecartPos.position = Vector3.Lerp(minecartPosInside.position, minecartPosOutisde.position, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
