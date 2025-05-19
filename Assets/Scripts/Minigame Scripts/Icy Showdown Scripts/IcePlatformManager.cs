using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

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

    public int BrittlePlatformCount
    {
        get
        {
            int count = 0;
            foreach (var platform in icePlatforms) if (platform.IsBrittle) count++;
            return count;
        }
    }

    public IcePlatform[] BrittlePlatforms
    {
        get
        {
            return icePlatforms.Where(platform => platform.IsBrittle).ToArray();
        }
    }

    public IcePlatform[] NonBrokenPlatforms
    {
        get
        {
            return icePlatforms.Where(platform => !platform.IsBroken).ToArray();
        }
    }

    public IcePlatform GetRandomPlatform => icePlatforms[Random.Range(0, icePlatforms.Length)];

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
            icePlatforms[Random.Range(0, icePlatforms.Length)].SetBrittle();
            SelectWaitTime();
        }
    }

    private void SelectWaitTime()
    {
        timer = 0;
        currentWaitTime = Random.Range(minSeconds, maxSeconds);
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

    public IcePlatform[] SelectUniquePlatforms(int count)
    {
        var platforms = NonBrokenPlatforms;

        if (count == 0 || count > platforms.Length) return null;

        List<int> usedIndicies = new();
        IcePlatform[] selectedPlatforms = new IcePlatform[count];

        for (int i = 0; i < count; i++)
        {
            int index;
            do index = Random.Range(0, platforms.Length);
            while (usedIndicies.Contains(index));

            selectedPlatforms[i] = platforms[index];
            usedIndicies.Add(index);
        }

        return selectedPlatforms;
    }
}
