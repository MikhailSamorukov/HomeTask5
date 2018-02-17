using System;
using MyIoC.Attributes;

namespace IoCSamples
{
    [Export]
    public class Logger
    {
        public void Log()
        {
            Console.WriteLine("imitation log method");
        }
    }
}