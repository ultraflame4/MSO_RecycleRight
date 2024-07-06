using UnityEngine;

public class GrimeController : MonoBehaviour
{
    [Tooltip("The sprite renderer hosting the grime overlay texture and the grime overlay material")]
    public SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("The amount of grime / dirt to show. Range between 0-1"), Range(0, 1)]
    private float _grimeAmount;

    public float GrimeAmount
    {
        get => _grimeAmount;
        set
        {
            _grimeAmount = Mathf.Clamp01(value);
            UpdateMaterial();
        }
    }

    private Material material;
    private void Start()
    {
        InitMaterial();
    }

    private void InitMaterial()
    {
        material = spriteRenderer.material;
        spriteRenderer.material = material;
    }
    private void UpdateMaterial()
    {
        if (material == null)
        {
            InitMaterial();
        }
        material.SetFloat("_percent", _grimeAmount);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateMaterial();
        }
    }

}