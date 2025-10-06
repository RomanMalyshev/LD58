using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectsController : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    [Tooltip("Volume, в котором хранятся эффекты камеры (виньетка, цветофильтр и т.д.)")]
    public Volume volume;

    [Header("Vignette Settings")]
    [Range(0f, 1f)] public float vignetteIntensity = 0.3f;

    [Header("Color Filter Settings")]
    [Tooltip("Цветовой фильтр, наложенный на изображение")]
    public Color colorFilter = Color.white;

    [Range(0f, 1f)] 
    public float colorFilterIntensity = 0.3f;

    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("❌ Volume не назначен на CameraEffectsController!");
            return;
        }
       
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);

        if (vignette == null)
        {
            vignette = volume.profile.Add<Vignette>(true);
        }
        if (colorAdjustments == null)
        {
            colorAdjustments = volume.profile.Add<ColorAdjustments>(true);
        }

        vignette.active = true;
        colorAdjustments.active = true;
    }

    void Update()
    {
        if (vignette != null)
        {
            vignette.intensity.value = vignetteIntensity;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, colorFilter, colorFilterIntensity);
        }
    }
}
