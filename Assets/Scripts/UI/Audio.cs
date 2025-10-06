using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour, IPointerEnterHandler
{
    [Header("Sound Settings")]
    [Tooltip("Аудиоклип, который будет проигрываться при наведении курсора")]
    public AudioClip hoverSound;

    [Tooltip("Громкость звука")]
    [Range(0f, 1f)]
    public float volume = 0.8f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, volume);
        }
    }
}
