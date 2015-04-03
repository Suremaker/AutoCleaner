using System;
using AutoCleaner.UnitTests.Hierarchy;
using Moq;
using NUnit.Framework;

namespace AutoCleaner.UnitTests
{
    [TestFixture]
    public class StateCleanerTests
    {
        [Test]
        [TestCase(HierarchyOptions.Declared, false, true, false)]
        [TestCase(HierarchyOptions.Descendant, true, false, false)]
        [TestCase(HierarchyOptions.Inherited, false, false, true)]
        [TestCase(HierarchyOptions.Inherited | HierarchyOptions.Declared, false, true, true)]
        [TestCase(HierarchyOptions.Inherited | HierarchyOptions.Descendant, true, false, true)]
        [TestCase(HierarchyOptions.Descendant | HierarchyOptions.Declared, true, true, false)]
        [TestCase(HierarchyOptions.All, true, true, true)]
        public void ResetInstance_should_honor_hierarchy_options(HierarchyOptions hierarchyOptions, bool shouldResetChildren, bool shouldResetThis, bool shouldResetParent)
        {
            var text = "abc";
            var guid = Guid.NewGuid();
            var childDisposable = new Mock<IDisposable>();
            var disposable = new Mock<IDisposable>();
            var grandChildDisposable = new Mock<IDisposable>();
            var grandDisposable = new Mock<IDisposable>();
            var parentDisposable = new Mock<IDisposable>();

            var instance = new GrandChild
            {
                ChildClass = text,
                ChildDisposable = childDisposable.Object,
                ChildStruct = guid,
                Class = text,
                Disposable = disposable.Object,
                GrandChildClass = text,
                GrandChildDisposable = grandChildDisposable.Object,
                GrandChildStruct = guid,
                GrandClass = text,
                GrandDisposable = grandDisposable.Object,
                GrandStruct = guid,
                ParentClass = text,
                ParentDisposable = parentDisposable.Object,
                ParentStruct = guid,
                Struct = guid,
                AbstractProperty = text,
                CurrentAbstractProperty = text,
                GrandAbstractProperty = text
            };
            Current current = instance;
            StateCleaner.ResetInstance(current, hierarchyOptions, VisibilityOptions.Public);

            Assert.That(instance.GrandChildClass == null, Is.EqualTo(shouldResetChildren));
            Assert.That(instance.AbstractProperty == null, Is.EqualTo(shouldResetChildren));
            Assert.That(instance.GrandChildStruct == Guid.Empty, Is.EqualTo(shouldResetChildren));
            Assert.That(instance.GrandChildDisposable == null, Is.EqualTo(shouldResetChildren));
            grandChildDisposable.Verify(d => d.Dispose(), Times.Exactly(shouldResetChildren ? 1 : 0));

            Assert.That(instance.ChildClass == null, Is.EqualTo(shouldResetChildren));
            Assert.That(instance.ChildStruct == Guid.Empty, Is.EqualTo(shouldResetChildren));
            Assert.That(instance.ChildDisposable == null, Is.EqualTo(shouldResetChildren));
            childDisposable.Verify(d => d.Dispose(), Times.Exactly(shouldResetChildren ? 1 : 0));

            Assert.That(instance.Class == null, Is.EqualTo(shouldResetThis));
            Assert.That(instance.CurrentAbstractProperty == null, Is.EqualTo(shouldResetThis));
            Assert.That(instance.Struct == Guid.Empty, Is.EqualTo(shouldResetThis));
            Assert.That(instance.Disposable == null, Is.EqualTo(shouldResetThis));
            disposable.Verify(d => d.Dispose(), Times.Exactly(shouldResetThis ? 1 : 0));

            Assert.That(instance.ParentClass == null, Is.EqualTo(shouldResetParent));
            Assert.That(instance.ParentStruct == Guid.Empty, Is.EqualTo(shouldResetParent));
            Assert.That(instance.ParentDisposable == null, Is.EqualTo(shouldResetParent));
            parentDisposable.Verify(d => d.Dispose(), Times.Exactly(shouldResetParent ? 1 : 0));

            Assert.That(instance.GrandClass == null, Is.EqualTo(shouldResetParent));
            Assert.That(instance.GrandAbstractProperty == null, Is.EqualTo(shouldResetParent));
            Assert.That(instance.GrandStruct == Guid.Empty, Is.EqualTo(shouldResetParent));
            Assert.That(instance.GrandDisposable == null, Is.EqualTo(shouldResetParent));
            grandDisposable.Verify(d => d.Dispose(), Times.Exactly(shouldResetParent ? 1 : 0));
        }

