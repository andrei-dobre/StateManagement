using System;
using System.Collections.Generic;
using DAA.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace DAA.StateManagement.Stores
{
    [TestClass]
    public class UnitTest_Store
    {
        private Store<object, object> TestInstance { get => TestInstanceMock.Object; }
        private Mock<Store<object, object>> TestInstanceMock { get; set; }
        private IProtectedMock<Store<object, object>> TestInstanceMockProtected { get => TestInstanceMock.Protected(); }

        private object Value { get; set; }
        private object Key { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            Value = new object();
            Key = new object();

            TestInstanceMock = new Mock<Store<object, object>>();
            TestInstanceMock.CallBase = true;
        }

        [TestMethod]
        public void Save_KeyNotContained_NotUpdated()
        {
            TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            TestInstanceMock.Setup(_ => _.Add(It.IsAny<object>(), It.IsAny<object>())).Returns(true);

            TestInstance.Save(Key, Value);

            TestInstanceMock.Verify(_ => _.Update(Key, Value), Times.Never());
        }

        [TestMethod]
        public void Save_KeyContained_Updated()
        {
            TestInstanceMock.Setup(_ => _.Update(It.IsAny<object>(), It.IsAny<object>()));
            TestInstanceMock.Setup(_ => _.Add(It.IsAny<object>(), It.IsAny<object>())).Returns(false);

            TestInstance.Save(Key, Value);

            TestInstanceMock.Verify(_ => _.Update(Key, Value), Times.Once());
        }


        [TestMethod]
        public void Insert_KeyNotContained_Added()
        {
            TestInstanceMock
                .Setup(a => a.Add(Key, Value))
                .Returns(true).Verifiable();

            TestInstance.Insert(Key, Value);

            TestInstanceMock.Verify();
        }

        [TestMethod]
        public void Insert_KeyContained_InvalidOperationException()
        {
            TestInstance.Insert(Key, Value);

            Assert.ThrowsException<InvalidOperationException>(() => TestInstance.Insert(Key, Value));
        }

        [TestMethod]
        public void Insert_NullValue_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TestInstance.Insert(Key, null));
        }


        [TestMethod]
        public void Contains_NotInserted_False()
        {
            var result = TestInstance.Contains(Key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Contains_Inserted_True()
        {
            TestInstance.Insert(Key, Value);

            var result = TestInstance.Contains(Key);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void RetrieveKeys__AllInsertedKeys()
        {
            var expectedValues = ArraysHelper.CreateWithContent(new object(), new object(), new object());

            expectedValues.ForEach(_ => TestInstance.Insert(_, new object()));

            var result = ReflectionHelper.Invoke(TestInstance, "RetrieveKeys")
                            as IEnumerable<object>;

            Assert.IsTrue(expectedValues.Equivalent(result));
        }


        [TestMethod]
        public void Set_KeyDoesNotExist_CanRetrieveData()
        {
            ReflectionHelper.Invoke(TestInstance, "Set", Key, Value);

            Assert.AreSame(Value, TestInstance.Retrieve(Key));
        }

        [TestMethod]
        public void Set_KeyExists_ValueChanged()
        {
            ReflectionHelper.Invoke(TestInstance, "Set", Key, new object());
            ReflectionHelper.Invoke(TestInstance, "Set", Key, Value);

            Assert.AreSame(Value, TestInstance.Retrieve(Key));
        }

        [TestMethod]
        public void Set__KeyValueAssociationsKept()
        {
            var keyOne = new object();
            var keyTwo = new object();
            var valueOne = new object();
            var valueTwo = new object();

            ReflectionHelper.Invoke(TestInstance, "Set", keyOne, valueOne);
            ReflectionHelper.Invoke(TestInstance, "Set", keyTwo, valueTwo);

            Assert.AreSame(valueOne, TestInstance.Retrieve(keyOne));
            Assert.AreSame(valueTwo, TestInstance.Retrieve(keyTwo));
        }
    }
}
