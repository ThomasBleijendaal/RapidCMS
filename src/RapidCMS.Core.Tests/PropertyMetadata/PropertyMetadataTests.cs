using NUnit.Framework;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RapidCMS.Core.Tests.PropertyMetadata;

public class PropertyMetadataTests
{
    [Test]
    public void BasicProperty()
    {
        var instance = new BasicClass { Test = "Test Value" };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("Test"));
        Assert.That(data.Getter(instance), Is.EqualTo("Test Value"));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(string)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(BasicClass)));

        data.Setter(instance, "New Value");

        Assert.That(instance.Test, Is.EqualTo("New Value"));
    }

    [Test]
    public void BasicNonStringProperty()
    {
        var instance = new BasicClass { Test = "Test Value", Id = 1 };
        Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("Id"));
        Assert.That(data.Getter(instance), Is.EqualTo(1));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(int)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(BasicClass)));

        data.Setter(instance, 2);

        Assert.That(instance.Id, Is.EqualTo(2));
    }

    [Test]
    public void NestedProperty()
    {
        var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
        Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("Basic.Test"));
        Assert.That(data.Getter(instance), Is.EqualTo("Test Value"));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(string)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(ParentClass)));

        data.Setter(instance, "New Value");

        Assert.That(instance.Basic.Test, Is.EqualTo("New Value"));
    }

    [Test]
    public void NestedNonStringProperty()
    {
        var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value", Id = 1 } };
        Expression<Func<ParentClass, int>> func = (ParentClass x) => x.Basic.Id;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("Basic.Id"));
        Assert.That(data.Getter(instance), Is.EqualTo(1));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(int)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(ParentClass)));

        data.Setter(instance, 2);

        Assert.That(instance.Basic.Id, Is.EqualTo(2));
    }

    [Test]
    public void BasicStringExpression()
    {
        var instance = new BasicClass { Test = "Test Value" };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => $"{x.Test} - Blub";

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo("Test Value - Blub"));

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData, Is.Not.Null);
        Assert.That(propertyData as IFullPropertyMetadata, Is.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("x => Format(\"{0} - Blub\", x.Test)"));
        Assert.That(propertyData.Getter(instance), Is.EqualTo("Test Value - Blub"));
        Assert.That(propertyData.PropertyType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void BasicNullStringExpression()
    {
        var instance = new BasicClass { Test = null };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test;

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo(""));

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData as IFullPropertyMetadata, Is.Not.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("Test"));
        Assert.That(propertyData.Getter(instance), Is.EqualTo(null));
        Assert.That(propertyData.PropertyType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void BasicNullNullableStringExpression()
    {
        var instance = new BasicClass { NullableTest = null };
        Expression<Func<BasicClass, string?>> func = (BasicClass x) => x.NullableTest;

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo(string.Empty));
    }

    [Test]
    public void BasicNotNullNullableStringExpression()
    {
        var instance = new BasicClass { NullableTest = "test" };
        Expression<Func<BasicClass, string?>> func = (BasicClass x) => x.NullableTest;

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo("test"));
    }

    [Test]
    public void BasicStringExpression2()
    {
        var instance = new BasicClass { Test = "Test Value" };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => $"Blaat";

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo("Blaat"));

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData, Is.Not.Null);
        Assert.That(propertyData as IFullPropertyMetadata, Is.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("x => \"Blaat\""));
        Assert.That(propertyData.Getter(instance), Is.EqualTo("Blaat"));
        Assert.That(propertyData.PropertyType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void BasicStringExpression3()
    {
        var instance = new BasicClass { Test = "Test Value" };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => string.Join(' ', x.Test.ToCharArray());

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo("T e s t   V a l u e"));

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData, Is.Not.Null);
        Assert.That(propertyData as IFullPropertyMetadata, Is.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("x => Join( , x.Test.ToCharArray())"));
        Assert.That(propertyData.Getter(instance), Is.EqualTo("T e s t   V a l u e"));
    }

    [Test]
    public void BasicStringExpressionConverted()
    {
        var instance = new BasicClass { Test = "Test Value" };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => string.Join(' ', x.Test.ToCharArray());

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData, Is.Not.Null);
        Assert.That(propertyData as IFullPropertyMetadata, Is.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("x => Join( , x.Test.ToCharArray())"));
        Assert.That(propertyData.Getter(instance), Is.EqualTo("T e s t   V a l u e"));

        var data = PropertyMetadataHelper.GetExpressionMetadata(propertyData);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo("T e s t   V a l u e"));
    }

    [Test]
    public void BasicNonStringExpression()
    {
        var instance = new BasicClass { Test = "Test Value", Id = 3 };
        Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Null);

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData, Is.Not.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("Id"));
        Assert.That(propertyData.Getter(instance), Is.EqualTo(3));
        Assert.That(propertyData.PropertyType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void NestedStringExpression()
    {
        var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
        Expression<Func<ParentClass, string>> func = (ParentClass x) => $"{x.Basic} {x.Basic.Test}";

        var data = PropertyMetadataHelper.GetExpressionMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.StringGetter(instance), Is.EqualTo("RapidCMS.Core.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value"));

        var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(propertyData, Is.Not.Null);
        Assert.That(propertyData as IFullPropertyMetadata, Is.Null);
        Assert.That(propertyData.PropertyName, Is.EqualTo("x => Format(\"{0} {1}\", x.Basic, x.Basic.Test)"));
        Assert.That(propertyData.Getter(instance), Is.EqualTo("RapidCMS.Core.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value"));
        Assert.That(propertyData.PropertyType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void BasicReadonlyProperty()
    {
        var instance = new BasicClass { Test = "Test Value" };
        Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test.ToLower();

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("x => x.Test.ToLower()"));
        Assert.That(data.Getter(instance), Is.EqualTo("test value"));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(string)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(BasicClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Null);
    }

    [Test]
    public void BasicReadonlyNonStringProperty()
    {
        var instance = new BasicClass { Test = "Test Value", Id = 1 };
        Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id * 2;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("x => (x.Id * 2)"));
        Assert.That(data.Getter(instance), Is.EqualTo(2));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(int)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(BasicClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Null);
    }

    [Test]
    public void NestedReadonlyProperty()
    {
        var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
        Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test.ToLower();

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("x => x.Basic.Test.ToLower()"));
        Assert.That(data.Getter(instance), Is.EqualTo("test value"));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(string)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(ParentClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Null);
    }

    [Test]
    public void NestedReadonlyNonStringProperty()
    {
        var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value", Id = 1 } };
        Expression<Func<ParentClass, int>> func = (ParentClass x) => x.Basic.Id * 2;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("x => (x.Basic.Id * 2)"));
        Assert.That(data.Getter(instance), Is.EqualTo(2));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(int)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(ParentClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Null);
    }

    [Test]
    public void BasicNullableProperty()
    {
        var instance = new BasicClass { NullableTest = "Test Value" };
        Expression<Func<BasicClass, string?>> func = (BasicClass x) => x.NullableTest;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("NullableTest"));
        Assert.That(data.Getter(instance), Is.EqualTo("Test Value"));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(string)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(BasicClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Not.Null);
    }

    [Test]
    public void BasicNullableNonStringProperty()
    {
        var instance = new BasicClass { NullableTest = "Test Value", Id = 1 };
        Expression<Func<BasicClass, int?>> func = (BasicClass x) => x.NullableTest == null ? default(int?) : x.NullableTest.Length;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Getter(instance), Is.EqualTo(10));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(int?)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(BasicClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Null);
    }

    [Test]
    public void NestedNullableProperty()
    {
        var instance = new ParentClass { Basic = new BasicClass { NullableTest = "Test Value" } };
        Expression<Func<ParentClass, string?>> func = (ParentClass x) => x.Basic.NullableTest;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo("Basic.NullableTest"));
        Assert.That(data.Getter(instance), Is.EqualTo("Test Value"));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(string)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(ParentClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Not.Null);
    }

    [Test]
    public void NestedNullableNonStringProperty()
    {
        var instance = new ParentClass { Basic = new BasicClass { NullableTest = "Test Value", Id = 1 } };
        Expression<Func<ParentClass, int?>> func = (ParentClass x) => x.Basic == null ? default(int?) : x.Basic.NullableTest == null ? default : x.Basic.NullableTest.Length;

        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Getter(instance), Is.EqualTo(10));
        Assert.That(data.PropertyType, Is.EqualTo(typeof(int?)));
        Assert.That(data.ObjectType, Is.EqualTo(typeof(ParentClass)));

        Assert.That(data as IFullPropertyMetadata, Is.Null);
    }

    [Test]
    public void PropertyFingerprintDifference()
    {
        Expression<Func<ParentClass, int>> func1 = (ParentClass x) => x.Basic.Id * 2;
        var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics.Select(x => x.Id);
        var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

        Assert.That(data1.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.EqualTo(data1.Fingerprint));
    }

    [Test]
    public void PropertyFingerprintEquality()
    {
        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func1 = (ParentCollectionClass z) => z.Basics.Select(q => q.Id);
        var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics.Select(x => x.Id);
        var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

        Assert.That(data1.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.EqualTo(data1.Fingerprint));
    }

    [Test]
    public void PropertyFingerprintEquality2()
    {
        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func1 = (ParentCollectionClass z) => z.Basics.Select(q => q.Id);
        var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics2.Select(x => x.Id);
        var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

        Assert.That(data1.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.EqualTo(data1.Fingerprint));
    }

    [Test]
    public void PropertyFingerprintEquality3()
    {
        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func1 = (ParentCollectionClass z) => z.Basics.Select(q => q.Id);
        var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

        Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics.Select(x => 1);
        var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

        Assert.That(data1.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.EqualTo(data1.Fingerprint));
    }

    [Test]
    public void PropertyFingerprintEquality4()
    {
        Expression<Func<ParentClass, string>> func1 = (ParentClass z) => z.Basic.Test;
        var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

        var data2 = PropertyMetadataHelper.GetPropertyMetadata(typeof(ParentClass),
            new[] { typeof(ParentClass).GetProperty(nameof(ParentClass.Basic)) }, typeof(BasicClass).GetProperty(nameof(BasicClass.Test)));

        var data3 = PropertyMetadataHelper.GetPropertyMetadata(typeof(ParentClass), "Basic.Test");

        Assert.That(data1.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.Not.Null);
        Assert.That(data3.Fingerprint, Is.Not.Null);
        Assert.That(data2.Fingerprint, Is.EqualTo(data1.Fingerprint));
        Assert.That(data3.Fingerprint, Is.EqualTo(data2.Fingerprint));
        Assert.That(data3.Fingerprint, Is.EqualTo(data1.Fingerprint));
    }

    [Test]
    public void SelfAsProperty()
    {
        var instance = new RecursiveClass { Name = "Self" };

        Expression<Func<RecursiveClass, RecursiveClass>> func = x => x;
        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(((RecursiveClass)data.Getter(instance)).Name, Is.EqualTo("Self"));
    }

    [Test]
    public void ParentAsProperty()
    {
        var instance = new RecursiveClass { Name = "Self", Parent = new RecursiveClass { Name = "Parent" } };

        Expression<Func<RecursiveClass, RecursiveClass>> func = x => x.Parent;
        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(((RecursiveClass)data.Getter(instance)).Name, Is.EqualTo("Parent"));
    }

    [Test]
    public void ParentAsPropertyViaInterface()
    {
        IRecursiveClass instance = new RecursiveClass { Name = "Self", Parent = new RecursiveClass { Name = "Parent" } };

        Expression<Func<IRecursiveClass, IRecursiveClass>> func = x => x.Parent;
        var data = PropertyMetadataHelper.GetPropertyMetadata(func);

        Assert.That(data, Is.Not.Null);
        Assert.That(((RecursiveClass)data.Getter(instance)).Name, Is.EqualTo("Parent"));
    }

    [Test]
    public void PropertyFromProperty()
    {
        var expected = "string";
        var instance = new BasicClass { Test = expected };

        var property = typeof(BasicClass).GetProperty(nameof(BasicClass.Test));

        var data = PropertyMetadataHelper.GetPropertyMetadata(instance.GetType(), property);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo(nameof(BasicClass.Test)));
        Assert.That(data.Getter(instance), Is.EqualTo(expected));
    }

    [Test]
    public void PropertyFromProperties()
    {
        var expected = "string";
        var instance = new ParentClass { Basic = new BasicClass { Test = expected } };

        var property1 = typeof(ParentClass).GetProperty(nameof(ParentClass.Basic));
        var property2 = typeof(BasicClass).GetProperty(nameof(BasicClass.Test));

        var data = PropertyMetadataHelper.GetPropertyMetadata(instance.GetType(), new[] { property1 }, property2);

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo($"{nameof(ParentClass.Basic)}.{nameof(BasicClass.Test)}"));
        Assert.That(data.Getter(instance), Is.EqualTo(expected));
    }

    [Test]
    public void PropertyFromPropertyName()
    {
        var expected = "string";
        var instance = new BasicClass { Test = expected };

        var data = PropertyMetadataHelper.GetPropertyMetadata(instance.GetType(), "Test");

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo(nameof(BasicClass.Test)));
        Assert.That(data.Getter(instance), Is.EqualTo(expected));
    }

    [Test]
    public void PropertyFromNestedPropertyName()
    {
        var expected = "string";
        var instance = new ParentClass { Basic = new BasicClass { Test = expected } };

        var data = PropertyMetadataHelper.GetPropertyMetadata(instance.GetType(), "Basic.Test");

        Assert.That(data, Is.Not.Null);
        Assert.That(data.PropertyName, Is.EqualTo($"{nameof(ParentClass.Basic)}.{nameof(BasicClass.Test)}"));
        Assert.That(data.Getter(instance), Is.EqualTo(expected));
    }

    [Test]
    public void ReferenceTypePropertyCanHandleNull()
    {
        var instance = new ReferenceType { Data = "x" };

        var dataProperty = (IFullPropertyMetadata)PropertyMetadataHelper.GetPropertyMetadata<ReferenceType, string>(x => x.Data);

        Assert.That(dataProperty, Is.Not.Null);
        Assert.DoesNotThrow(() => dataProperty.Setter(instance, null));
        Assert.That(instance.Data, Is.Null);
    }

    [Test]
    public void ValueTypePropertyCanHandleNull()
    {
        var instance = new ValueType { Data = 1 };

        var dataProperty = (IFullPropertyMetadata)PropertyMetadataHelper.GetPropertyMetadata<ValueType, int>(x => x.Data);

        Assert.That(dataProperty, Is.Not.Null);
        Assert.DoesNotThrow(() => dataProperty.Setter(instance, null));
        Assert.That(instance.Data, Is.EqualTo(0));
    }

    [Test]
    public void NullableValueTypePropertyCanHandleNull()
    {
        var instance = new NullableValueType { Data = 1 };

        var dataProperty = (IFullPropertyMetadata)PropertyMetadataHelper.GetPropertyMetadata<NullableValueType, int?>(x => x.Data);

        Assert.That(dataProperty, Is.Not.Null);
        Assert.DoesNotThrow(() => dataProperty.Setter(instance, null));
        Assert.That(instance.Data, Is.Null);
    }

    class BasicClass
    {
        public string Test { get; set; }
        public int Id { get; set; }
        public string? NullableTest { get; set; }
    }


    class ParentClass
    {
        public BasicClass Basic { get; set; }
    }

    interface IRecursiveClass
    {
        public IRecursiveClass? Parent { get; }
    }

    class RecursiveClass : IRecursiveClass
    {
        public RecursiveClass? Parent { get; set; }
        public string Name { get; set; }
        public string? NullableTest { get; set; }

        IRecursiveClass IRecursiveClass.Parent => Parent;
    }

    class ParentCollectionClass
    {
        public ICollection<BasicClass> Basics { get; set; }
        public ICollection<BasicClass> Basics2 { get; set; }
    }

    class ReferenceType
    {
        public string Data { get; set; }
    }

    class ValueType
    {
        public int Data { get; set; }
    }

    class NullableValueType
    {
        public int? Data { get; set; }
    }
}
