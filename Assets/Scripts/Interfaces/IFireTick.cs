namespace Interfaces
{
    /// <summary>
    /// Objects that implement this can be damaged by fire tiles.
    /// </summary>
    public interface IFireTick
    {
        /// <summary>
        /// Damage by fire
        /// </summary>
        /// <param name="dmg">Amount of damage</param>
        public void ApplyFireDamage(float dmg);
    }
}