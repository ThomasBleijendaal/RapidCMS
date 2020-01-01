using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models;

namespace RapidCMS.Core.Tests.PropertyMetadata
{
    public class PropertyMetadataTests
    {
        [Test]
        public void BasicProperty()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("Test", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            data.Setter(instance, "New Value");

            Assert.AreEqual("New Value", instance.Test);
        }

        [Test]
        public void BasicNonStringProperty()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 1 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("Id", data.PropertyName);
            Assert.AreEqual(1, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            data.Setter(instance, 2);

            Assert.AreEqual(2, instance.Id);
        }

        [Test]
        public void NestedProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("BasicTest", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            data.Setter(instance, "New Value");

            Assert.AreEqual("New Value", instance.Basic.Test);
        }

        [Test]
        public void NestedNonStringProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value", Id = 1 } };
            Expression<Func<ParentClass, int>> func = (ParentClass x) => x.Basic.Id;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("BasicId", data.PropertyName);
            Assert.AreEqual(1, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            data.Setter(instance, 2);

            Assert.AreEqual(2, instance.Basic.Id);
        }

        [Test]
        public void BasicStringExpression()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => $"{x.Test} - Blub";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Test Value - Blub", data.StringGetter(instance));

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Format(\"{0} - Blub\", x.Test)", propertyData.PropertyName);
            Assert.AreEqual("Test Value - Blub", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), propertyData.PropertyType);
        }

        [Test]
        public void BasicNullStringExpression()
        {
            var instance = new BasicClass { Test = null };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test;

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("", data.StringGetter(instance));

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("Test", propertyData.PropertyName);
            Assert.AreEqual(null, propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), propertyData.PropertyType);
        }

        [Test]
        public void BasicNullNullableStringExpression()
        {
            var instance = new BasicClass { NullableTest = null };
            Expression<Func<BasicClass, string?>> func = (BasicClass x) => x.NullableTest;

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual(string.Empty, data.StringGetter(instance));
        }

        [Test]
        public void BasicNotNullNullableStringExpression()
        {
            var instance = new BasicClass { NullableTest = "test" };
            Expression<Func<BasicClass, string?>> func = (BasicClass x) => x.NullableTest;

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("test", data.StringGetter(instance));
        }

        [Test]
        public void BasicStringExpression2()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => $"Blaat";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Blaat", data.StringGetter(instance));

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => (\"Blaat\" ?? \"\")", propertyData.PropertyName);
            Assert.AreEqual("Blaat", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), propertyData.PropertyType);
        }

        [Test]
        public void BasicStringExpression3()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => string.Join(' ', x.Test.ToCharArray());

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("T e s t   V a l u e", data.StringGetter(instance));

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Join( , x.Test.ToCharArray())", propertyData.PropertyName);
            Assert.AreEqual("T e s t   V a l u e", propertyData.Getter(instance));
        }

        [Test]
        public void BasicStringExpressionConverted()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => string.Join(' ', x.Test.ToCharArray());

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Join( , x.Test.ToCharArray())", propertyData.PropertyName);
            Assert.AreEqual("T e s t   V a l u e", propertyData.Getter(instance));

            var data = PropertyMetadataHelper.GetExpressionMetadata(propertyData);

            Assert.IsNotNull(data);
            Assert.AreEqual("T e s t   V a l u e", data.StringGetter(instance));
        }

        [Test]
        public void BasicNonStringExpression()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 3 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNull(data);

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.AreEqual("Id", propertyData.PropertyName);
            Assert.AreEqual(3, propertyData.Getter(instance));
            Assert.AreEqual(typeof(int), propertyData.PropertyType);
        }

        [Test]
        public void NestedStringExpression()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => $"{x.Basic} {x.Basic.Test}";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("RapidCMS.Core.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value", data.StringGetter(instance));

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Format(\"{0} {1}\", x.Basic, x.Basic.Test)", propertyData.PropertyName);
            Assert.AreEqual("RapidCMS.Core.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), propertyData.PropertyType);
        }

        [Test]
        public void BasicReadonlyProperty()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test.ToLower();

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => x.Test.ToLower()", data.PropertyName);
            Assert.AreEqual("test value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void BasicReadonlyNonStringProperty()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 1 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id * 2;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => (x.Id * 2)", data.PropertyName);
            Assert.AreEqual(2, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void NestedReadonlyProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test.ToLower();

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => x.Basic.Test.ToLower()", data.PropertyName);
            Assert.AreEqual("test value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void NestedReadonlyNonStringProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value", Id = 1 } };
            Expression<Func<ParentClass, int>> func = (ParentClass x) => x.Basic.Id * 2;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => (x.Basic.Id * 2)", data.PropertyName);
            Assert.AreEqual(2, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void BasicNullableProperty()
        {
            var instance = new BasicClass { NullableTest = "Test Value" };
            Expression<Func<BasicClass, string?>> func = (BasicClass x) => x.NullableTest;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("NullableTest", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            Assert.IsNotNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void BasicNullableNonStringProperty()
        {
            var instance = new BasicClass { NullableTest = "Test Value", Id = 1 };
            Expression<Func<BasicClass, int?>> func = (BasicClass x) => x.NullableTest == null ? default(int?) : x.NullableTest.Length;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual(10, data.Getter(instance));
            Assert.AreEqual(typeof(int?), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void NestedNullableProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { NullableTest = "Test Value" } };
            Expression<Func<ParentClass, string?>> func = (ParentClass x) => x.Basic.NullableTest;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("BasicNullableTest", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.IsNotNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void NestedNullableNonStringProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { NullableTest = "Test Value", Id = 1 } };
            Expression<Func<ParentClass, int?>> func = (ParentClass x) => x.Basic == null ? default(int?) : x.Basic.NullableTest == null ? default : x.Basic.NullableTest.Length;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual(10, data.Getter(instance));
            Assert.AreEqual(typeof(int?), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void PropertyFingerprintDifference()
        {
            Expression<Func<ParentClass, int>> func1 = (ParentClass x) => x.Basic.Id * 2;
            var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics.Select(x => x.Id);
            var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

            Assert.IsNotNull(data1.Fingerprint);
            Assert.IsNotNull(data2.Fingerprint);
            Assert.AreNotEqual(data1.Fingerprint, data2.Fingerprint);
        }

        [Test]
        public void PropertyFingerprintEquality()
        {
            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func1 = (ParentCollectionClass z) => z.Basics.Select(q => q.Id);
            var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics.Select(x => x.Id);
            var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

            Assert.IsNotNull(data1.Fingerprint);
            Assert.IsNotNull(data2.Fingerprint);
            Assert.AreEqual(data1.Fingerprint, data2.Fingerprint);
        }

        [Test]
        public void PropertyFingerprintEquality2()
        {
            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func1 = (ParentCollectionClass z) => z.Basics.Select(q => q.Id);
            var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics2.Select(x => x.Id);
            var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

            Assert.IsNotNull(data1.Fingerprint);
            Assert.IsNotNull(data2.Fingerprint);
            Assert.AreNotEqual(data1.Fingerprint, data2.Fingerprint);
        }

        [Test]
        public void PropertyFingerprintEquality3()
        {
            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func1 = (ParentCollectionClass z) => z.Basics.Select(q => q.Id);
            var data1 = PropertyMetadataHelper.GetPropertyMetadata(func1);

            Expression<Func<ParentCollectionClass, IEnumerable<int>>> func2 = (ParentCollectionClass x) => x.Basics.Select(x => 1);
            var data2 = PropertyMetadataHelper.GetPropertyMetadata(func2);

            Assert.IsNotNull(data1.Fingerprint);
            Assert.IsNotNull(data2.Fingerprint);
            Assert.AreNotEqual(data1.Fingerprint, data2.Fingerprint);
        }

        [Test]
        public void SelfAsProperty()
        {
            var instance = new RecursiveClass { Name = "Self" };

            Expression<Func<RecursiveClass, RecursiveClass>> func = x => x;
            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Self", ((RecursiveClass)data.Getter(instance)).Name);
        }

        [Test]
        public void ParentAsProperty()
        {
            var instance = new RecursiveClass { Name = "Self", Parent = new RecursiveClass { Name = "Parent" } };

            Expression<Func<RecursiveClass, RecursiveClass>> func = x => x.Parent;
            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Parent", ((RecursiveClass)data.Getter(instance)).Name);
        }

        [Test]
        public void ParentAsPropertyViaInterface()
        {
            IRecursiveClass instance = new RecursiveClass { Name = "Self", Parent = new RecursiveClass { Name = "Parent" } };

            Expression<Func<IRecursiveClass, IRecursiveClass>> func = x => x.Parent;
            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Parent", ((RecursiveClass)data.Getter(instance)).Name);
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
    }
}
