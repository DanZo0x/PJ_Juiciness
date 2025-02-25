using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Space]
    [Header("References")]
    public static AudioManager Instance;
    [SerializeField] private GameObject musicSourceObj;
    [SerializeField] private GameObject sfxSourceObj;

    [Space]
    [Header("Music Audio Clips")]
    [SerializeField] private AudioClip debugMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        musicSourceObj = GameObject.FindGameObjectWithTag("MusicSource");
        sfxSourceObj = GameObject.FindGameObjectWithTag("SfxSource");
        musicAudioSource = musicSourceObj.GetComponent<AudioSource>();
        sfxAudioSource = sfxSourceObj.GetComponent<AudioSource>();
    }
    
    private void Start()
    {
        musicAudioSource.clip = debugMusic;
        musicAudioSource.Play();
    }

    public void PlaySFX(AudioClip sfxToPlay)
    {
        sfxAudioSource.PlayOneShot(sfxToPlay);
    }
}