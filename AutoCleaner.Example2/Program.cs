using System;

namespace AutoCleaner.Example2
{
    class Disposable : IDisposable
    {
        private readonly string _text;
        public Disposable(string text) { _text = text; }
        public void Dispose() { Console.WriteLine("  Disposed {0}", _text); }
    }

    class Parent
    {
        private Disposable _field = new Disposable("Parent field");
    }

    class MyClass : Parent
    {
        private Disposable _field = new Disposable("My field");
    }

    class ChildClass : MyClass
    {
        private Disposable _field = new Disposable("Child field");
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("# Simple cleanup");
            StateCleaner.ResetInstance<ChildClass>(new ChildClass());

            Console.WriteLine("# Only self");
            StateCleaner.ResetInstance<ChildClass>(new ChildClass(), HierarchyOptions.Declared);

            Console.WriteLine("# Only parents");
            StateCleaner.ResetInstance<ChildClass>(new ChildClass(), HierarchyOptions.Inherited);

            Console.WriteLine("# Only self (referred as base class)");
            StateCleaner.ResetInstance<MyClass>(new ChildClass(), HierarchyOptions.Declared);

            Console.WriteLine("# Only parent (referred as base class)");
            StateCleaner.ResetInstance<MyClass>(new ChildClass(), HierarchyOptions.Inherited);

            Console.WriteLine("# Only children (referred as base class)");
            StateCleaner.ResetInstance<MyClass>(new ChildClass(), HierarchyOptions.Descendant);

            Console.ReadKey();
        }
    }
}
