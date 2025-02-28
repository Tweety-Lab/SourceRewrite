using FileFormats.BSP;
using FileFormats.VMF;
using System.Globalization;
using VBSP.Conversion;
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: vbsp.exe <path_to_vmf.vmf>");
            return;
        }

        string vmfPath = args[0];
        Console.WriteLine($"Loading VMF from: {vmfPath}");

        // Read the VMF file
        string contents = File.ReadAllText(vmfPath);

        // Compile the BSP
        BSPFormat compiledBSP = VMFToBSP.CompileVMF(contents);

        // Set output BSP file path
        string bspOutputPath = "./bsp_test.bsp";

        // Write BSP
        using (BSPWriter writer = new BSPWriter(bspOutputPath, vmfPath))
        {
            writer.WriteToMap(compiledBSP);
        }

        Console.WriteLine($"BSP successfully written to: {bspOutputPath}");
    }
}
