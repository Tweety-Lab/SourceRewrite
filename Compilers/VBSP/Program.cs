using FileFormats.BSP;
using System;
class Program
{
    static void Main(string[] args)
    {
        // Command-line arguments using the 'args' parameter
        if (args.Length > 0)
        {
            BSPFormat testMap = new BSPFormat();

            // By default, write the BSP to the location of vbsp.exe
            BSPWriter writer = new BSPWriter("./bsp_test.bsp", args[args.Length - 1]);
            writer.WriteToMap(testMap);
        }
    }
}
