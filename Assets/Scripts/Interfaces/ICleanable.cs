namespace Interfaces
{
    /// <summary>
    /// Objects that implemented this can be cleaned
    /// </summary>
    public interface ICleanable
    {
        /// <summary>
        /// Cleans this object by a certain amount.
        /// </summary>
        /// <param name="clean_amount">The amount to clean (Percentage ranging from 0.0 - 1.0)</param>
        public void Clean(float clean_amount);
    }
}
