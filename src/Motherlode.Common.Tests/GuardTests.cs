using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Motherlode.Common.Tests
{
    [TestFixture]
    public class GuardTests
    {
        #region Enums

        [Flags]
        private enum FlagsEnum
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 4,
            Four = 8
        }

        [Flags]
        private enum FlagsEnumNoNone
        {
            One = 1,
        }

        private enum SimpleEnum : byte
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 4,
            Four = 8
        }

        #endregion

        #region Public Methods and Operators

        [Test]
        public void IsEnumMember_FlagsEnumNoNone_ShouldThrowIfPassedValueIsZero()
        {
            var e = (FlagsEnumNoNone)0;
            var ex = Assert.Throws<ArgumentException>(() => Guard.IsEnumMember(() => e));
            Assert.AreEqual(
                "Enum value '0' is not valid for flags enumeration " +
                "'Motherlode.Common.Tests.GuardTests+FlagsEnumNoNone'.\r\nParameter name: e",
                ex.Message);
        }

        [Test]
        public void IsEnumMember_FlagsEnum_ShouldNotThrowIfValueIsValid()
        {
            FlagsEnum e = FlagsEnum.One | FlagsEnum.Four;
            Guard.IsEnumMember(() => e);
        }

        [Test]
        public void IsEnumMember_FlagsEnum_ShouldThrowIfValueIsInvalid()
        {
            var e = (FlagsEnum)666;
            var ex = Assert.Throws<ArgumentException>(() => Guard.IsEnumMember(() => e));
            Assert.AreEqual(
                "Enum value '666' is not valid for flags enumeration " +
                "'Motherlode.Common.Tests.GuardTests+FlagsEnum'.\r\nParameter name: e",
                ex.Message);
        }

        [Test]
        public void IsEnumMember_SimpleEnum_ShouldNotThrowIfValueIsValid()
        {
            var e = SimpleEnum.One;
            Guard.IsEnumMember(() => e);
        }

        [Test]
        public void IsEnumMember_SimpleEnum_ShouldThrowIfValueIsInvalid()
        {
            var e = (SimpleEnum)222;
            var ex = Assert.Throws<ArgumentException>(() => Guard.IsEnumMember(() => e));
            Assert.AreEqual(
                "Enum value '222' is not defined for enumeration " +
                "'Motherlode.Common.Tests.GuardTests+SimpleEnum'.\r\nParameter name: e",
                ex.Message);
        }

        [Test]
        public void IsNotNullOrEmpty_ShouldNotThrowIfPassedValueIsValid()
        {
            string str = "test";
            Guard.IsNotNullOrEmpty(() => str);
        }

        [Test]
        public void IsNotNullOrEmpty_ShouldThrowIfPassedValueIsEmpty()
        {
            string str = "";
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.IsNotNullOrEmpty(() => str));
            Assert.True(ex.Message.EndsWith(" str"));
        }

        [Test]
        public void IsNotNullOrEmpty_ShouldThrowIfPassedValueIsNull()
        {
            string str = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.IsNotNullOrEmpty(() => str));
            Assert.True(ex.Message.EndsWith(" str"));
        }

        [Test]
        public void IsNotNullOrWhiteSpace_ShouldThrowIfPassedValueIsWhiteSpace()
        {
            string str = "  \t \r \n ";
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.IsNotNullOrWhiteSpace(() => str));
            Assert.True(ex.Message.EndsWith(" str"));
        }

        [Test]
        public void IsNotNull_PerformanceTest()
        {
            string test = "test";
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                Guard.IsNotNull(() => test, test);
            }
            
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.ToString());
            
            sw.Restart();
            for (int i = 0; i < 100000; i++)
            {
                Guard.IsNotNull(test, "test");
            }

            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.ToString());
        }

        [Test]
        public void IsNotNull_GenericArgumentsAreCorrectlyHandled()
        {
            string str = null;
            var ex = Assert.Throws<ArgumentNullException>(() => this.genericMethod(str));
            Assert.True(ex.Message.EndsWith(" s"));
        }

        [Test]
        public void IsNotNull_Nullable_ShouldNotThrowIfHasValue()
        {
            int? i = 1;
            Assert.DoesNotThrow(() => Guard.IsNotNull(() => i));
        }

        [Test]
        public void IsNotNull_Nullable_ShouldThrowWhenNull()
        {
            int? i = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.IsNotNull(() => i));
            Assert.True(ex.Message.EndsWith(" i"));
        }

        [Test]
        public void IsNotNull_Reference_ShouldNotThrowIfHasValue()
        {
            string test = "testText";
            Assert.DoesNotThrow(() => Guard.IsNotNull(() => test));
        }

        [Test]
        public void IsNotNull_Reference_ShouldThrowWhenNull()
        {
            string test = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.IsNotNull(() => test));
            Assert.True(ex.Message.EndsWith(" test"));
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        #region Methods

        private void genericMethod<T>(T s) where T : class
        {
            Guard.IsNotNull(() => s);
        }

        #endregion
    }
}
