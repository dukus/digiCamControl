namespace Canon.Eos.Framework
{
    public class EosVolumeInfo
    {
        internal EosVolumeInfo() { }

        /// <summary>
        /// Gets the access.
        /// </summary>
        public long Access { get; internal set; }

        /// <summary>
        /// Gets the free space in bytes.
        /// </summary>
        public ulong FreeSpaceInBytes { get; internal set; }

        /// <summary>
        /// Gets the max capacity in bytes.
        /// </summary>
        public ulong MaxCapacityInBytes { get; internal set; }

        /// <summary>
        /// Gets the type of the storage.
        /// </summary>
        /// <value>
        /// The type of the storage.
        /// </value>
        public long StorageType { get; internal set; }

        /// <summary>
        /// Gets the volume label.
        /// </summary>
        public string VolumeLabel { get; internal set; }
    }
}
