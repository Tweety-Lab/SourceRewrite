using System.Globalization;
using System.Numerics;
using SourceFormats.KeyValues;

namespace SourceFormats.VMF;

/// <summary>
/// Valve's Source 1 .vmf (uncompiled) map file.
/// </summary>
public class VMFFormat
{
    public List<VMFEntity> Entities = new();

    // Accessible data
    public VersionInfo VersionInfo;
    public World World;

    public VMFFormat(string contents)
    {
        // Load the .vmf file as KeyValues
        var kvVMF = new KeyValuesFormat(contents);

        // Version Info
        var pkVersionInfo = kvVMF.GetParentKey("versioninfo");
        LoadVersionInfo(pkVersionInfo);

        // World
        var pkWorld = kvVMF.GetParentKey("world");
        LoadWorld(pkWorld);

        // Process world solids
        ProcessSolids(pkWorld, ref World.Solids);

        var entityCount = 0;
        foreach (var parentKey in kvVMF.ParentKeys)
            // Entity Processing
            if (parentKey.Name == "entity")
            {
                entityCount++;

                var entity = new VMFEntity();
                entity.ID = (int)parentKey.GetKeyValue("id").Value; // Set the Entity ID
                entity.ClassName = (string)parentKey.GetKeyValue("classname").Value; // Set the Entity Class Name
                entity.Origin = (Vector3)parentKey.GetKeyValue("origin").Value; // Set the Entity Origin
                entity.Angles = parentKey.GetKeyValue("angles")?.Value is Vector3 angles
                    ? angles
                    : new Vector3(0, 0, 0); // Set the Entity Angles

                entity.Properties = new List<KeyValue>(); // Init properties for later storage
                entity.Connections = new List<KeyValue>(); // Init connections for later storage
                entity.Solids = new List<VMFSolid>(); // Initialize solids list for entity

                entity.TargetName =
                    (string)(parentKey.GetKeyValue("targetname")?.Value ??
                             entityCount.ToString()); // Set the Entity Target Name (if applicable)

                // Process entity solids
                ProcessSolids(parentKey, ref entity.Solids);

                // Loop through every key value attribute in the entity
                foreach (var keyValue in parentKey.KeyValues) entity.Properties.Add(keyValue); // Add the attribute

                foreach (var keyValue in parentKey.GetChildParentKey("connections")?.KeyValues ??
                                         Enumerable.Empty<KeyValue>()) entity.Connections.Add(keyValue);

                Entities.Add(entity);
            }
    }

    private void ProcessSolids(ParentKey parentKey, ref List<VMFSolid> solidsList)
    {
        foreach (var childParentKey in parentKey.ParentKeys)
            if (childParentKey.Name == "solid")
            {
                var newSolid = new VMFSolid();
                newSolid.ID = (int)childParentKey.GetKeyValue("id").Value;
                newSolid.Sides = new List<VMFSide>();

                foreach (var solidParentKey in childParentKey.ParentKeys)
                    if (solidParentKey.Name == "side")
                        newSolid.Sides.Add(ProcessSide(solidParentKey));

                solidsList.Add(newSolid);
            }
    }

    private VMFSide ProcessSide(ParentKey pkSide)
    {
        var planeValue = (string)pkSide.GetKeyValue("plane").Value;

        // Process the plane
        var plane = VMFUtility.ProcessSidePlane(planeValue);

        // Process UV Axis
        var uAxis = VMFUtility.ProcessUVAxis((string)pkSide.GetKeyValue("uaxis").Value);
        var vAxis = VMFUtility.ProcessUVAxis((string)pkSide.GetKeyValue("vaxis").Value);

        return new VMFSide
        {
            ID = (int)pkSide.GetKeyValue("id").Value,
            Plane = new VMFPlane
            {
                Point1 = plane[0],
                Point2 = plane[1],
                Point3 = plane[2]
            },
            Material = (string)pkSide.GetKeyValue("material").Value,

            UAxis = uAxis,
            VAxis = vAxis
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
        World.MapVersion =
            (int)pkWorld.GetKeyValue("mapversion")
                .Value; // Set the World Map Version (is this any different from version info?)
        World.Solids = new List<VMFSolid>(); // Init solids for later storage
    }
}