        [Test]
        public void ResetInstance_should_do_nothing_if_target_is_null()
        {
            Assert.DoesNotThrow(() => StateCleaner.ResetInstance((Current)null, HierarchyOptions.All, VisibilityOptions.All));
        }

        [Test]
        [TestCase(VisibilityOptions.Public, true, false, false, false, false)]
        [TestCase(VisibilityOptions.Protected, false, true, false, false, false)]
        [TestCase(VisibilityOptions.Private, false, false, true, false, false)]
        [TestCase(VisibilityOptions.Internal, false, false, false, true, false)]
        [TestCase(VisibilityOptions.ProtectedInternal, false, false, false, false, true)]
        [TestCase(VisibilityOptions.Public | VisibilityOptions.Protected, true, true, false, false, false)]
        [TestCase(VisibilityOptions.Public | VisibilityOptions.Private, true, false, true, false, false)]
        [TestCase(VisibilityOptions.Protected | VisibilityOptions.Private, false, true, true, false, false)]
        [TestCase(VisibilityOptions.All, true, true, true, true, true)]
        [TestCase(VisibilityOptions.NonPublic, false, true, true, true, true)]
        public void ResetInstance_should_honor_visibility_options(VisibilityOptions visibilityOptions, bool shouldResetPublic, bool shouldResetProtected, bool shouldResetPrivate, bool shouldResetInternal, bool shouldResetProtectedInternal)
        {
            var current = new Visibility.Current();
            StateCleaner.ResetInstance(current, HierarchyOptions.All, visibilityOptions);

            Assert.That(current.PublicField == null, Is.EqualTo(shouldResetPublic));
            Assert.That(current.PublicPublicProperty == null, Is.EqualTo(shouldResetPublic));

            Assert.That(current.GetProtectedField == null, Is.EqualTo(shouldResetProtected));
            Assert.That(current.PublicProtectedProperty == null, Is.EqualTo(shouldResetProtected));

            Assert.That(current.GetPrivateField == null, Is.EqualTo(shouldResetPrivate));
            Assert.That(current.PublicPrivateProperty == null, Is.EqualTo(shouldResetPrivate));

            Assert.That(current.InternalField == null, Is.EqualTo(shouldResetInternal));
            Assert.That(current.ProtectedInternalField == null, Is.EqualTo(shouldResetProtectedInternal));
        }

        [Test]
        [TestCase(ResetOptions.None, false, false, false, false, true, true)]
        [TestCase(ResetOptions.DoNotDispose, false, false, false, false, true, false)]
        [TestCase(ResetOptions.IncludeReadOnlyMembers, false, true, false, false, true, true)]
        [TestCase(ResetOptions.OverruleNoAutoClean, true, false, true, false, true, true)]
        [TestCase(ResetOptions.OverruleNoAutoClean | ResetOptions.IncludeReadOnlyMembers, true, true, true, true, true, true)]
        [TestCase(ResetOptions.OverruleNoAutoClean | ResetOptions.IncludeReadOnlyMembers | ResetOptions.DoNotDispose, true, true, true, true, true, false)]
        public void ResetInstance_should_honor_options(ResetOptions resetOptions, bool shouldResetNoCleanField, bool shouldResetReadonlyField, bool shouldResetNoCleanProperty, bool shouldResetNoCleanReadonlyField, bool shouldResetField, bool shouldDispose)
        {
            var noCleanField = new Mock<IDisposable>();
            var readonlyField = new Mock<IDisposable>();
            var noCleanProperty = new Mock<IDisposable>();
            var noCleanReadonlyField = new Mock<IDisposable>();
            var field = new Mock<IDisposable>();

            var current = new Options.Current(
                noCleanField.Object,
                readonlyField.Object,
                noCleanProperty.Object,
                noCleanReadonlyField.Object,
                field.Object
                );

            StateCleaner.ResetInstance(current, HierarchyOptions.All, VisibilityOptions.All, resetOptions);

            Assert.That(current.NoCleanField == null, Is.EqualTo(shouldResetNoCleanField));
            noCleanField.Verify(d => d.Dispose(), Times.Exactly(shouldResetNoCleanField && shouldDispose ? 1 : 0));

            Assert.That(current.ReadonlyField == null, Is.EqualTo(shouldResetReadonlyField));
            readonlyField.Verify(d => d.Dispose(), Times.Exactly(shouldResetReadonlyField && shouldDispose ? 1 : 0));

            Assert.That(current.NoCleanProperty == null, Is.EqualTo(shouldResetNoCleanProperty));
            noCleanProperty.Verify(d => d.Dispose(), Times.Exactly(shouldResetNoCleanProperty && shouldDispose ? 1 : 0));

            Assert.That(current.NoCleanReadonlyField == null, Is.EqualTo(shouldResetNoCleanReadonlyField));
            noCleanReadonlyField.Verify(d => d.Dispose(), Times.Exactly(shouldResetNoCleanReadonlyField && shouldDispose ? 1 : 0));

            Assert.That(current.Field == null, Is.EqualTo(shouldResetField));
            field.Verify(d => d.Dispose(), Times.Exactly(shouldResetField && shouldDispose ? 1 : 0));

        }

