using System;
using System.Reflection;
using IoCSamples;
using MyIoC;

namespace HomeTask5
{
    class Program
    {
        static void Main()
        {
            var container = new Container();

            //container.AddAssembly(Assembly.LoadFrom("IoCSamples.dll"));

            #region constructors
            //Example with empty constructor
            //var constructorExample = (ConstructorExample)container.CreateInstance(typeof(ConstructorExample));

            //Generic example with not empty constructor
            //var constructorExample = container.CreateInstance<ConstructorExample>(typeof(IExportExample));

            #endregion

            #region Export types

            //container.AddType(typeof(ExportExample), typeof(IExportExample));
            //var exportExample = container.CreateInstance<IExportExample>();
            //var exportExample = (ExportExample)container.CreateInstance(typeof(IExportExample));

            //exportExample.SayHello();

            //container.AddType(typeof(Logger));
            //var logger = container.CreateInstance<Logger>();

            //logger.Log();

            #endregion

            #region Properties

            //var propertySample = container.CreateInstance<PropertySample>();
            //propertySample.ExportExample.SayHello();
            //propertySample.Logger.Log();

            #endregion

            Console.ReadKey();
        }
    }
}
