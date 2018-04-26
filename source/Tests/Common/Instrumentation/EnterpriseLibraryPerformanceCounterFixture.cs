// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnterpriseLibrary.Common.Instrumentation.Tests
{
    [TestClass]
    public class EnterpriseLibraryPerformanceCounterFixture
    {
        public static readonly string counterCategoryName = "TestCategory";
        public static readonly string counterName = "TestCounter";

        EnterpriseLibraryPerformanceCounterFactory counterFactory;

        [TestInitialize]
        public void CreateCounterFactory()
        {
            counterFactory = new EnterpriseLibraryPerformanceCounterFactory();

            try
            {
                counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total" });
            }
            catch (InvalidOperationException)
            {
                Assert.Inconclusive("In order to run the test, please run RegAssemblies.bat script first as an Administrator.");
            }
        }

        [TestMethod]
        public void CreateAndClearCounter()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total" });
            counter.Clear();

            long expected = 0;
            Assert.AreEqual(expected, counter.Value);
        }

        [TestMethod]
        public void CounterCanBeIncremented()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total" });
            counter.Clear();
            counter.Increment();

            long expected = 1;
            Assert.AreEqual(expected, counter.Value);
        }

        [TestMethod]
        public void CanIncrementDifferentInstancesOfSameCounter()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "ctr1", "ctr2" });
            counter.Clear();
            counter.Increment();

            long expected = 1;
            Assert.AreEqual(expected, counter.GetValueFor("ctr1"));
            Assert.AreEqual(expected, counter.GetValueFor("ctr2"));
        }

        [TestMethod]
        public void IncrementingDifferentCounterInstancesCausesBaseToIncrementSeparatelyFromChildCounters()
        {
            EnterpriseLibraryPerformanceCounter counter1 = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Base", "A" });
            EnterpriseLibraryPerformanceCounter counter2 = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Base", "B" });
            counter1.Clear();
            counter2.Clear();

            counter1.Increment();
            counter2.Increment();

            long expected = 2;
            Assert.AreEqual(expected, counter1.GetValueFor("Base"));
            Assert.AreEqual(expected, counter2.GetValueFor("Base"));

            expected = 1;
            Assert.AreEqual(expected, counter1.GetValueFor("A"));
            Assert.AreEqual(expected, counter2.GetValueFor("B"));
        }

        [TestMethod]
        public void CounterCanBeIncrementedByAnArbitraryQuantity()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total" });
            counter.Clear();
            counter.IncrementBy(10);

            long expected = 10;
            Assert.AreEqual(expected, counter.Value);
        }

        [TestMethod]
        public void CanIncrementByAnArbitraryQuantityDifferentInstancesOfSameCounter()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "ctr1", "ctr2" });
            counter.Clear();
            counter.IncrementBy(10);

            long expected = 10;
            Assert.AreEqual(expected, counter.GetValueFor("ctr1"));
            Assert.AreEqual(expected, counter.GetValueFor("ctr2"));
        }

        [TestMethod]
        public void IncrementingDifferentCounterInstancesByAnArbitraryQuantityCausesBaseToIncrementSeparatelyFromChildCounters()
        {
            EnterpriseLibraryPerformanceCounter counter1 = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Base", "A" });
            EnterpriseLibraryPerformanceCounter counter2 = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Base", "B" });
            counter1.Clear();
            counter2.Clear();

            counter1.IncrementBy(10);
            counter2.IncrementBy(10);

            long expected = 20;
            Assert.AreEqual(expected, counter1.GetValueFor("Base"));
            Assert.AreEqual(expected, counter2.GetValueFor("Base"));

            expected = 10;
            Assert.AreEqual(expected, counter1.GetValueFor("A"));
            Assert.AreEqual(expected, counter2.GetValueFor("B"));
        }

        [TestMethod]
        public void CounterValueCanBeSet()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total" });
            counter.Clear();

            long expected = 10;

            counter.SetValueFor("Total", expected);

            Assert.AreEqual(expected, counter.GetValueFor("Total"));
        }

        [TestMethod]
        public void CounterValueSetForWrongNameHasNoEffect()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total" });
            counter.Clear();

            long expected = 0;

            counter.SetValueFor("wrong name", 1000);

            Assert.AreEqual(expected, counter.GetValueFor("Total"));
        }

        [TestMethod]
        public void CounterValueSetForInstanceNameDoesNotUpdateOtherInstances()
        {
            EnterpriseLibraryPerformanceCounter counter = counterFactory.CreateCounter(counterCategoryName, counterName, new string[] { "Total", "A" });
            counter.Clear();

            long expected = 10;

            counter.SetValueFor("Total", expected);

            Assert.AreEqual(expected, counter.GetValueFor("Total"));
            Assert.AreEqual(0L, counter.GetValueFor("A"));
        }
    }
}

