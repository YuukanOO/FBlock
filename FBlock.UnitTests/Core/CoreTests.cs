using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FBlock.Core;

namespace FBlock.UnitTests.Core
{
    public class DummyComponent1 : Component<string, string>
    {
        protected int _value;

        public DummyComponent1(int value = 0)
        {
            _value = value;
        }

        public override string Process(string arg, JobContext context)
        {
            if(_value != 0)
                context.Set("DUMMY_VALUE", _value);

            return context.Job.Name;
        }
    }

    public class DummyComponent2 : Component<string, int>
    {
        public override int Process(string arg, JobContext context)
        {
            return context.Get<int>("DUMMY_VALUE");
        }
    }

    public class DummyComponent3 : Component<int, string>
    {
        public override string Process(int arg, JobContext context)
        {
            return arg.ToString().ToUpper();
        }
    }

    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestJobNoStart()
        {
            Job<string, string> job = new Job<string, string>();

            job.Process(string.Empty);
        }

        [TestMethod]
        public void TestJob()
        {
            Job<string, string> job = new Job<string, string>("My job");
            job.StartAndEnd(new DummyComponent1());

            Assert.AreEqual("My job", job.Name);
            Assert.AreEqual("My job", job.Process(string.Empty));
        }

        [TestMethod]
        public void TestContext()
        {
            Job<string, int> job = new Job<string, int>();

            job
                .Start(new DummyComponent1())
                .End(new DummyComponent2());

            Assert.AreEqual(0, job.Process(string.Empty));

            job
                .Start(new DummyComponent1(5))
                .End(new DummyComponent2());

            Assert.AreEqual(5, job.Process(string.Empty));
        }

        [TestMethod]
        public void TestChaining()
        {
            Job<string, string> job = new Job<string, string>();

            job
                .Start(new DummyComponent1(1337))
                .Then(new DummyComponent2())
                .End(new DummyComponent3());

            Assert.AreEqual("1337", job.Process(string.Empty));
        }
    }
}
