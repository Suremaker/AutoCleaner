using System;

namespace AutoCleaner
{
    /// <summary>
    /// An additional options controlling behavior of cleaning instance fields/auto-properties.
    /// </summary>
    [Flags]
    public enum ResetOptions : byte
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Include read-only fields in cleanup. 
        /// </summary>
        IncludeReadOnlyMembers = 1,
        /// <summary>
        /// Include fields/properties with NoAutoClean attributes.
        /// </summary>
        OverruleNoAutoClean = 2,
        /// <summary>
        /// Do not call Dispose() method on members implementing IDisposable() interface.
        /// </summary>
        DoNotDispose = 4
    }
}