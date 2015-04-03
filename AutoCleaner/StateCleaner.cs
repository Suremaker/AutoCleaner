using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AutoCleaner
{
    /// <summary>
    /// Class allowing to automatically clean state of passed instance.
    /// </summary>
    public static class StateCleaner
    {
        /// <summary>
        /// Resets state of specified instance fields/auto-properties to their defaults (null for classes, default value for structs).
        /// If given instance field implements IDisposable interface, it would be disposed before reset.
        /// 
        /// It is possible to control which fields/auto-properties would be cleaned with hierarchy, visibility and reset options.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="target">Target instance to be reset.</param>
        /// <param name="hierarchyOptions">Options controlling which members would be cleaned, depending on which type in type hierarchy they belongs to.</param>
        /// <param name="visibilityOptions">Options controlling which members would be cleaned, depending on their visibility flags.</param>
        /// <param name="resetOptions">An additional reset options.</param>
        public static void ResetInstance<T>(T target, HierarchyOptions hierarchyOptions = HierarchyOptions.All, VisibilityOptions visibilityOptions = VisibilityOptions.All, ResetOptions resetOptions = ResetOptions.None) where T : class
        {
            if (target == null)
                return;

            Debug.WriteLine(string.Format("Resetting instance of: {0}", target.GetType()));
            foreach (var field in GetAllFields(target, typeof(T), hierarchyOptions)
                .Where(f => IsApplicable(f, visibilityOptions, resetOptions)))
            {
                if ((resetOptions & ResetOptions.DoNotDispose) == 0)
                    DisposeField(field, target);
                ResetField(field, target);
            }
        }

        private static bool IsApplicable(FieldInfo field, VisibilityOptions visibility, ResetOptions resetOptions)
        {
            if (visibility == 0)
                throw new ArgumentException("VisibilityOptions are not specified.");
            if (IsBackendField(field))
            {
                var propertyInfo = ExtractPropertyForBackendField(field);
                if (!CheckMethodVisibility(propertyInfo.GetSetMethod(true), visibility))
                    return false;
                if (!CheckNoAutoCleanFilter(propertyInfo, resetOptions))
                    return false;
            }
            else
            {
                if (!CheckFieldVisibility(field, visibility))
                    return false;
                if (!CheckReadonlyFilter(field, resetOptions))
                    return false;
                if (!CheckNoAutoCleanFilter(field, resetOptions))
                    return false;
            }
            return true;
        }

        private static PropertyInfo ExtractPropertyForBackendField(FieldInfo field)
        {
            return field.DeclaringType.GetProperty(
                field.Name.Substring(1, field.Name.Length - 17),
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static bool CheckNoAutoCleanFilter(MemberInfo member, ResetOptions resetOptions)
        {
            return resetOptions.IsSet(ResetOptions.OverruleNoAutoClean) ||
                   member.GetCustomAttributes(typeof(NoAutoCleanAttribute), true).Length == 0;
        }

        private static bool CheckReadonlyFilter(FieldInfo field, ResetOptions resetOptions)
        {
            return !field.IsInitOnly || resetOptions.IsSet(ResetOptions.IncludeReadOnlyMembers);
        }

        private static bool IsBackendField(FieldInfo field)
        {
            return field.Name.EndsWith("k__BackingField");
        }

        private static bool CheckFieldVisibility(FieldInfo field, VisibilityOptions visibilityOptions)
        {
            if (field.IsPublic && visibilityOptions.IsSet(VisibilityOptions.Public)) return true;
            if (field.IsFamily && visibilityOptions.IsSet(VisibilityOptions.Protected)) return true;
            if (field.IsPrivate && visibilityOptions.IsSet(VisibilityOptions.Private)) return true;
            if (field.IsAssembly && visibilityOptions.IsSet(VisibilityOptions.Internal)) return true;
            if (field.IsFamilyOrAssembly && visibilityOptions.IsSet(VisibilityOptions.ProtectedInternal)) return true;
            if (field.IsFamilyAndAssembly && visibilityOptions.IsSet(VisibilityOptions.ProtectedPrivate)) return true;
            return false;
        }

        private static bool CheckMethodVisibility(MethodInfo method, VisibilityOptions visibilityOptions)
        {
            if (method.IsPublic && visibilityOptions.IsSet(VisibilityOptions.Public)) return true;
            if (method.IsFamily && visibilityOptions.IsSet(VisibilityOptions.Protected)) return true;
            if (method.IsPrivate && visibilityOptions.IsSet(VisibilityOptions.Private)) return true;
            if (method.IsAssembly && visibilityOptions.IsSet(VisibilityOptions.Internal)) return true;
            if (method.IsFamilyOrAssembly && visibilityOptions.IsSet(VisibilityOptions.ProtectedInternal)) return true;
            if (method.IsFamilyAndAssembly && visibilityOptions.IsSet(VisibilityOptions.ProtectedPrivate)) return true;
            return false;
        }

        private static IEnumerable<FieldInfo> GetAllFields(object target, Type targetType, HierarchyOptions hierarchyOptions)
        {
            if (hierarchyOptions == 0)
                throw new ArgumentException("HierarchyOptions are not specified.");

            var fields = Enumerable.Empty<FieldInfo>();
            if (hierarchyOptions.IsSet(HierarchyOptions.Descendant))
                fields = fields.Concat(GetAllFields(target.GetType(), targetType));
            if (hierarchyOptions.IsSet(HierarchyOptions.Declared))
                fields = fields.Concat(GetAllFields(targetType, targetType.BaseType));
            if (hierarchyOptions.IsSet(HierarchyOptions.Inherited))
                fields = fields.Concat(GetAllFields(targetType.BaseType, null));
            return fields;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type fromType, Type toBaseType)
        {
            var fields = Enumerable.Empty<FieldInfo>();
            var type = fromType;
            while (type != null && type != toBaseType)
            {
                fields = fields.Concat(type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic));
                type = type.BaseType;
            }
            return fields;
        }

        private static void ResetField(FieldInfo field, object target)
        {
            Debug.WriteLine(string.Format("  Resetting {0}.{1}", field.DeclaringType.Name, field.Name));
            field.SetValue(target, field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null);
        }

        private static void DisposeField(FieldInfo field, object target)
        {
            var value = field.GetValue(target) as IDisposable;
            if (value == null)
                return;
            Debug.WriteLine(string.Format("  Disposing {0}.{1}", field.DeclaringType.Name, field.Name));
            value.Dispose();
        }
    }

    internal static class EnumExtensions
    {
        public static bool IsSet<T>(this T target, T value) where T : struct
        {
            var t = Convert.ToInt32(target);
            var v = Convert.ToInt32(value);
            return (t & v) != 0;
        }
    }
}
