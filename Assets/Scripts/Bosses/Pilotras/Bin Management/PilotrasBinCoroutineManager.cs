using System.Collections;
using UnityEngine;
using Level.Bins;
using Bosses.Pilotras.FSM;

namespace Bosses.Pilotras.Bin
{
    public class PilotrasBinCoroutineManager : MonoBehaviour
    {
        // references
        PilotrasController character;
        BinDropState binDrop => character.BinDropState;

        // variables to manage indicator
        Vector3 originalIndicatorScale;
        Transform indicatorSprite;

        void Start()
        {
            // get character controller component
            character = GetComponentInParent<PilotrasController>();
            // create indicator, and store sprite transform and original scale
            GameObject indicator = character.indicatorManager.Instantiate(2, Vector2.zero);
            indicatorSprite = indicator.transform.GetChild(0);
            originalIndicatorScale = new Vector3(indicatorSprite.transform.localScale.x, indicatorSprite.transform.localScale.y, 
                    indicatorSprite.transform.localScale.z);
            indicator.SetActive(false);
        }

        public IEnumerator DelayedBinDrop()
        {
            // set bin location and activate bin
            binDrop.binContainerLocation = (Vector2) character.transform.position + character.behaviourData.bin_offset;
            binDrop.xPos = binDrop.binContainerLocation.x - (character.behaviourData.bin_spacing * ((binDrop.binManager.selectedBins.Count - 1f) / 2f));
            binDrop.yPos = binDrop.binContainerLocation.y;

            // set shockwave values
            binDrop.shockwaveSize = new Vector2(Mathf.Abs(binDrop.xPos) + character.behaviourData.bin_spacing, 
                character.yPosTop - binDrop.binContainerLocation.y + character.behaviourData.bin_spacing);
            binDrop.shockwaveCenter = binDrop.binContainerLocation;
            binDrop.shockwaveCenter.y += (binDrop.shockwaveSize.y - character.behaviourData.bin_spacing) / 2f;

            // show indicator
            Vector2 indicatorScale = binDrop.shockwaveSize;
            indicatorScale.y -= (character.transform.position.y - character.data.maxBounds.y) * 0.5f;
            binDrop.indicator = character.indicatorManager.Instantiate(2, 
                new Vector2(binDrop.shockwaveCenter.x, binDrop.shockwaveCenter.y - (indicatorScale.y * 0.25f)));
            indicatorSprite.localScale = originalIndicatorScale * indicatorScale;

            // delay the attack by the attack indicator
            yield return new WaitForSeconds(character.data.attack_delay);
            // hide indicator after attack delay duration
            binDrop.indicator.SetActive(false);
            // drop bin after waiting for attack delay
            binDrop.binManager.DropBin();
        }

        public IEnumerator AwaitRaiseBin()
        {
            yield return new WaitForSeconds(binDrop._duration - character.behaviourData.bin_drop_speed);
            // raise bin after waiting
            binDrop.binManager.RaiseBin();
        }

        public IEnumerator DelayedBinEnable(RecyclingBin bin, bool firstBin)
        {
            yield return new WaitForSeconds(character.behaviourData.bin_drop_delay + 
                character.behaviourData.bin_drop_speed);
            bin.enabled = true;
            if (firstBin) HandleFirstBin();
        }

        void HandleFirstBin()
        {
            // play sfx when bin landed on ground
            SoundManager.Instance?.PlayOneShot(character.data.sfx_bin_drop);
            // shake camera for effects
            character.levelManager?.camera?.ShakeCamera(0.5f);
        }

        public IEnumerator DelayedBinInactive(RecyclingBin bin)
        {
            yield return new WaitForSeconds(character.behaviourData.bin_drop_speed);
            bin?.gameObject.SetActive(false);
        }
    }
}
