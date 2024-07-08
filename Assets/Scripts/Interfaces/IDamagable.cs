namespace Interfaces
{
    /// <summary>
    /// Objects that implement this can be damaged.
    /// </summary>
    public interface IDamagable
    {
        /// <summary>
        /// Damages this entity by a certain amount
        /// </summary>
        /// <param name="damage">damage amount.</param>
        public void Damage(float damage);
    }
}
