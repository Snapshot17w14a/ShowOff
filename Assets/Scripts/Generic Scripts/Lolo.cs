using UnityEngine;
using System;

public class Lolo : MonoBehaviour
{
    [SerializeField] private Transform lookTarget;

    public bool lookAtTarget;

    public Animator GetLoloAnimator => animator;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtTarget) transform.LookAt(lookTarget);
    }

    public void FlyAroundPlayer(MinigamePlayer player, Action callback)
    {
        lookAtTarget = false;

        Vector3 startingPos = transform.position;
        Vector3 playerOffset = new(0, 0.5f, 0.5f);
        Vector3 playerPos = player.transform.position;
        Scheduler.Instance.Lerp(t =>
        {
            transform.position = Vector3.Lerp(startingPos, playerPos + playerOffset, t);
            transform.LookAt(Vector3.Lerp(lookTarget.position, playerPos + new Vector3(0, 0.5f, 0), t));
        }, 2f, () =>
        {
            animator.SetTrigger("TurnGold");
            Scheduler.Instance.Lerp(t =>
            {
                t = t * t * (3 - 2 * t);
                Debug.Log(new Vector3(0, 0.5f * Mathf.Sin(Mathf.PI * (t * 4)), 0.5f));
                transform.position = player.transform.position + (Quaternion.Euler(new(0, -360f * t, 0)) * new Vector3(0, 0.5f + 0.1f * Mathf.Sin(Mathf.PI * (t * 4)), 0.5f));
                transform.LookAt(playerPos + new Vector3(0, 0.5f, 0));
            }, 4f, callback);
        });
    }

    public void SetLookTarget(Transform transform) => lookTarget = transform;
}
