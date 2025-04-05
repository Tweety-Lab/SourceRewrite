using FileFormats.Binary;
using FileFormats.BSP;
using FileFormats.BSP.IO;
using System.Numerics;
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

        BSPFormat testBSP = new BSPFormat();
        testBSP.Header.Identifier = "VBSP";
        testBSP.Header.Version = 26;
        testBSP.Header.Lumps = new BSPLump[16];

        testBSP.SetLumpData(BSPLumpType.LUMP_SIDES, new SideData[2] { new SideData() { ID = 3, MaterialName = "TEST MATERAIL1", X = 0, Y = 0, Z = 0 }, new SideData() { ID = 4, MaterialName = "TEST MATERAIL2", X = 1, Y = 2, Z = 3 } });

        SideData[] lumpData = (SideData[])testBSP.GetLump(BSPLumpType.LUMP_SIDES).Data;

        // Print all sides
        foreach (SideData side in lumpData)
        {
            Console.WriteLine($"ID: {side.ID}, Material: {side.MaterialName}, X: {side.X}, Y: {side.Y}, Z: {side.Z}");
        }

        BSPWriter writer = new BSPWriter(bspOutputPath, testBSP);
        writer.WriteToFile();
    }
}
