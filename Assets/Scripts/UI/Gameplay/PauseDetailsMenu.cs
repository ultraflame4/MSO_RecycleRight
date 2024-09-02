using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PauseDetailsMenu : MonoBehaviour
    {
        [SerializeField] GameObject defaultPauseMenu;

        [Header("Character Select Profile")]
        [SerializeField] Transform profileParent;
        [SerializeField] GameObject profilePrefab;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            defaultPauseMenu.SetActive(false);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            defaultPauseMenu.SetActive(true);
        }
    }
}
