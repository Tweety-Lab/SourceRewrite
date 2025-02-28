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

        // Load the VMF file
        VMFFormat VMF = new VMFFormat(contents);

        BSPFormat outputBSP = new BSPFormat();

        // Set the BSP header
        outputBSP.Header.mapRevision = VMF.VersionInfo.MapVersion;

        string[] entities = new string[VMF.Entities.Count];

        // Convert entities to GameObject strings
        // Right now this is just entity = component
        int i = 0;
        foreach (Entity vmfEntity in VMF.Entities)
        {
            string entityString = @$"
            GameObject{i} {{
                position ""{vmfEntity.Origin.X} {vmfEntity.Origin.Z} {vmfEntity.Origin.Y}""
                rotation ""0 0 0""
                scale ""1 1 1""
                GameComponents {{
                    {ClassConversion.ClassMap[$"{vmfEntity.ClassName}"]} {{

                    }}
                }}
            }}";

            entities[i] = entityString;

            i++;
        }

        outputBSP.SetLumpData(LumpType.LUMP_GAME_OBJECTS, entities);


        // Set output BSP file path
        string bspOutputPath = "./bsp_test.bsp";

        // Write BSP
        using (BSPWriter writer = new BSPWriter(bspOutputPath, vmfPath))
        {
            writer.WriteToMap(outputBSP);
        }

        Console.WriteLine($"BSP successfully written to: {bspOutputPath}");
    }
}
