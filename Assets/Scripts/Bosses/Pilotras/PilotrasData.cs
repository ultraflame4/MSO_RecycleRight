using UnityEngine;

namespace Bosses.Pilotras
{
    public class PilotrasData : MonoBehaviour
    {
        [SerializeField] EntitySO data;
        public EntitySO entityData => data;
    }
}
