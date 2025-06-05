using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class IcePlatformManager : MonoBehaviour
{
    [SerializeField] private float minSeconds;
    [SerializeField] private float maxSeconds;

    private float currentWaitTime = 0;
    private float timer = 0;

    private IcePlatform[] icePlatforms;

    public static IcePlatformManager Instance { get; private set; }

    public bool DoBrittlePlatformsExist => SelectPlatforms(platform => platform.IsBrittle).Length != 0;

    public int BrittlePlatformCount => SelectPlatforms(platform => platform.IsBrittle).Length;

    public IcePlatform GetRandomPlatform => icePlatforms[Random.Range(0, icePlatforms.Length)];

    private void Awake()
    {
        if (Instance != null) DestroyImmediate(gameObject);
        Instance = this;

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
    public void ExecuteForEachPlatform(Action<IcePlatform> action)
    {
        foreach (var platform in icePlatforms) action(platform);
    }

    /// <summary>
    /// Select a platform from the array of platforms using the predicate
    /// </summary>
    /// <param name="predicate">The predicate function to chose which platforms to select</param>
    public IcePlatform[] SelectPlatforms(Func<IcePlatform, bool> predicate) => icePlatforms.Where(platform => predicate(platform)).ToArray();

    public IcePlatform[] SelectUniquePlatforms(int count)
    {
        var platforms = SelectPlatforms(platform => !platform.IsBroken);

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
