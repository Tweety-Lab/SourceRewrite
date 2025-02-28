using FileFormats.BSP;
using FileFormats.VMF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return outputBSP;
        }
    }
}
