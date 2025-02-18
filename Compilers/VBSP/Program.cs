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
            writer.Dispose();


            // Reading Reference
            BSPReader reader = new BSPReader("./bsp_test.bsp");
            Console.WriteLine($"Material: {reader.GetLumpData<string>(Lump.LumpType.LUMP_MATERIAL)}");
        }
    }
}
