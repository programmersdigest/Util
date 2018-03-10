using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilTests {
    [TestClass]
    public class ComparableWeakReferenceTests {
        [TestMethod]
        public void ComparableWeakReference_Ctor_TargetIsNull_ShouldNotThrowException() {
            var reference = new ComparableWeakReference<object>(null);
            Assert.IsNotNull(reference);
        }

        [TestMethod]
        public void ComparableWeakReference_GetHashCode_TargetIsNull_HashCodeShouldBeZero() {
            var reference = new ComparableWeakReference<object>(null);
            Assert.AreEqual(0, reference.GetHashCode());
        }

        [TestMethod]
        public void ComparableWeakReference_GetHashCode_ValidTarget_HashCodeShouldBeTargetHashCode() {
            var target = new object();

            var reference = new ComparableWeakReference<object>(target);
            Assert.AreEqual(target.GetHashCode(), reference.GetHashCode());
        }

        [TestMethod]
        public void ComparableWeakReference_GetHashCode_TargetGetsGarbageCollected_HashCodeShouldNotChange() {
#if DEBUG
            Assert.Inconclusive("Please run test in configuration \"Release\". Otherwise Garbage Collection will not collect the reference target and the test will fail.");
#endif
            ComparableWeakReference<object> reference;
            int targetHashCode;

            {
                var target = new object();
                targetHashCode = target.GetHashCode();

                reference = new ComparableWeakReference<object>(target);
            }

            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();

            {
                Assert.IsFalse(reference.TryGetTarget(out var target));
            }

            Assert.AreEqual(targetHashCode, reference.GetHashCode());
        }

        [TestMethod]
        public void ComparableWeakReference_Equal_ShouldNotBeConsideredEqualToTarget() {
            var target = new object();

            var reference = new ComparableWeakReference<object>(target);
            Assert.AreNotEqual(target, reference);
            Assert.AreNotEqual(reference, target);
        }

        [TestMethod]
        public void ComparableWeakReference_UseInHashSet_MultipleReferencesSameTarget_ShouldBeConsideredDuplicates() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);
            var reference3 = new ComparableWeakReference<object>(target);

            var set = new HashSet<ComparableWeakReference<object>> {
                reference1,
                reference2,
                reference3
            };

            Assert.AreEqual(1, set.Count);
        }

        [TestMethod]
        public void ComparableWeakReference_UseInHashSet_MultipleReferencesDifferentTargets_ShouldBeConsideredDifferent() {
            var target1 = new object();
            var target2 = new object();
            var target3 = new object();

            var reference1 = new ComparableWeakReference<object>(target1);
            var reference2 = new ComparableWeakReference<object>(target2);
            var reference3 = new ComparableWeakReference<object>(target3);

            var set = new HashSet<ComparableWeakReference<object>> {
                reference1,
                reference2,
                reference3
            };

            Assert.AreEqual(3, set.Count);
        }

        [TestMethod]
        public void ComparableWeakReference_UseInHashSet_ContainsUsingDifferentReferenceToSameTarget_ShouldReturnTrue() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);

            var set = new HashSet<ComparableWeakReference<object>> {
                reference1
            };

            Assert.IsTrue(set.Contains(reference2));
        }

        [TestMethod]
        public void ComparableWeakReference_UseInHashSet_RemoveUsingDifferentReferenceToSameTarget_ShouldRemoveEntry() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);

            var set = new HashSet<ComparableWeakReference<object>> {
                reference1
            };

            set.Remove(reference2);

            Assert.AreEqual(0, set.Count);
        }

        [TestMethod]
        public void ComparableWeakReference_UseAsDictionaryKey_MultipleReferencesSameTarget_ShouldBeConsideredDuplicates() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);
            var reference3 = new ComparableWeakReference<object>(target);

            var dict = new Dictionary<ComparableWeakReference<object>, string> {
                [reference1] = "Ref 1",
                [reference2] = "Ref 2",
                [reference3] = "Ref 3"
            };

            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual("Ref 3", dict.First().Value);
        }

        [TestMethod]
        public void ComparableWeakReference_UseAsDictionaryKey_MultipleReferencesDifferentTargets_ShouldBeConsideredDifferent() {
            var target1 = new object();
            var target2 = new object();
            var target3 = new object();

            var reference1 = new ComparableWeakReference<object>(target1);
            var reference2 = new ComparableWeakReference<object>(target2);
            var reference3 = new ComparableWeakReference<object>(target3);

            var dict = new Dictionary<ComparableWeakReference<object>, string> {
                [reference1] = "Ref 1",
                [reference2] = "Ref 2",
                [reference3] = "Ref 3"
            };

            Assert.AreEqual(3, dict.Count);
        }

        [TestMethod]
        public void ComparableWeakReference_UseAsDictionaryKey_ContainsKeyUsingDifferentReferenceToSameTarget_ShouldReturnTrue() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);

            var dict = new Dictionary<ComparableWeakReference<object>, string> {
                [reference1] = "Ref 1"
            };

            Assert.IsTrue(dict.ContainsKey(reference2));
        }

        [TestMethod]
        public void ComparableWeakReference_UseAsDictionaryKey_GetEntryUsingDifferentReferenceToSameTarget_ShouldReturnEntry() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);

            var dict = new Dictionary<ComparableWeakReference<object>, string> {
                [reference1] = "Ref 1"
            };
            
            Assert.AreEqual("Ref 1", dict[reference2]);
        }

        [TestMethod]
        public void ComparableWeakReference_UseAsDictionaryKey_RemoveUsingDifferentReferenceToSameTarget_ShouldRemoveEntry() {
            var target = new object();

            var reference1 = new ComparableWeakReference<object>(target);
            var reference2 = new ComparableWeakReference<object>(target);

            var dict = new Dictionary<ComparableWeakReference<object>, string> {
                [reference1] = "Ref 1"
            };

            dict.Remove(reference2);

            Assert.AreEqual(0, dict.Count);
        }
    }
}
