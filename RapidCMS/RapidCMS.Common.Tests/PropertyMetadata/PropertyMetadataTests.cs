using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Tests.PropertyMetadata
{
    public class PropertyMetadataTests
    {
        [SetUp]
        public void Setup()
        {
        }

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
        public void BasicNonStringExpression()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 3 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("3", data.StringGetter(instance));

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
            Assert.AreEqual("RapidCMS.Common.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value", data.StringGetter(instance));

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Format(\"{0} {1}\", x.Basic, x.Basic.Test)", propertyData.PropertyName);
            Assert.AreEqual("RapidCMS.Common.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value", propertyData.Getter(instance));
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

        class BasicClass
        {
            public string Test { get; set; }
            public int Id { get; set; }
        }

        class ParentClass
        {
            public BasicClass Basic { get; set; }
        }

        class ParentCollectionClass
        {
            public ICollection<BasicClass> Basics { get; set; }
            public ICollection<BasicClass> Basics2 { get; set; }
        }
    }
}
