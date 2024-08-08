using System;
using UnityEngine;
using TMPro;
using NPC;
using Unity.VisualScripting;

namespace Level.Bins
{
    [RequireComponent(typeof(SpriteRenderer), typeof(PestSpawner))]
    public class RecyclingBin : MonoBehaviour
    {
        #region Component Config
        [Tooltip("How long it will take for the bin to become infested, once contaminated with food items.")]
        public float infestation_secs;
        [Tooltip("Whether or not this bin can be cleaned by players")]
        public bool cleanable = true;

        [field: Header("Config")]

        [field: SerializeField]
        public RecyclableType recyclableType { get; private set; }
        [field: SerializeField]
        public BinState binState { get; private set; }
        #endregion

        #region Component References
        
        [Header("References")]

        public TMP_Text nameText;
        public TMP_Text scoreText;
        public ParticleSystem[] cleaningEffects;
        public Sprite contaminatedSprite;
        private Sprite cleanedSprite;
        private SpriteRenderer spriteR;
        private PestSpawner pestSpawner;
        #endregion

        [field: Header("Internal"), SerializeField]
        public float infestation_percent { get; private set; }
        [SerializeField]
        private bool pending_infestation;
        [SerializeField]
        private float _score = 0;
        public float Score
        {
            get => _score;
            set
            {
                if (binState != BinState.CLEAN){
                    _score = 0;
                    return;
                }
                _score = value;
                

            }
        }
        public bool IsInfested => infestation_percent > 0 || binState == BinState.INFESTED;

        public event Action<float, RecyclableType?> BinScored;

        private void Awake() {
            spriteR = GetComponent<SpriteRenderer>();
            pestSpawner = GetComponent<PestSpawner>();

            cleanedSprite = spriteR.sprite; // Initialise here.
        }

        private void Start()
        {
            
            if (spriteR == null) return;
            
            SetActiveCleaningEffects(false);
            // check if already contaminated, if so change sprite to contaminated
            if (binState == BinState.CLEAN || contaminatedSprite == null) return;
            spriteR.sprite = contaminatedSprite;
        }

        private void Update()
        {
            if (pending_infestation)
            {
                infestation_percent += Time.deltaTime / infestation_secs;
                if (infestation_percent >= 1)
                {
                    // Bin is infested

                    pending_infestation = false;
                    infestation_percent = 0;
                    binState = BinState.INFESTED;
                    pestSpawner.StartPestSpawning();
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
            if (binState == BinState.INFESTED || pending_infestation) return;
            // If cleaning, skip infestation
            if (binState == BinState.CLEANING) return;

            binState = BinState.CONTAMINATED;
            pending_infestation = true;
            infestation_percent = 0;
            Score = 0;

            if (spriteR == null) return;
            spriteR.sprite = contaminatedSprite;
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
            pestSpawner.StopPestSpawning();
            

        }

        /// <summary>
        /// Use this when bin has completed its cleaning proccess and can continue being used
        /// </summary>
        [EasyButtons.Button]
        public void CompleteClean()
        {
            binState = BinState.CLEAN;
            Score = 0;
            if (spriteR != null) spriteR.sprite = cleanedSprite;
            // Renable text
            nameText.enabled = true;
            scoreText.enabled = true;
            SetActiveCleaningEffects(false);
            pestSpawner.ClearPests();
            pestSpawner.StopPestSpawning();
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

            if (spriteR == null) return;
            spriteR.sprite = contaminatedSprite;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            var item = other.GetComponent<IBinTrashItem>();
            if (item == null) return;
            var prevScore = Score;
            item.OnEnterBin(this);
            BinScored?.Invoke(Score - prevScore, other.GetComponent<FSMRecyclableNPC>()?.recyclableType);
        }
    }
}