        [Test]
        public void ResetInstance_should_throw_if_visibility_is_not_specified()
        {
            var ex = Assert.Throws<ArgumentException>(() => StateCleaner.ResetInstance(new Visibility.Current(), HierarchyOptions.All, default(VisibilityOptions)));
            Assert.That(ex.Message, Is.EqualTo("VisibilityOptions are not specified."));
        }

        [Test]
        public void ResetInstance_should_throw_if_hierarchy_is_not_specified()
        {
            var ex = Assert.Throws<ArgumentException>(() => StateCleaner.ResetInstance(new Visibility.Current(), default(HierarchyOptions), VisibilityOptions.All));
            Assert.That(ex.Message, Is.EqualTo("HierarchyOptions are not specified."));
        }

        [Test]
        public void ResetInstance_should_handler_null_fields()
        {
            var current = new Options.Current(null, new Mock<IDisposable>().Object, null, null, new Mock<IDisposable>().Object);
            Assert.DoesNotThrow(() => StateCleaner.ResetInstance(current, HierarchyOptions.All, VisibilityOptions.All, ResetOptions.OverruleNoAutoClean | ResetOptions.IncludeReadOnlyMembers));
            Assert.That(current.ReadonlyField,Is.Null);
            Assert.That(current.Field,Is.Null);
        }
    }

    namespace Hierarchy
    {
        public class GrandParent
        {
            public IDisposable GrandDisposable;
            public string GrandClass;
            public Guid GrandStruct;
            public virtual string AbstractProperty { get; set; }
        }

        public class Parent : GrandParent
        {
            public IDisposable ParentDisposable;
            public string ParentClass;
            public Guid ParentStruct;
        }

        public class Current : Parent
        {
            public IDisposable Disposable;
            public string Class;
            public Guid Struct;
            public override string AbstractProperty { get; set; }
            public string GrandAbstractProperty { get { return base.AbstractProperty; } set { base.AbstractProperty = value; } }
        }

        public class Child : Current
        {
            public IDisposable ChildDisposable;
            public string ChildClass;
            public Guid ChildStruct;
        }

        public class GrandChild : Child
        {
            public IDisposable GrandChildDisposable;
            public string GrandChildClass;
            public Guid GrandChildStruct;
            public override string AbstractProperty { get; set; }
            public string CurrentAbstractProperty { get { return base.AbstractProperty; } set { base.AbstractProperty = value; } }
        }
    }

    namespace Visibility
    {
        class Current
        {
            public string PublicField;
            protected string ProtectedField;
            private string PrivateField;
            internal string InternalField;
            protected internal string ProtectedInternalField;

            public string PublicPublicProperty { get; set; }
            public string PublicProtectedProperty { get; protected set; }
            public string PublicPrivateProperty { get; private set; }

            public Current()
            {
                PublicField = ProtectedField = PrivateField = InternalField = ProtectedInternalField = "abc";
                PublicPublicProperty = PublicProtectedProperty = PublicPrivateProperty = "abc";
            }

            public string GetProtectedField { get { return ProtectedField; } }
            public string GetPrivateField { get { return PrivateField; } }
        }
    }

    namespace Options
    {
        public class Current
        {
            [NoAutoClean]
            public IDisposable NoCleanField;
            public readonly IDisposable ReadonlyField;
            [NoAutoClean]
            public IDisposable NoCleanProperty { get; set; }

            [NoAutoClean]
            public readonly IDisposable NoCleanReadonlyField;
            public IDisposable Field;

            public Current(IDisposable noCleanField, IDisposable readonlyField, IDisposable noCleanProperty, IDisposable noCleanReadonlyField, IDisposable field)
            {
                NoCleanField = noCleanField;
                ReadonlyField = readonlyField;
                NoCleanProperty = noCleanProperty;
                NoCleanReadonlyField = noCleanReadonlyField;
                Field = field;
            }
        }
    }
}
