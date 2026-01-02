using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsController : MonoBehaviour
{
    public static SoundsController Instance;

    [Header("Sound Clips Settings")]
    public AudioClip[] Guns;
    public AudioClip[] Instruments;
    public AudioClip[] Looting;
    public AudioClip[] Environment;
    public AudioClip[] Inventory;
    public AudioClip[] Player;
    public AudioClip[] Builds;
    public AudioClip[] Enemies;
    public AudioClip[] Menu;
    public AudioClip[] Musics;

    private AudioSource audioSource;

    private AudioSource playerAudioSource;

    [Header("References")]
    public GameObject playerPosition;

    public float soundsDistance;
    public float currentVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = soundsDistance;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        //Player
        if (playerPosition != null)
        {
            playerAudioSource = playerPosition.GetComponent<AudioSource>();
            if (playerAudioSource == null)
                playerAudioSource = playerPosition.AddComponent<AudioSource>();

            playerAudioSource.spatialBlend = 1f;
            playerAudioSource.minDistance = 1f;
            playerAudioSource.maxDistance = soundsDistance;
            playerAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }
    }

    private void Update()
    {
        currentVolume = Settings.instance.Volume;

        playerAudioSource.volume = currentVolume;
    }

    public void PlayGun(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Guns[index], soundPosition, currentVolume);
    }

    public void PlayEnvironment(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Environment[index], soundPosition, currentVolume);
    }

    public void PlayLooting(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Looting[index], soundPosition, currentVolume);
    }

    public void PlayInstruments(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Instruments[index], soundPosition, currentVolume);
    }

    public void PlayInventory(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Inventory[index], soundPosition, currentVolume);
    }

    public void PlayMusic(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Musics[index], soundPosition, currentVolume);
    }

    public void PlayPlayer(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Player[index], soundPosition, currentVolume);
    }

    public void PlayBuilds(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Builds[index], soundPosition, currentVolume);
    }

    public void PlayEnemies(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Enemies[index], soundPosition, currentVolume);
    }
    
    public void PlayMenu(int index, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(Menu[index], soundPosition, currentVolume);
    }

    public void PlayFootstep(int index, bool loop = false, float pitch = 1f)
    {
        if (playerAudioSource != null && index < Player.Length)
        {
            playerAudioSource.clip = Player[index];
            playerAudioSource.loop = loop;
            playerAudioSource.pitch = pitch;
            if (!playerAudioSource.isPlaying)
                playerAudioSource.Play();
        }
    }

    public void StopFootstep()
    {
        if (playerAudioSource != null && playerAudioSource.isPlaying)
        {
            playerAudioSource.Stop();
            playerAudioSource.loop = false;
        }
    }
}