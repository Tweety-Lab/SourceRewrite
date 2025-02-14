using SourceRewrite.Files.FileTypes;
using System;
using VBSP.IO;
class Program
{
    static void Main(string[] args)
    {
        // Command-line arguments using the 'args' parameter
        if (args.Length > 0)
        {
            MapFormat testMap = new MapFormat();

            // By default, write the BSP to the location of vbsp.exe
            BSPWriter writer = new BSPWriter("./bsp_test.bsp", args[args.Length - 1]);
            writer.WriteToMap(testMap);
        }
    }
}
