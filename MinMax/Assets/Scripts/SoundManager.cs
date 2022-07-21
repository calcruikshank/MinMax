using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton;

    public AudioClip[] audioClips;
    public GameObject soundPrefab;
    public List<PlayerCursor> pcs;

    void Awake()
    {
        if (singleton is null)
        {
            singleton = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (audioClips.Length < 1) audioClips = Resources.LoadAll<AudioClip>("Sounds") as AudioClip[];
    }

    public void PlaySound(int soundIndex, float volume = 0.5f, float variance = 0.1f, float basePitch = 1f)
    {
        if (soundIndex < 0 || soundIndex > audioClips.Length) return;
        AudioClip foundClip = audioClips[soundIndex];
        if (foundClip is null) return;
        playSound(foundClip, volume, variance, basePitch);
    }

    public void PlaySound(string soundName, float volume = 0.5f, float variance = 0.1f, float basePitch = 1f)
    {
        if (string.IsNullOrEmpty(soundName)) return;
        AudioClip foundClip = null;
        foreach(AudioClip ac in audioClips)
        {
            if (ac.name == soundName) foundClip = ac;
        }
        if (foundClip is null) return;
        playSound(foundClip, volume, variance, basePitch);
    }

    private void playSound(AudioClip clip, float volume, float variance, float basePitch){
        GameObject newSound = Instantiate(soundPrefab);
        AudioSource newAC = newSound.GetComponent<AudioSource>();
        newAC.volume = volume;
        newAC.pitch = basePitch;
        newAC.pitch += Random.Range(-variance, variance);
        newAC.PlayOneShot(clip);
        Destroy(newSound, clip.length);
    }

    public void PlayRandomDieSound()
    {
        PlaySound(Mathf.FloorToInt(Random.Range(1, 3)), 0.5f, 0.5f);
    }

    public void PlayRandomStepSound()
    {
        PlaySound(Mathf.FloorToInt(Random.Range(7, 9)), 0.3f, 0.5f);
    }

    public void PlayRandomDieHitSound()
    {
        PlaySound(Mathf.FloorToInt(Random.Range(11, 15)), 0.4f, 0.5f);
    }
}
