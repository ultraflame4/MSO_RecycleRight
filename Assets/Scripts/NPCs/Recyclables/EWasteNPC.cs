namespace NPC.Recyclable
{
    public class EWasteNPC : RecyclableNPC
    {
        public override void Contaminate(float dmg)
        {
            // EWasteNPCs are immune to contamination
        }
    }
}