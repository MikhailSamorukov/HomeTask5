using System;
using MyIoC.Attributes;

namespace IoCSamples
{
    [Export(typeof(IExportExample))]
    public class ExportExample : IExportExample
    {
        public void SayHello()
        {
            Console.WriteLine("Hello i'm ExportExample Type");
        }
    }
}