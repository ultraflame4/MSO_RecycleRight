namespace Interfaces
{
    /// <summary>
    /// Objects that return this method can toggle between being able, or unable to be moved by players
    /// </summary>
    public interface IAmNotMovableByWilson
    {
        /// <summary>
        /// Checks if the object can be moved
        /// </summary>
        /// <returns>Returns a boolean depending on if the object can be moved</returns>
        public bool CanMove();
    }
}
