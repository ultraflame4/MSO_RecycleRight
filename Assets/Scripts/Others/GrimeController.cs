using UnityEngine;

public class GrimeController : MonoBehaviour
{
    [Tooltip("The sprite renderer hosting the grime overlay texture and the grime overlay material")]
    public SpriteRenderer spriteRenderer;
    public Material grimeMaterial;
    [SerializeField, Tooltip("The amount of grime / dirt to show. Range between 0-1"), Range(0, 1)]
    private float _grimeAmount;

    /// <summary>
    /// The amount of grime / dirt to show. Ranges between 0-1
    /// </summary>
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

    private void Awake()
    {
        InitMaterial();
    }
    private void Start()
    {
        UpdateMaterial();
    }

    private void InitMaterial()
    {
        material =  new Material(grimeMaterial);
        spriteRenderer.material = material;
    }
    private void UpdateMaterial()
    {
        if (material == null)
        {
            InitMaterial();
        }
        material.SetFloat("_percent", _grimeAmount);
        Debug.Log($"update mat, {_grimeAmount}");
        spriteRenderer.material = material;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateMaterial();
        }
    }

}