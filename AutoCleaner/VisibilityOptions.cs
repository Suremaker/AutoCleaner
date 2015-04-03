using System;

namespace AutoCleaner
{
    /// <summary>
    /// Visibility options controls which members would be cleaned, depending on their visibility flags.
    /// 
    /// Please note that for AutomaticProperties, the visibility of setter method would be used.
    /// </summary>
    [Flags]
    public enum VisibilityOptions : byte
    {
        /// <summary>
        /// Include public members.
        /// </summary>
        Public = 1,
        /// <summary>
        /// Include protected members.
        /// </summary>
        Protected = 2,
        /// <summary>
        /// Include private members.
        /// </summary>
        Private = 4,
        /// <summary>
        /// Include internal members.
        /// </summary>
        Internal = 8,
        /// <summary>
        /// Include protected-interna members.
        /// </summary>
        ProtectedInternal = 16,
        /// <summary>
        /// Include protected-private members (Family and Assembly).
        /// </summary>
        ProtectedPrivate = 32,
        /// <summary>
        /// Include all non-public members.
        /// </summary>
        NonPublic = Protected | Private | Internal | ProtectedInternal | ProtectedPrivate,
        /// <summary>
        /// Include all members.
        /// </summary>
        All = Public | NonPublic,
    }
}