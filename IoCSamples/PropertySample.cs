using MyIoC;
using MyIoC.Attributes;

namespace IoCSamples
{
    public class PropertySample
    {
        [Import] public IExportExample ExportExample { get; set; }
        [Import] public Logger Logger { get; set; }
    }
}
