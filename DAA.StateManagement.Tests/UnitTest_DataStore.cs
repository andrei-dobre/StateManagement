using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using DAA.Helpers;
using DAA.StateManagement.DataManagement;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataStore
    {
        private IDataManipulator<IData> DataManipulator { get => DataManipulatorMock.Object; }
        private Mock<IDataManipulator<IData>> DataManipulatorMock { get; set; }

        private ITerminalDescriptor Descriptor { get => DescriptorMock.Object; }
        private Mock<ITerminalDescriptor> DescriptorMock { get; set; }

        private DataStore<IData> TestInstance { get => TestInstanceMock.Object; }
        private Mock<DataStore<IData>> TestInstanceMock { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            DataManipulatorMock = new Mock<IDataManipulator<IData>>();
            DescriptorMock = new Mock<ITerminalDescriptor>();

            TestInstanceMock = new Mock<DataStore<IData>>(DataManipulator);
            TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void GetDataManipulator__ProvidedValue()
        {
            var testInstance = new DataStore<IData>(DataManipulator);

            var result = ReflectionHelper.Invoke(testInstance, "DataManipulator");

            Assert.AreSame(DataManipulator, result);
        }


        [TestMethod]
        public void Update__InitialDataUpdatedFromSpecifiedInstance()
        {
            var initialData = new Mock<IData>().Object;
            var newData = new Mock<IData>().Object;

            TestInstanceMock
                .Setup(_ => _.Retrieve(Descriptor))
                .Returns(initialData);

            TestInstance.Update(Descriptor, newData);

            DataManipulatorMock.Verify(_ => _.Update(initialData, newData));
        }
    }
}
