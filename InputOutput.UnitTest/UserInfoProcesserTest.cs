using InputOutput.Processer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Diagnostics;

namespace InputOutput.UnitTest
{


    /// <summary>
    ///This is a test class for UserInfoProcesserTest and is intended
    ///to contain all UserInfoProcesserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserInfoProcesserTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Process
        ///</summary>
        [TestMethod()]
        public void ProcessTest()
        {
            log4net.Config.XmlConfigurator.Configure();


            UserInfoProcesser target = new UserInfoProcesser(); // TODO: Initialize to an appropriate value

            var actual = target.Process("pc45025", "name chet");
            Assert.AreEqual(string.Format(Constant.UserInfo_ProcessResultFormat, "name", "chet"), actual);
            actual = target.Process("pc45025", "name chenping");
            Assert.AreEqual(string.Format(Constant.UserInfo_ProcessResultFormat, "name", "chenping"), actual);
            actual = target.Process("pc45025", "birthday,19860524");
            Assert.AreEqual(string.Format(Constant.UserInfo_ProcessResultFormat, "birthday", "19860524"), actual);

        }
    }
}
