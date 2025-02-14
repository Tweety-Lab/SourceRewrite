using System;
using VBSP.IO;
class Program
{
    static void Main(string[] args)
    {
        // Entry point of the application
        Console.WriteLine("Hello, World!");

        // By default, write the BSP to the location of vbsp.exe
        BSPWriter writer = new BSPWriter("./bsp_test.bsp");
        writer.Write("Test Binary Write.");

        // Command-line arguments using the 'args' parameter
        if (args.Length > 0)
        {
            Console.WriteLine("Command-line arguments:");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
        }
    }
}
