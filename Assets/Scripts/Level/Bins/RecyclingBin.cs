using TMPro;
using UnityEngine;
using NPC;

namespace Level.Bins
{
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
        private float _score = 0;
        public float Score{
            get => _score;
            set {
                if (binState != BinState.CLEAN) return;
                _score = value;
            }
        }
        public bool IsInfested => infestation_percent > 0 || binState == BinState.INFESTED;
        public TMP_Text nameText;
        public TMP_Text scoreText;
        public ParticleSystem[] cleaningEffects;

        // sprites
        public Sprite contaminatedSprite;
        private Sprite cleanedSprite;
        private new SpriteRenderer renderer;

        private void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            if (renderer == null) return;
            cleanedSprite = renderer.sprite;
            // check if already contaminated, if so change sprite to contaminated
            if (binState == BinState.CLEAN || contaminatedSprite == null) return;
            renderer.sprite = contaminatedSprite;
            SetActiveCleaningEffects(false);
        }

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
            scoreText.text = $"Score: {Score}";
        }

        void SetActiveCleaningEffects(bool active)
        {
            // Loop through all cleaning effects and start/stop them
            foreach (var effect in cleaningEffects)
            {
                if (effect == null) continue;
                if (active)
                {
                    effect.Play();
                }
                else
                {
                    effect.Stop();
                }
            }
        }

        /// <summary>
        /// Use this when bin is contaminated with food items, causing it to get infested.
        /// This will also start a timer, when timer completes, the bin will release cockroaches & other pests
        /// 
        /// Sets state to contaminated, when timer runs out, state changes to infested.
        /// </summary>
        [EasyButtons.Button]
        public void StartInfestation()
        {
            // Already infested skip
            if (binState == BinState.INFESTED) return;

            binState = BinState.CONTAMINATED;
            pending_infestation = true;
            infestation_percent = 0;
            Score = 0;

            if (renderer == null) return;
            renderer.sprite = contaminatedSprite;
        }

        /// <summary>
        /// Use this when the bin is being cleaned.
        /// 
        /// Sets state to cleaning
        /// </summary>
        [EasyButtons.Button]
        public void SetCleaning()
        {
            binState = BinState.CLEANING;
            pending_infestation = false;
            infestation_percent = 0;

            // Disable text as they are not used anyways
            nameText.enabled = false;
            scoreText.enabled = false;
            SetActiveCleaningEffects(true);
            
        }

        /// <summary>
        /// Use this when bin has completed its cleaning proccess and can continue being used
        /// </summary>
        [EasyButtons.Button]
        public void CompleteClean()
        {
            binState = BinState.CLEAN;
            if (renderer == null) return;
            renderer.sprite = cleanedSprite;

            // Renable text
            nameText.enabled = true;
            scoreText.enabled = true;
            SetActiveCleaningEffects(false);
        }

        /// <summary>
        /// Use this if bin is contaminated with other recyclables
        /// 
        /// Sets state to contaminated.
        /// </summary>
        [EasyButtons.Button]
        public void SetContaminated()
        {
            // Infestation takes higher priority
            if (IsInfested) return;
            binState = BinState.CONTAMINATED;
            pending_infestation = false;
            infestation_percent = 0;
            Score = 0;

            if (renderer == null) return;
            renderer.sprite = contaminatedSprite;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (binState != BinState.CLEAN) return;
            var recyclable = other.GetComponent<FSMRecyclableNPC>();
            if (recyclable == null) return;
            Debug.Log($"Recyclable {recyclable} Type {recyclable.recyclableType} entered bin {this} of type {this.recyclableType}");
            recyclable.OnEnteredBin(this);
        }
    }
}