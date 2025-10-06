using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Assign these AudioSource components in the Inspector
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Music Settings")]
    // Configure clip on the AudioSource component itself
    [SerializeField] [Range(0f, 1f)] private float targetMusicVolume = 0.8f;
    [SerializeField] private float musicFadeInDuration = 2.0f;

    [Header("Ambient Sound Settings")]
    // Configure clip on the AudioSource component itself
    [SerializeField] [Range(0f, 1f)] private float targetAmbientVolume = 0.6f;
    [SerializeField] private float ambientFadeInDuration = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start and fade in music if source is assigned
        if (musicSource != null)
        {
            // Optional: Ensure loop is set if you want script control, 
            // otherwise configure loop on the AudioSource in Inspector.
            // musicSource.loop = true; 

            // Start fade-in coroutine
            StartCoroutine(FadeInAudioSource(musicSource, targetMusicVolume, musicFadeInDuration));
        }
        else
        {
            Debug.LogWarning("AudioManager: Music Source not assigned in the Inspector.", this);
        }

        // Start and fade in ambient sound if source is assigned
        if (ambientSource != null)
        {
            // Optional: Ensure loop is set
            // ambientSource.loop = true;

            // Start fade-in coroutine
            StartCoroutine(FadeInAudioSource(ambientSource, targetAmbientVolume, ambientFadeInDuration));
        }
        else
        {
            Debug.LogWarning("AudioManager: Ambient Source not assigned in the Inspector.", this);
        }
    }

    // Generic coroutine to fade in an AudioSource
    private IEnumerator FadeInAudioSource(AudioSource source, float targetVolume, float duration)
    {
        // Only start playing if it's not already playing (e.g., from Play On Awake)
        if (!source.isPlaying)
        {
            source.volume = 0f;
            source.Play();
        }
        // If already playing (Play On Awake), just fade from current volume
        else
        {
            // Optionally start fade from 0 even if playing? 
            // source.volume = 0f; 
        }
        
        float startVolume = source.volume; // Fade from current volume (useful if Play On Awake is true)
        float timer = 0f;
        
        if (duration <= 0f)
        {
             source.volume = targetVolume;
             yield break; 
        }
        
        while (timer < duration)
        {
            // Lerp from startVolume to targetVolume
            source.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null; 
        }

        source.volume = targetVolume;
    }

    // Update is called once per frame (kept for potential future use)
    void Update()
    {
        
    }
}
