using System.Collections;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    [SerializeField] private GameObject movePoint;
    [SerializeField] private int maxGemIntake = 3;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float waitTime = 10f;
    [SerializeField] private GameObject goldPile;
    [SerializeField] private Vector3 pileEndPos;
    [SerializeField] private Vector3 pileEndScl;

    private Vector3 pileStartPos;
    private Vector3 pileStartScl;

    private BoxCollider boxCollider;

    private int currentGemAmount = 0;
    private bool isFull = false;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        startPosition = transform.position;
        endPosition = movePoint.transform.position;
        boxCollider = GetComponent<BoxCollider>();

        pileStartPos = goldPile.transform.localPosition;
        pileStartScl = goldPile.transform.localScale;
    }

    public void AddGem()
    {
        if (isFull || isMoving)
        {
            return;
        }

        currentGemAmount++;
        float gemLimit = (Mathf.CeilToInt((ServiceLocator.GetService<PlayerRegistry>().RegisteredPlayerCount + 1) / 2) + 1);

        //Change the gold pile height
        SetFill(currentGemAmount / gemLimit);

        Debug.Log($"Gem intake: {gemLimit}");

        if (currentGemAmount >= gemLimit)
        {
            isFull = true;
            StartCoroutine(MoveCart());
        }
    }

    private void SetFill(float t)
    {
        goldPile.transform.localPosition = Vector3.Lerp(pileStartPos, pileEndPos, t);
        goldPile.transform.localScale = Vector3.Lerp(pileStartScl, pileEndScl, t);
    }

    private IEnumerator MoveCart()
    {
        isMoving = true;
        boxCollider.enabled = false;

        while (Vector3.Distance(transform.position, endPosition) > 0.1f)
        {

            transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(waitTime);
        SetFill(0);

        while (Vector3.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        boxCollider.enabled = true;
        currentGemAmount = 0;
        isMoving = false;
        isFull = false;
    }
}
