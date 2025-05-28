using System;
using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    Bob,
    Penguin,
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Serializable]
    public class ClipEntry
    {
        public string name;
        public AudioClip clip;
    }

    [Serializable]
    public class NameOfAudioClip
    {
        public ESoundType soundType;
        [SerializeField] private List<ClipEntry> audioClips;
    }

    [SerializeField] private List<NameOfAudioClip> audioGroups;

    private Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

/*        foreach (var audioClip in audioClips)
        {
            if (!sounds.ContainsKey(audioClip.name))
            {
                sounds.Add(audioClip.name, audioClip.clip);
            }
            else
            {
                Debug.LogError($"Duplicated clip {audioClip.name}");
            }
        }*/
    }

    public static void PlaySound(string name, float volume = 1f)
    {
        if (Instance == null)
        {
            return;
        }

        if (Instance.sounds.TryGetValue(name, out var clip))
        {
            Instance.audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogError($"Sound: {name} not found!");
        }
    }
}
