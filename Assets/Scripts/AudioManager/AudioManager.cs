using System;
using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    Bob,
    Penguin,
    Music,
    Environment,
    Other,
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
    private AudioSource[] sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = new AudioSource[4];

        for (int i = 0; i < 4; i++)
        {
            sfxSource[i] = gameObject.AddComponent<AudioSource>();
            sfxSource[i].loop = false;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

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

    public static void PlaySound(ESoundType soundSourceType, string name, bool randomizePitch, float pitch = 1f, float volume = 1f, int channel = 0)
    {
        if (Instance == null)
        {
            Debug.LogWarning("AudioManager instance is null.");
            return;
        }

        var group = Instance.audioGroups.Find(g => g.soundType == soundSourceType);

        if (group == null)
        {
            Debug.LogError($"SoundType: {soundSourceType} does not exist!");
            return;
        }

        var clip = group.audioClips.Find(c => c.name == name);

        if (clip == null)
        {
            Debug.LogError($"Sound name: {name} doesn't exist in {group}");
            return;
        }

        if (randomizePitch)
        {
            Instance.sfxSource[channel].pitch = UnityEngine.Random.Range(1f, 2f);
        }
        else
        {
            Instance.sfxSource[channel].pitch = pitch;
        }

        Instance.sfxSource[channel].PlayOneShot(clip.clip, volume);
        Instance.sfxSource[channel].pitch = 1f;
    }

    public static void PlayMusic(ESoundType soundSourceType, string name, float volume = 1f)
    {
        if (Instance == null)
        {
            Debug.LogWarning("AudioManager instance is null.");
            return;
        }

        var group = Instance.audioGroups.Find(g => g.soundType == soundSourceType);

        if (group == null)
        {
            Debug.LogError($"SoundType: {soundSourceType} does not exist!");
            return;
        }

        var clip = group.audioClips.Find(c => c.name == name);

        if (clip == null)
        {
            Debug.LogError($"Sound name: {name} doesn't exist in {group}");
            return;
        }

        if (Instance.musicSource.isPlaying)
        {
            Instance.musicSource.Stop();
        }

        Instance.musicSource.clip = clip.clip;
        Instance.musicSource.volume = volume;
        Instance.musicSource.Play();
    }

    public static void StopMusic()
    {
        Instance.musicSource.Stop();
    }

    public static void StopSound(int channel = 0)
    {
        Instance.sfxSource[channel].Stop();
    }
}
