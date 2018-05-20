using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using DAA.Helpers;
using DAA.StateManagement.DataManagement;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_Store
    {
        private Store<object, object> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<Store<object, object>> TestInstanceMock { get; set; }

        private object Value { get; set; }
        private object Key { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            this.Value = new object();
            this.Key = new object();

            this.TestInstanceMock = new Mock<Store<object, object>>();
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void KeyToValueMap__NotNull_Constant()
        {
            var testInstanceMock = new Mock<Store<object, object>>();
            var testInstance = testInstanceMock.Object;

            var resultOne = ReflectionHelper.Invoke(testInstance, "KeyToValueMap");
            var resultTwo = ReflectionHelper.Invoke(testInstance, "KeyToValueMap");

            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreSame(resultOne, resultTwo);
        }

        [TestMethod]
        public void Save_DescriptorNotContained_Inserted()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(false);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Insert(this.Key, this.Value), Times.Once());
        }

        [TestMethod]
        public void Save_DescriptorNotContained_NotUpdated()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(false);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Update(this.Key, this.Value), Times.Never());
        }

        [TestMethod]
        public void Save_DescriptorContained_Updated()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(true);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Update(this.Key, this.Value), Times.Once());
        }

        [TestMethod]
        public void Save_DescriptorContained_NotInserted()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(true);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Insert(this.Key, this.Value), Times.Never());
        }


        [TestMethod]
        public void Insert_DescriptorNotContained_CanRetrieveData()
        {
            this.TestInstance.Insert(this.Key, this.Value);

            var result = this.TestInstance.Retrieve(this.Key);

            Assert.AreSame(result, this.Value);
        }

        [TestMethod]
        public void Insert__DescriptorNotContained_OtherDescriptorContained__CanRetrieveData()
        {
            var terminalDescriptor = new Mock<object>().Object;
            var data = new Mock<object>().Object;

            this.TestInstance.Insert(terminalDescriptor, data);
            this.TestInstance.Insert(this.Key, this.Value);

            var resultOne = this.TestInstance.Retrieve(this.Key);
            var resultTwo = this.TestInstance.Retrieve(terminalDescriptor);

            Assert.AreSame(resultOne, this.Value);
            Assert.AreSame(resultTwo, data);
        }

        [TestMethod]
        public void Insert_DescriptorContained_InvalidOperationException()
        {
            this.TestInstance.Insert(this.Key, this.Value);

            Assert.ThrowsException<InvalidOperationException>(() => this.TestInstance.Insert(this.Key, this.Value));
        }

        [TestMethod]
        public void Insert_NullData_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.TestInstance.Insert(this.Key, null));
        }


        [TestMethod]
        public void Contains_NotInserted_False()
        {
            var result = this.TestInstance.Contains(this.Key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Contains_Inserted_True()
        {
            this.TestInstance.Insert(this.Key, this.Value);

            var result = this.TestInstance.Contains(this.Key);

            Assert.IsTrue(result);
        }
    }
}
