using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util.Reflection;
using System;
using System.Diagnostics;
using System.Linq;

namespace UtilTests.Reflection
{
    [TestClass]
    public class PropertiesCacheTests
    {
        public class TestClass1
        {
            public string? Property1 { get; set; }
            public int Property2 { get; }
            public bool Property3 { private get; set; }

            private string? Property4 { get; set; }
        }

        public class TestClass2
        {
            public string? Property1 { get; set; }
            public string? Property2 { get; set; }
            public string? Property3 { get; set; }
            public string? Property4 { get; set; }
            public string? Property5 { get; set; }

            public string? Property6 { get; set; }
            public string? Property7 { get; set; }
            public string? Property8 { get; set; }
            public string? Property9 { get; set; }
            public string? Property0 { get; set; }

            public string? Property11 { get; set; }
            public string? Property12 { get; set; }
            public string? Property13 { get; set; }
            public string? Property14 { get; set; }
            public string? Property15 { get; set; }

            public string? Property16 { get; set; }
            public string? Property17 { get; set; }
            public string? Property18 { get; set; }
            public string? Property19 { get; set; }
            public string? Property20 { get; set; }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PropertiesCache_GetPropertiesOf_TypeIsNull_ShouldThrowArgumentNullException()
        {
            var cache = new PropertiesCache();

            cache.GetPropertiesOf(null!);
        }

        [TestMethod]
        public void PropertiesCache_GetPropertiesOf_ShouldReturnCorrectNumberOfProperties()
        {
            var cache = new PropertiesCache();

            var properties = cache.GetPropertiesOf<TestClass1>();
            Assert.AreEqual(3, properties.Length);
        }

        [TestMethod]
        public void PropertiesCache_GetPropertiesOf_ResultShouldEqualTypeGetProperties()
        {
            var cache = new PropertiesCache();

            var cachedProperties = cache.GetPropertiesOf<TestClass1>();
            var typeProperties = typeof(TestClass1).GetProperties();

            Assert.AreEqual(typeProperties.Length, cachedProperties.Length);
            foreach (var typeProperty in typeProperties)
            {
                Assert.IsTrue(cachedProperties.Contains(typeProperty));
            }
        }

        [TestMethod]
        public void PropertiesCache_GetPropertiesOf_ShouldReturnCorrectProperties()
        {
            var cache = new PropertiesCache();

            var properties = cache.GetPropertiesOf<TestClass1>();
            Assert.IsTrue(properties.Any(p => p.Name == "Property1"));
            Assert.IsTrue(properties.Any(p => p.Name == "Property2"));
            Assert.IsTrue(properties.Any(p => p.Name == "Property3"));
        }

        [TestMethod]
        public void PropertiesCache_GetPropertiesOf_ShouldCacheProperties()
        {
            var stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (var i = 0; i < 10000000; i++)
            {
                typeof(TestClass2).GetProperties();
            }
            stopwatch1.Stop();

            var cache2 = new PropertiesCache();
            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            for (var i = 0; i < 10000000; i++)
            {
                cache2.GetPropertiesOf<TestClass2>();
            }
            stopwatch2.Stop();

            Console.WriteLine(stopwatch1.Elapsed);
            Console.WriteLine(stopwatch2.Elapsed);
            Assert.IsTrue(stopwatch1.Elapsed > stopwatch2.Elapsed);
        }
    }
}
