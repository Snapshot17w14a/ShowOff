using UnityEngine;
using UnityEngine.Events;

public class HubManager : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
