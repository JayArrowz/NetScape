﻿namespace NetScape.Abstractions.Cache
{
    /// <summary>
    /// Represents metadata of an entry within a <see cref="CacheFileInfo" />.
    /// </summary>
    public class CacheFileEntryInfo
    {
        /// <summary>
        /// The entry's id.
        /// </summary>
        public int? EntryId { get; set; }

        /// <summary>
        /// This entry's identifier.
        /// </summary>
        public int? Identifier { get; set; }

        /// <summary>
        /// Returns a copy of this object with the same values.
        /// </summary>
        /// <returns></returns>
        public CacheFileEntryInfo Clone()
        {
            return new CacheFileEntryInfo
            {
                EntryId = this.EntryId,
                Identifier = this.Identifier
            };
        }
    }
}