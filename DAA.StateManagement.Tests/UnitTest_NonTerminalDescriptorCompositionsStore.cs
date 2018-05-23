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
        private NonTerminalDescriptorCompositionsStore TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<NonTerminalDescriptorCompositionsStore> TestInstanceMock { get; set; }
        private IProtectedMock<NonTerminalDescriptorCompositionsStore> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }

        private INonTerminalDescriptor Descriptor { get => this.DescriptorMock.Object; }
        private Mock<INonTerminalDescriptor> DescriptorMock { get; set; }

        private IEnumerable<ITerminalDescriptor> Composition { get => this.CompositionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> CompositionMock { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DescriptorMock = new Mock<INonTerminalDescriptor>();
            this.CompositionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            this.TestInstanceMock = new Mock<NonTerminalDescriptorCompositionsStore>();
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void Update__NewCompositionSet()
        {
            this.TestInstanceMockProtected
                .Setup("Set", this.Descriptor, this.Composition)
                .Verifiable();

            this.TestInstance.Update(this.Descriptor, this.Composition);

            this.TestInstanceMock.Verify();
        }


        [TestMethod]
        public void UpdateAndProvideAdditions__AdditionsComputedBeforeUpdating()
        {
            var callCounter = 0;
            var CompareToInitialCompositionAndFindAdditionsCallNumber = 0;
            var updateCallNumber = 0;

            this.TestInstanceMockProtected
                .Setup("CompareToInitialCompositionAndFindAdditions", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Callback(() => CompareToInitialCompositionAndFindAdditionsCallNumber = ++callCounter);
            this.TestInstanceMock
                .Setup(_ => _.Update(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()))
                .Callback(() => updateCallNumber = ++callCounter);

            this.TestInstance.UpdateAndProvideAdditions(this.Descriptor, this.Composition);

            if (0 != updateCallNumber)
            {
                Assert.IsTrue(CompareToInitialCompositionAndFindAdditionsCallNumber <= updateCallNumber);
            }
        }

        [TestMethod]
        public void UpdateAndProvideAdditions__AdditionsCorrectlyComputedAndResultProxied()
        {
            var additions = new Mock<IEnumerable<ITerminalDescriptor>>().Object;

            this.TestInstanceMock
                .Setup(_ => _.Update(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()));
            this.TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("CompareToInitialCompositionAndFindAdditions", this.Descriptor, this.Composition)
                .Returns(additions)
                .Verifiable();

            var result = this.TestInstance.UpdateAndProvideAdditions(this.Descriptor, this.Composition);

            this.TestInstanceMock.Verify();
            Assert.AreSame(additions, result);
        }

        [TestMethod]
        public void UpdateAndProvideAdditions__CompositionUpdated()
        {
            this.TestInstanceMockProtected
                .Setup("CompareToInitialCompositionAndFindAdditions", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());
            this.TestInstanceMock
                .Setup(_ => _.Update(this.Descriptor, this.Composition))
                .Verifiable();

            this.TestInstance.UpdateAndProvideAdditions(this.Descriptor, this.Composition);

            this.TestInstanceMock.Verify();
        }


        [TestMethod]
        public void CompareToInitialCompositionAndFindAdditions_Descriptor_ReturnsAdditionsOfTheNewCompositionOverTheInitialOne()
        {
            var initialComposition = new Mock<IEnumerable<ITerminalDescriptor>>().Object;
            var newComposition = new Mock<IEnumerable<ITerminalDescriptor>>().Object;
            var additions = new Mock<IEnumerable<ITerminalDescriptor>>().Object;

            this.TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("FindDistinctAdditions", initialComposition, newComposition)
                .Returns(additions)
                .Verifiable();

            this.TestInstanceMock
                .Setup(_ => _.Retrieve(this.Descriptor))
                .Returns(initialComposition)
                .Verifiable();

            var result = ReflectionHelper.Invoke(this.TestInstance, "CompareToInitialCompositionAndFindAdditions", this.Descriptor, newComposition);

            Assert.AreSame(additions, result);
        }


        [TestMethod]
        public void FindDistinctAdditions_InitialCompositionEmpty_EquivalentToNewComposition()
        {
            var initialComposition = new ITerminalDescriptor[0];
            var newComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);

            var result = ReflectionHelper.Invoke(this.TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(newComposition.Equivalent(result));
        }

        [TestMethod]
        public void FindDistinctAdditions_NewCompositionEmpty_EmptyCollection()
        {
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition= new ITerminalDescriptor[0];

            var result = ReflectionHelper.Invoke(this.TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
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

            var result = ReflectionHelper.Invoke(this.TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(additions.Equivalent(result));
        }

        [TestMethod]
        public void FindDistinctAdditions_CompositionsDisjunct_EquivalentToNewComposition()
        {
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);

            var result = ReflectionHelper.Invoke(this.TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(newComposition.Equivalent(result));
        }

        [TestMethod]
        public void FindDistinctAdditions_NewCompositionsSubsetOfInitialComposition_EmptyCollection()
        {
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition = ArraysHelper.CreateWithContent(initialComposition[0], initialComposition[2]);

            var result = ReflectionHelper.Invoke(this.TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void FindDistinctAdditions_NewCompositionHasDuplicatedAdditions_Distinct()
        {
            var additions = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var initialComposition = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);
            var newComposition = ArraysHelper.CreateWithContent(additions[0], additions[1], additions[1], additions[0]);

            var result = ReflectionHelper.Invoke(this.TestInstance, "FindDistinctAdditions", initialComposition, newComposition)
                            as IEnumerable<ITerminalDescriptor>;

            Assert.IsTrue(additions.Equivalent(result));
        }
    }
}
