using FileFormats.BSP;
using FileFormats.KeyValues;
using FileFormats.VMF;

namespace VBSP.Conversion
{
    public static class VMFToBSP
    {
        /// <summary>
        /// Returns a BSPFormat object compiled from a VMF file.
        /// </summary>
        public static BSPFormat CompileVMF(string contents)
        {
            // Load the VMF file
            VMFFormat VMF = new VMFFormat(contents);

            BSPFormat outputBSP = new BSPFormat();

            // Set the BSP header to the VMF MapVersion
            outputBSP.Header.mapRevision = VMF.VersionInfo.MapVersion;

            string[] entities = new string[VMF.Entities.Count];

            // Convert entities to GameObject strings
            // Right now this is just entity = component
            int i = 0;
            foreach (Entity vmfEntity in VMF.Entities)
            {
                // Get their properties
                string entityPropertiesString = string.Empty;
                foreach (KeyValue kvProperty in vmfEntity.Properties)
                {
                    entityPropertiesString += $" {kvProperty.Key} \"{kvProperty.Value}\"\n";
                }

                string entityString = @$" GameObject{i} {{
                position ""{vmfEntity.Origin.X} {vmfEntity.Origin.Z} {vmfEntity.Origin.Y}""
                rotation ""0 0 0""
                scale ""1 1 1""
                GameComponents {{
                    {ClassConversion.ClassMap[$"{vmfEntity.ClassName}"]} {{
                        {entityPropertiesString}
                    }}
                }}
            }}
            ";

                entities[i] = entityString;

                i++;
            }

            Console.WriteLine(entities[1]);
            outputBSP.SetLumpData(LumpType.LUMP_MATERIAL, $"{VMF.World.Solids[0].Sides[0].Material}");
            outputBSP.SetLumpData(LumpType.LUMP_GAME_OBJECTS, entities);

            return outputBSP;
        }
    }
}
