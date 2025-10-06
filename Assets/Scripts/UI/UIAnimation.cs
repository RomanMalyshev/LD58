using UnityEngine;
using UnityEngine.EventSystems;

public class UIAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Wobble Settings")]
    [Tooltip("Амплитуда колебаний по X и Y (в пикселях)")]
    public Vector2 amplitude = new Vector2(2f, 2f);

    [Tooltip("Частота колебаний по X и Y (в герцах)")]
    public Vector2 frequency = new Vector2(2f, 1f);

    [Tooltip("Добавить лёгкое вращение для живости")]
    public bool useRotation = true;

    [Tooltip("Амплитуда вращения (в градусах)")]
    public float rotationAmplitude = 2f;

    [Header("Animation Settings")]
    [Tooltip("Скорость возврата к исходному положению/углу")]
    public float returnSpeed = 3f;

    private bool isHovered;
    private RectTransform rectTransform;
    private Vector3 startPos;
    private Quaternion startRot;
    private float time;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        startRot = rectTransform.localRotation;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (isHovered)
        {
            float offsetX = Mathf.Sin(time * frequency.x) * amplitude.x;
            float offsetY = Mathf.Sin(time * frequency.y + Mathf.PI / 2f) * amplitude.y;

            Vector3 targetPos = startPos + new Vector3(offsetX, offsetY, 0);
            rectTransform.anchoredPosition = targetPos;

            if (useRotation)
            {
                float angle = Mathf.Sin(time * (frequency.x + frequency.y) * 0.5f) * rotationAmplitude;
                rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, startPos, Time.deltaTime * returnSpeed);
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, startRot, Time.deltaTime * returnSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        time = 0f; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
