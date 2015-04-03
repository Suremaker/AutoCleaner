using System;

namespace AutoCleaner
{
    /// <summary>
    /// Hierarchy options controls which members would be cleaned, depending on which type in type hierarchy they belongs to.
    /// 
    /// Hierarchy analysis is based on a type that is passed to StateCleaner.ResetInstance() method.
    /// 
    /// If instance is passed as a base type, it is possible to specify if declared, inherited or descendant members should be cleaned.
    /// It is possible also to use any combination of those flags together.
    /// </summary>
    [Flags]
    public enum HierarchyOptions : byte
    {
        /// <summary>
        /// Includes all members declared in specified type.
        /// 
        /// See example:
        /// <code>
        ///     class Base { int _field; }
        ///     class Current: Base { int _field2; }
        ///     class Child: Current { int _field3; }
        /// 
        ///     Current c = new Child();
        /// 
        ///     StateCleaner.ResetInstance(c, HierarchyOptions.Declared, VisibilityOptions.All);
        /// </code>
        /// 
        /// Because instance 'c' has been passed as Current type and Declared option is used, only _field2 would be cleaned.
        /// </summary>
        Declared = 1,
        /// <summary>
        /// Includes all members declared in child types of specified type.
        /// Applicable only, if a base type is used to pass instance to StateCleaner.
        /// 
        /// See example:
        /// <code>
        ///     class Base { int _field; }
        ///     class Current: Base { int _field2; }
        ///     class Child: Current { int _field3; }
        /// 
        ///     Current c = new Child();
        /// 
        ///     StateCleaner.ResetInstance(c, HierarchyOptions.Descendant, VisibilityOptions.All);
        /// </code>
        /// 
        /// Because instance 'c' has been passed as Current type and Descendant option is used, only _field3 would be cleaned.
        /// </summary>
        Descendant = 2,
        /// <summary>
        /// Includes all members declared in inherited types of specified type.
        /// 
        /// See example:
        /// <code>
        ///     class Base { int _field; }
        ///     class Current: Base { int _field2; }
        ///     class Child: Current { int _field3; }
        /// 
        ///     Current c = new Child();
        /// 
        ///     StateCleaner.ResetInstance(c, HierarchyOptions.Inherited, VisibilityOptions.All);
        /// </code>
        /// 
        /// Because instance 'c' has been passed as Current type and Inherited option is used, only _field would be cleaned.
        /// </summary>
        Inherited = 4,
        /// <summary>
        /// Includes all members of specified type, no matter if they are declared in base types, specified type or child types.
        /// </summary>
        All = Declared | Descendant | Inherited,
    }
}