using System;
using UnityEngine;

public class IcePlatformManager : MonoBehaviour
{
    [SerializeField] private float minSeconds;
    [SerializeField] private float maxSeconds;

    private float currentWaitTime = 0;
    private float timer = 0;

    private IcePlatform[] icePlatforms;

    public static IcePlatformManager Instance => _instance;
    private static IcePlatformManager _instance;

    public bool DoBrittlePlatformsExist
    {
        get
        {
            foreach (var platform in icePlatforms) if (platform.IsBrittle) return true;
            return false;
        }
    }

    public IcePlatform GetRandomPlatform => icePlatforms[UnityEngine.Random.Range(0, icePlatforms.Length)];

    private void Start()
    {
        if (_instance != null) DestroyImmediate(gameObject);
        _instance = this;

        icePlatforms = FindObjectsByType<IcePlatform>(FindObjectsSortMode.None);
        SelectWaitTime();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= currentWaitTime)
        {
            icePlatforms[UnityEngine.Random.Range(0, icePlatforms.Length)].SetBrittle();
            SelectWaitTime();
        }
    }

    private void SelectWaitTime()
    {
        timer = 0;
        currentWaitTime = UnityEngine.Random.Range(minSeconds, maxSeconds);
    }

    public void BreakBrittlePlatforms()
    {
        ExecuteForEachPlatform(platform => platform.BreakIfBrittle());
    }

    /// <summary>
    /// Pass in a function to be executed for each platform
    /// </summary>
    /// <param name="action"></param>
    public void ExecuteForEachPlatform(Action<IcePlatform> action)
    {
        foreach (var platform in icePlatforms) action(platform);
    }
}
