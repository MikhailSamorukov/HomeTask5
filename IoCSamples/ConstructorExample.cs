using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyIoC;
using MyIoC.Attributes;

namespace IoCSamples
{
    [ImportConstructor]
    public class ConstructorExample
    {
        public ConstructorExample()
        {
            Console.WriteLine("Empty constructor");
        }

        public ConstructorExample(IExportExample exp)
        {
            exp.SayHello();
        }

        public ConstructorExample(IExportExample exp, Logger logger)
        {
            exp.SayHello();
            logger.Log();
        }
    }
}
