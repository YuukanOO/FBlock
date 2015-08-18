using FBlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBlock.UnitTests.Fixtures
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
            if (_value != 0)
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
}
