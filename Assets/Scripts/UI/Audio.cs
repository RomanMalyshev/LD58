using UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour, IPointerDownHandler
{
    [Header("Sound Settings")] [Tooltip("Аудиоклип, который будет проигрываться при наведении курсора")]
    public AudioClip hoverSound;

    [Tooltip("Громкость звука")] [Range(0f, 1f)]
    public float volume = 0.8f;

    private AudioSource audioSource;
    [SerializeField] private Hud trigger;

    void Start()
    {
        //trigger = GetComponentInChildren<Hud>();
        if (trigger != null)
            trigger.OnMassageChange += PlaySound;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }


    private void PlaySound()
    {
        audioSource.PlayOneShot(hoverSound, volume);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ((hoverSound != null) && (trigger == null))
        {
//            Debug.Log("Навелось");
            PlaySound();
        }
    }
}