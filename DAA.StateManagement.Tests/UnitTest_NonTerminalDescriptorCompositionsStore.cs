using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.DataManagement;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_NonTerminalDescriptorCompositionsStore
    {
        private NonTerminalDescriptorCompositionsStore TestInstance { get => TestInstanceMock.Object; }
        private Mock<NonTerminalDescriptorCompositionsStore> TestInstanceMock { get; set; }
        private IProtectedMock<NonTerminalDescriptorCompositionsStore> TestInstanceMockProtected { get => TestInstanceMock.Protected(); }

        private INonTerminalDescriptor Descriptor { get => DescriptorMock.Object; }
        private Mock<INonTerminalDescriptor> DescriptorMock { get; set; }

        private IEnumerable<ITerminalDescriptor> Composition { get => CompositionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> CompositionMock { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            DescriptorMock = new Mock<INonTerminalDescriptor>();
            CompositionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            TestInstanceMock = new Mock<NonTerminalDescriptorCompositionsStore>();
            TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void Update__NewCompositionSet()
        {
            TestInstanceMockProtected
                .Setup("Set", Descriptor, Composition)
                .Verifiable();

            TestInstance.Update(Descriptor, Composition);

            TestInstanceMock.Verify();
        }


        [TestMethod]
        public void UpdateAndProvideAdditions__AdditionsComputedBeforeUpdating()
        {
            var callCounter = 0;
            var CompareToInitialCompositionAndFindAdditionsCallNumber = 0;
            var updateCallNumber = 0;

            TestInstanceMockProtected
                .Setup("CompareToInitialCompositionAndFindAdditions", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Callback(() => CompareToInitialCompositionAndFindAdditionsCallNumber = ++callCounter);
            TestInstanceMock
                .Setup(_ => _.Update(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()))
                .Callback(() => updateCallNumber = ++callCounter);

            TestInstance.UpdateAndProvideAdditions(Descriptor, Composition);

            if (0 != updateCallNumber)
            {
                Assert.IsTrue(CompareToInitialCompositionAndFindAdditionsCallNumber <= updateCallNumber);
            }
        }

        [TestMethod]
        public void UpdateAndProvideAdditions__AdditionsCorrectlyComputedAndResultProxied()
        {
            var additions = new Mock<IEnumerable<ITerminalDescriptor>>().Object;

            TestInstanceMock
                .Setup(_ => _.Update(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()));
            TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("CompareToInitialCompositionAndFindAdditions", Descriptor, Composition)
                .Returns(additions)
                .Verifiable();

            var result = TestInstance.UpdateAndProvideAdditions(Descriptor, Composition);

            TestInstanceMock.Verify();
            Assert.AreSame(additions, result);
        }

        [TestMethod]
        public void UpdateAndProvideAdditions__CompositionUpdated()
        {
            TestInstanceMockProtected
                .Setup("CompareToInitialCompositionAndFindAdditions", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());
            TestInstanceMock
                .Setup(_ => _.Update(Descriptor, Composition))
                .Verifiable();

            TestInstance.UpdateAndProvideAdditions(Descriptor, Composition);

            TestInstanceMock.Verify();
        }


        [TestMethod]
        public void CompareToInitialCompositionAndFindAdditions_Descriptor_ReturnsAdditionsOfTheNewCompositionOverTheInitialOne()
        {
            var initialComposition = new Mock<IEnumerable<ITerminalDescriptor>>().Object;
            var newComposition = new Mock<IEnumerable<ITerminalDescriptor>>().Object;
            var additions = new Mock<IEnumerable<ITerminalDescriptor>>().Object;

            TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("FindDistinctAdditions", initialComposition, newComposition)
                .Returns(additions)
                .Verifiable();

            TestInstanceMock
                .Setup(_ => _.Retrieve(Descriptor))
                .Returns(initialComposition)
                .Verifiable();

            var result = ReflectionHelper.Invoke(TestInstance, "CompareToInitialCompositionAndFindAdditions", Descriptor, newComposition);

            Assert.AreSame(additions, result);
        }


        [TestMethod]
        public void FindDistinctAdditions_InitialCompositionEmpty_EquivalentToNewComposition()
        {
            var initialComposition = new ITerminalDescriptor[0];
            var newComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);

            var result = ReflectionHelper.Invoke(TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(newComposition.Equivalent(result));
        }

        [TestMethod]
        public void FindDistinctAdditions_NewCompositionEmpty_EmptyCollection()
        {
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition= new ITerminalDescriptor[0];

            var result = ReflectionHelper.Invoke(TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void FindDistinctAdditions_CompositionsIntersecting_OnlyAdditions()
        {
            var intersection = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var additions = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var initialComposition = ArraysHelper.CreateWithContent(intersection[0], new Mock<ITerminalDescriptor>().Object, intersection[1]);
            var newComposition = ArraysHelper.CreateWithContent(additions[0], intersection[1], additions[1], intersection[0]);

            var result = ReflectionHelper.Invoke(TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(additions.Equivalent(result));
        }

        [TestMethod]
        public void FindDistinctAdditions_CompositionsDisjunct_EquivalentToNewComposition()
        {
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);

            var result = ReflectionHelper.Invoke(TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(newComposition.Equivalent(result));
        }

        [TestMethod]
        public void FindDistinctAdditions_NewCompositionsSubsetOfInitialComposition_EmptyCollection()
        {
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition = ArraysHelper.CreateWithContent(initialComposition[0], initialComposition[2]);

            var result = ReflectionHelper.Invoke(TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void FindDistinctAdditions_NewCompositionHasDuplicatedAdditions_Distinct()
        {
            var additions = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition = ArraysHelper.CreateWithContent(additions[0], additions[1], additions[1], additions[0]);

            var result = ReflectionHelper.Invoke(TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(additions.Equivalent(result));
        }
    }
}
