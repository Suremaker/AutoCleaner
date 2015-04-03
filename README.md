AutoCleaner
===========

AutoCleaner project offers tool for automatic class instance state cleanup, usefull for all kind of Context classes or Test classes.

It can be used in situation when test class uses fields to share state between test methods, while such fields should be cleaned up between tests.
Such situation could be often found in projects using NUnit, MbUnit or MsTest test frameworks. Such situation is valid also for [LightBDD scenarios](https://github.com/Suremaker/LightBDD#example).

The AutoCleaner tool offers **StateCleaner** class, that allows to automatically cleanup class fields (including property backend ones) by:

* disposing all objects implementing IDisposable interface,
* resetting fields to default values (null, or empty struct).

It also offers various filters to clean only a subset of fields: 

* **Hierarchy options**, allowing to reset fields belonging to current type, base types or derived types,
* **Visibility options**, allowing to reset fields basing on their visibiliry (i.e. public, private, protected and others),
* **Reset options**, allowing to control if readonly fields or fields annotated with [NoAutoClean] attribute should be cleaned as well as allows to skip disposal of disposable fields.

## Examples

Please see [AutoCleaner.Example](https://github.com/Suremaker/AutoCleaner/tree/master/AutoCleaner.Example), especially [My_feature_with_state.Steps.cs](https://github.com/Suremaker/AutoCleaner/blob/master/AutoCleaner.Example/My_feature_with_state.Steps.cs) file.

Below, there is also an [AutoCleaner.Example2 code](https://github.com/Suremaker/AutoCleaner/tree/master/AutoCleaner.Example2) showing behavior of **Hierarchy options**:

```C#
class Disposable : IDisposable
{
    private readonly string _text;
    public Disposable(string text) { _text = text; }
    public void Dispose() { Console.WriteLine("  Disposed {0}", _text); }
}
class Parent { private Disposable _field = new Disposable("Parent field"); }
class MyClass : Parent { private Disposable _field = new Disposable("My field"); }
class ChildClass : MyClass { private Disposable _field = new Disposable("Child field"); }

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
```

and its output:

```
# Simple cleanup
  Disposed Child field
  Disposed My field
  Disposed Parent field
# Only self
  Disposed Child field
# Only parents
  Disposed My field
  Disposed Parent field
# Only self (referred as base class)
  Disposed My field
# Only parent (referred as base class)
  Disposed Parent field
# Only children (referred as base class)
  Disposed Child field
```

## Download
It is possible to download package using [NuGet](http://nuget.org):  
`PM> Install-Package AutoCleaner`  
or to clone sources from git: `git clone git://github.com/Suremaker/AutoCleaner.git`