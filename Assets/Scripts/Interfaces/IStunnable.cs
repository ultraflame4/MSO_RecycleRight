namespace Interfaces
{
    /// <summary>
    ///  Objects that implements this can be stunned.
    /// </summary>
    public interface IStunnable
    {
        /// <summary>
        /// Stuns this object for a set duration
        /// </summary>
        /// <param name="stun_duration">The stun duration in seconds.</param>
        public void Stun(float stun_duration);
    }
}
