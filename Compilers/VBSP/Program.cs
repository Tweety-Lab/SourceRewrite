using System.Numerics;
using EngineFormats.BSP;
using EngineFormats.BSP.IO;
using VBSP.Conversion;
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: vbsp.exe <path_to_vmf.vmf> [-o <output_path>]");
            return;
        }

        string vmfPath = null;
        string bspOutputPath = "./map.bsp"; // Default output path

        // Parse command-line arguments
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-o" && i + 1 < args.Length)
            {
                bspOutputPath = args[i + 1];
                i++; // Skip to next argument after the output path
            }
            else
            {
                vmfPath = args[i];
            }
        }

        if (vmfPath == null)
        {
            Console.WriteLine("Error: VMF path not specified.");
            return;
        }

        // Add extension if not present
        if (!vmfPath.EndsWith(".vmf"))
        {
            vmfPath += ".vmf";
        }

        Console.WriteLine($"Loading VMF from: {vmfPath}");

        // Read the VMF file
        string contents = File.ReadAllText(vmfPath);

        // Compile the BSP
        BSPFormat compiledBSP = VMFToBSP.CompileVMF(contents);

        compiledBSP.Header.Identifier = "VBSP";
        compiledBSP.Header.Version = 26;

        BSPWriter writer = new BSPWriter(bspOutputPath, compiledBSP);
        writer.WriteToFile();
    }
}
