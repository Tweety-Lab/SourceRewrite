using FileFormats.KeyValues;
using System.Globalization;
using System.Numerics;

namespace FileFormats.VMF
{
    /// <summary>
    /// Valve's Source 1 .vmf (uncompiled) map file.
    /// </summary>
    public class VMFFormat
    {
        // Accessible data
        public VersionInfo VersionInfo;
        public World World;

        public List<Entity> Entities = new List<Entity>();

        public VMFFormat(string contents)
        {
            // Load the .vmf file as KeyValues
            KeyValuesFormat kvVMF = new KeyValuesFormat(contents);

            // Version Info
            ParentKey pkVersionInfo = kvVMF.GetParentKey("versioninfo");
            LoadVersionInfo(pkVersionInfo);

            // World
            ParentKey pkWorld = kvVMF.GetParentKey("world");
            LoadWorld(pkWorld);

            // Loop through every parent key in the world
            foreach (ParentKey parentKey in pkWorld.ChildParentKeys)
            {
                // Solid Processing
                if (parentKey.Name == "solid")
                {
                    Solid newSolid = new Solid();
                    newSolid.ID = (int)parentKey.GetKeyValue("id").Value; // Set the Solid ID
                    newSolid.Sides = new List<Side>(); // Init sides for later storage

                    // Loop through every parent key in the solid
                    foreach (ParentKey solidParentKey in parentKey.ChildParentKeys)
                    {
                        // Side Processing
                        if (solidParentKey.Name == "side")
                        {
                            newSolid.Sides.Add(ProcessSide(solidParentKey)); // Add the processed side to the list
                        }
                    }

                    World.Solids.Add(newSolid); // Add the solid to the world
                }
            }

            foreach(ParentKey parentKey in kvVMF.ParentKeys)
            {
                // Entity Processing
                if (parentKey.Name == "entity")
                {
                    Entity entity = new Entity();
                    entity.ID = (int)parentKey.GetKeyValue("id").Value; // Set the Entity ID
                    entity.ClassName = (string)parentKey.GetKeyValue("classname").Value; // Set the Entity Class Name
                    entity.Origin = (Vector3)parentKey.GetKeyValue("origin").Value; // Set the Entity Origin
                    entity.Properties = new List<KeyValue>(); // Init properties for later storage

                    // Loop through every key value attribute in the entity
                    foreach (KeyValue keyValue in parentKey.ChildKeyValues)
                    {
                        entity.Properties.Add(keyValue); // Add the attribute
                    }

                    Entities.Add(entity);
                }
            }
        }

        private Side ProcessSide(ParentKey pkSide)
        {
            string planeValue = (string) pkSide.GetKeyValue("plane").Value;

            // Process the plane
            Vector3[] plane = ProcessSidePlane(planeValue);

            return new Side()
            {
                ID = (int)pkSide.GetKeyValue("id").Value,
                Plane = new Plane()
                {
                    Corner1 = plane[0],
                    Corner2 = plane[1],
                    Corner3 = plane[2],
                },
                Material = (string)pkSide.GetKeyValue("material").Value,
                UAxis = ProcessUVAxis(pkSide.GetKeyValue("uaxis").Value.ToString()),
                VAxis = ProcessUVAxis(pkSide.GetKeyValue("vaxis").Value.ToString())
            };
        }


        private UVAxis ProcessUVAxis(string input)
        {
            // Split the input string into the UV direction part and the scale part
            string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Parse the UV direction as a Vector4 (the first four numbers)
            Vector4 uvDir = new Vector4(
                float.Parse(parts[0].TrimStart('[')),  // X
                float.Parse(parts[1]),                  // Y
                float.Parse(parts[2]),                  // Z
                float.Parse(parts[3].TrimEnd(']')));    // W

            // Parse the UV scale (the last part of the string)
            float uvScale = float.Parse(parts[4]);

            // Return the populated UVAxis object
            return new UVAxis()
            {
                UVDir = uvDir,
                UVScale = uvScale
            };
        }

        // Converts a Side Plane into 3 Vector3s, Side plane example: "(0 0 0) (0 0 0) (0 0 0)"
        private Vector3[] ProcessSidePlane(string input)
        {
            // Remove any extra whitespace and split by closing and opening parentheses
            var parts = input
                .Replace("(", "")  // Remove opening parentheses
                .Replace(")", "")  // Remove closing parentheses
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split by spaces and ignore empty entries
                .Select(s => float.Parse(s, CultureInfo.InvariantCulture))  // Parse to float
                .ToArray();

            // Ensure that we have exactly 9 values (3 sets of 3 values)
            if (parts.Length != 9)
            {
                Console.WriteLine($"Error: Expected 9 values but got {parts.Length}. Input: '{input}'");
                return new Vector3[0]; // Return empty array to handle error gracefully
            }

            // Return the three Vector3s (each with 3 components)
            return new[]
            {
                new Vector3(parts[0], parts[1], parts[2]), // First Vector3
                new Vector3(parts[3], parts[4], parts[5]), // Second Vector3
                new Vector3(parts[6], parts[7], parts[8])  // Third Vector3
            };
        }

        // Loads Version Info
        private void LoadVersionInfo(ParentKey pkVersionInfo)
        {
            VersionInfo.EditorVersion = (int)pkVersionInfo.GetKeyValue("editorversion").Value; // Set the Editor Version
            VersionInfo.EditorBuild = (int)pkVersionInfo.GetKeyValue("editorbuild").Value; // Set the Editor Build
            VersionInfo.MapVersion = (int)pkVersionInfo.GetKeyValue("mapversion").Value; // Set the Map Version
            VersionInfo.FormatVersion = (int)pkVersionInfo.GetKeyValue("formatversion").Value; // Set the Format Version
        }
        
        // Loads World Info
        private void LoadWorld(ParentKey pkWorld)
        {
            World.ID = (int)pkWorld.GetKeyValue("id").Value; // Set the World ID
            World.MapVersion = (int)pkWorld.GetKeyValue("mapversion").Value; // Set the World Map Version (is this any different from version info?)
            World.Solids = new List<Solid>(); // Init solids for later storage
        }
    }
}
