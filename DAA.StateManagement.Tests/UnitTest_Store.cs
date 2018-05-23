using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.DataManagement;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_Store
    {
        private Store<object, object> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<Store<object, object>> TestInstanceMock { get; set; }
        private IProtectedMock<Store<object, object>> TestInstanceMockProtected { get => TestInstanceMock.Protected(); }

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
        public void Save_KeyNotContained_Inserted()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(false);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Insert(this.Key, this.Value), Times.Once());
        }

        [TestMethod]
        public void Save_KeyNotContained_NotUpdated()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(false);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Update(this.Key, this.Value), Times.Never());
        }

        [TestMethod]
        public void Save_KeyContained_Updated()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(true);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Update(this.Key, this.Value), Times.Once());
        }

        [TestMethod]
        public void Save_KeyContained_NotInserted()
        {
            this.TestInstanceMock.Setup(_ => _.Insert(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            this.TestInstanceMock.Setup(_ => _.Contains(It.IsAny<object>())).Returns(true);

            this.TestInstance.Save(this.Key, this.Value);

            this.TestInstanceMock.Verify(_ => _.Insert(this.Key, this.Value), Times.Never());
        }


        [TestMethod]
        public void Insert_KeyNotContained_Set()
        {
            this.TestInstanceMockProtected
                .Setup("Set", this.Key, this.Value)
                .Verifiable();

            this.TestInstance.Insert(this.Key, this.Value);

            this.TestInstanceMock.Verify();
        }

        [TestMethod]
        public void Insert_KeyContained_InvalidOperationException()
        {
            this.TestInstance.Insert(this.Key, this.Value);

            Assert.ThrowsException<InvalidOperationException>(() => this.TestInstance.Insert(this.Key, this.Value));
        }

        [TestMethod]
        public void Insert_NullValue_ArgumentNullException()
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


        [TestMethod]
        public void RetrieveKeys__AllInsertedKeys()
        {
            var expectedValues = ArraysHelper.CreateWithContent(new object(), new object(), new object());

            expectedValues.ForEach(_ => this.TestInstance.Insert(_, new object()));

            var result = ReflectionHelper.Invoke(this.TestInstance, "RetrieveKeys")
                            as IEnumerable<object>;

            Assert.IsTrue(expectedValues.Equivalent(result));
        }


        [TestMethod]
        public void Set_KeyDoesNotExist_CanRetrieveData()
        {
            ReflectionHelper.Invoke(this.TestInstance, "Set", this.Key, this.Value);

            Assert.AreSame(this.Value, this.TestInstance.Retrieve(this.Key));
        }

        [TestMethod]
        public void Set_KeyExists_ValueChanged()
        {
            ReflectionHelper.Invoke(this.TestInstance, "Set", this.Key, new object());
            ReflectionHelper.Invoke(this.TestInstance, "Set", this.Key, this.Value);

            Assert.AreSame(this.Value, this.TestInstance.Retrieve(this.Key));
        }

        [TestMethod]
        public void Set__KeyValueAssociationsKept()
        {
            var keyOne = new object();
            var keyTwo = new object();
            var valueOne = new object();
            var valueTwo = new object();

            ReflectionHelper.Invoke(this.TestInstance, "Set", keyOne, valueOne);
            ReflectionHelper.Invoke(this.TestInstance, "Set", keyTwo, valueTwo);

            Assert.AreSame(valueOne, this.TestInstance.Retrieve(keyOne));
            Assert.AreSame(valueTwo, this.TestInstance.Retrieve(keyTwo));
        }
    }
}
