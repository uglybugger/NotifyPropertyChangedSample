using NUnit.Framework;

namespace NotifyPropertyChangedSample.UnitTests
{
    [TestFixture]
    public abstract class Test
    {
        protected abstract void Given();
        protected abstract void When();

        [SetUp]
        public void SetUp()
        {
            Given();
            When();
        }
    }
}