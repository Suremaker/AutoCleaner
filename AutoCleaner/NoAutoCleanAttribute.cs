using System;

namespace AutoCleaner
{
    /// <summary>
    /// Attribute applicable on fields and (auto) properties.
    /// If present, StateCleaner will not clean annotated field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NoAutoCleanAttribute : Attribute
    {
    }
}