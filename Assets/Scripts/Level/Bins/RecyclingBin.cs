using UnityEngine;

public class RecyclingBin : MonoBehaviour
{

    [Tooltip("How long it will take for the bin to become infested, once contaminated with food items.")]
    public float infestation_secs;

    [field: Header("Internal")]

    [field: SerializeField]
    public RecyclableType recyclableType { get; private set; }
    [field: SerializeField]
    public BinState binState { get; private set; }
    [field: SerializeField]
    public bool pending_infestation { get; private set; }
    [field: SerializeField]
    public float infestation_percent { get; private set; }

    public bool IsInfested => infestation_percent > 0 || binState == BinState.INFESTED;


    private void Update()
    {
        if (pending_infestation)
        {
            infestation_percent += Time.deltaTime / infestation_secs;
            if (infestation_percent >= 1)
            {
                pending_infestation = false;
                infestation_percent = 0;
                binState = BinState.INFESTED;
            }
        }
    }

    /// <summary>
    /// Use this when bin is contaminated with food items, causing it to get infested.
    /// This will also start a timer, when timer completes, the bin will release cockroaches & other pests
    /// 
    /// Sets state to contaminated, when timer runs out, state changes to infested.
    /// </summary>
    public void StartInfestation()
    {
        // Already infested skip
        if (binState == BinState.INFESTED) return;

        binState = BinState.CONTAMINATED;
        pending_infestation = true;
        infestation_percent = 0;
    }

    /// <summary>
    /// Use this when the bin is cleaned.
    /// 
    /// Sets state to clean
    /// </summary>
    public void SetClean()
    {
        binState = BinState.CLEAN;
        pending_infestation = false;
        infestation_percent = 0;
    }

    /// <summary>
    /// Use this if bin is contaminated with other recyclables
    /// 
    /// Sets state to contaminated.
    /// </summary>
    public void SetContaminated()
    {
        // Infestation takes higher priority
        if (IsInfested) return;
        binState = BinState.CONTAMINATED;
        pending_infestation = false;
        infestation_percent = 0;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        var recyclable = other.GetComponent<FSMRecyclableNPC>();
        if (recyclable == null) return;
        Debug.Log($"Recyclable {recyclable} Type {recyclable.recyclableType} entered bin {this} of type {this.recyclableType}");
        if (recyclable.is_food){
            StartInfestation();
        }
        else if ( recyclable.recyclableType != recyclableType){
            SetContaminated();
        }
    }
}