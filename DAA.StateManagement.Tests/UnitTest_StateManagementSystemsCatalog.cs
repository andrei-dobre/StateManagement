using System;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_StateManagementSystemsCatalog
    {
        private IStateManagementSystem<IData> StateManagementSystem { get; set; }
        private Mock<IStateManagementSystem<IData>> MockedStateManagementSystem { get; set; }

        private StateManagementSystemsCatalog TestInstance => MockedTestInstance.Object;
        private Mock<StateManagementSystemsCatalog> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedStateManagementSystem = new Mock<IStateManagementSystem<IData>>();

            MockedTestInstance = new Mock<StateManagementSystemsCatalog>();
            MockedTestInstance.CallBase = true;
        }


        [TestMethod]
        public void Retrieve_NotRegistered_InvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() => TestInstance.Retrieve<IData>());
        }

        [TestMethod]
        public void Retrieve_Registered_Provided()
        {
            TestInstance.Register(StateManagementSystem);

            Assert.AreSame(StateManagementSystem, TestInstance.Retrieve<IData>());
        }

        [TestMethod]
        public void Register_AlreadyRegistered_InvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                TestInstance.Register(StateManagementSystem);
                TestInstance.Register(StateManagementSystem);
            });
        }
    }
}
