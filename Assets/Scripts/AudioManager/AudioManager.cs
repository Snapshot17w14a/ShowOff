using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        [SerializeField] public List<ClipEntry> audioClips;
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

        foreach (var audioClip in audioGroups)
        {
            foreach (var entry in audioClip.audioClips)
            {
                if (!sounds.ContainsKey(entry.name))
                {
                    sounds.Add(entry.name, entry.clip);
                }
            }
        }
    }

    public static void PlaySound(ESoundType soundSourceType, string name, float volume = 1f)
    {
        if (Instance == null)
        {
            Debug.LogWarning("AudioManager instance is null.");
            return;
        }

        var group = Instance.audioGroups.Find(g => g.soundType == soundSourceType);

        if(group == null )
        {
            Debug.LogError($"SoundType: {soundSourceType} does not exist!");
            return;
        }

        var clip = group.audioClips.Find(c => c.name == name);

        if( clip == null )
        {
            Debug.LogError($"Sound name: {name} doesn't exist in {group}");
            return;
        }

        Instance.audioSource.PlayOneShot(clip.clip, volume);
    }
}
