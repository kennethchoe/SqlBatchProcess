using NUnit.Framework;
using SqlBatchProcess;

namespace Test
{
    [TestFixture]
    class BitExtensionsBehavior
    {
        [TestCase("true", false, Result = true)]
        [TestCase("false", true, Result = false)]
        [TestCase("true", true, Result = true)]
        [TestCase("false", false, Result = false)]
        [TestCase("some bad value", false, Result = false)]
        [TestCase("some bad value", true, Result = true)]
        public bool StringValueTest(string source, bool defaultValue)
        {
            return source.ToBooleanOrDefault(defaultValue);
        }
    }
}
