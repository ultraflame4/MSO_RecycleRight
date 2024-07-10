namespace Level.Bins
{
    public enum BinState
    {
        /// <summary>
        /// Bin is clean, can be used, and can recieve points
        /// </summary>
        CLEAN,
        /// <summary>
        /// Bin is contaminated and needs to be cleaned
        /// </summary>
        CONTAMINATED,
        /// <summary>
        /// Bin is infested, needs to be cleaned, and can spawn insects
        /// </summary>
        INFESTED, 
        /// <summary>
        /// Bin is currently in the proccess of being cleaned
        /// </summary>
        CLEANING
    }
}    