using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.MDL.VTX
{
    /// <summary>
    /// Valve's Source 1 .vtx mesh strip format.
    /// </summary>
    public class VTXFormat
    {
        public VTXHeader Header;

        // Body Parts
        public List<VTXBodyPartHeader> BodyPartHeaders;
        public List<VTXModelHeader> ModelHeaders;
        public List<VTXModelLODHeader> ModelLODHeaders;
        public List<VTXMeshHeader> MeshHeaders;

        // Mesh Indices
        public List<int> MeshIndices;
        

        // This is where the core of the index data is kept
        public List<VTXStripGroupHeader> StripGroupHeaders;

        public VTXFormat(string path)
        {
            // Load the VTX
            Header = new VTXHeader();

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Read the header
                Header.Version = reader.ReadInt32();

                Header.VertCacheSize = reader.ReadInt32();
                Header.MaxBonesPerStrip = reader.ReadInt16();
                Header.MaxBonesPerTri = reader.ReadInt16();
                Header.MaxBonesPerVert = reader.ReadInt32();

                Header.Checksum = reader.ReadInt32();

                Header.NumLODs = reader.ReadInt32();

                Header.MaterialReplacementListOffset = reader.ReadInt32();

                Header.NumBodyParts = reader.ReadInt32();
                Header.BodyPartOffset = reader.ReadInt32();

                // Order here DOES matter
                BodyPartHeaders = ReadBodyParts(reader);
                ModelHeaders = ReadModels(reader);
                ModelLODHeaders = ReadModelLODs(reader);
                MeshHeaders = ReadMeshes(reader);
                StripGroupHeaders = ReadStripGroups(reader);
            }
        }

        // Read BodyPartHeaders
        public List<VTXBodyPartHeader> ReadBodyParts(BinaryReader reader)
        {
            List<VTXBodyPartHeader> VTXBodyParts = new List<VTXBodyPartHeader>();

            int originalPos = (int)reader.BaseStream.Position;
            reader.BaseStream.Seek(Header.BodyPartOffset, SeekOrigin.Begin);

            for (int i = 0; i < Header.NumBodyParts; i++)
            {
                VTXBodyPartHeader bodyPart = new VTXBodyPartHeader();
                bodyPart.NumModels = reader.ReadInt32();
                bodyPart.ModelOffset = reader.ReadInt32();

                // Add the body part to the list
                VTXBodyParts.Add(bodyPart);
            }

            // Return to original position
            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

            return VTXBodyParts;
        }

        // Read ModelHeaders
        public List<VTXModelHeader> ReadModels(BinaryReader reader)
        {
            int originalPos = (int)reader.BaseStream.Position;
            List<VTXModelHeader> VTXModels = new List<VTXModelHeader>();

            for (int i = 0; i < BodyPartHeaders.Count; i++)
            {
                reader.BaseStream.Seek(BodyPartHeaders[i].ModelOffset, SeekOrigin.Begin);

                VTXModelHeader model = new VTXModelHeader();
                model.NumLODs = reader.ReadInt32();
                model.LodOffset = reader.ReadInt32();

                VTXModels.Add(model);

                Console.WriteLine($"Model {i}: NumLODs = {model.NumLODs}, LodOffset = {model.LodOffset}");
            }

            // return to original position
            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

            return VTXModels;
        }

        // Read Model LODs
        public List<VTXModelLODHeader> ReadModelLODs(BinaryReader reader)
        {
            int originalPos = (int)reader.BaseStream.Position;

            List<VTXModelLODHeader> VTXModelLODs = new List<VTXModelLODHeader>();
            for (int i = 0; i < ModelHeaders.Count; i++)
            {
                reader.BaseStream.Seek(ModelHeaders[i].LodOffset, SeekOrigin.Begin);

                VTXModelLODHeader modelLOD = new VTXModelLODHeader();

                modelLOD.NumMeshes = reader.ReadInt32();
                modelLOD.MeshOffset = reader.ReadInt32();
                modelLOD.SwitchPoint = reader.ReadSingle();

                // Add the model LOD to the list
                VTXModelLODs.Add(modelLOD);
            }

            // return to original position
            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

            return VTXModelLODs;
        }

        // Read Mesh Headers
        public List<VTXMeshHeader> ReadMeshes(BinaryReader reader)
        {
            int originalPos = (int)reader.BaseStream.Position;
            List<VTXMeshHeader> VTXMeshes = new List<VTXMeshHeader>();

            for (int i = 0; i < ModelLODHeaders.Count; i++)
            {
                reader.BaseStream.Seek(ModelLODHeaders[i].MeshOffset, SeekOrigin.Begin);

                VTXMeshHeader mesh = new VTXMeshHeader();

                mesh.NumStripGroups = reader.ReadInt32();
                mesh.StripGroupHeaderOffset = reader.ReadInt32();
                mesh.Flags = (StripGroupFlags)reader.ReadByte();

                // Add the mesh to the list
                VTXMeshes.Add(mesh);
            }

            // return to original position
            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

            return VTXMeshes;
        }

        // Read Strip Group Headers
        public List<VTXStripGroupHeader> ReadStripGroups(BinaryReader reader)
        {
            int originalPos = (int)reader.BaseStream.Position;
            List<VTXStripGroupHeader> VTXStripGroups = new List<VTXStripGroupHeader>();

            for (int i = 0; i < MeshHeaders.Count; i++)
            {
                reader.BaseStream.Seek(MeshHeaders[i].StripGroupHeaderOffset, SeekOrigin.Begin);
                VTXStripGroupHeader stripGroup = new VTXStripGroupHeader();

                stripGroup.NumVerts = reader.ReadInt32();
                stripGroup.VertOffset = reader.ReadInt32();

                stripGroup.NumIndices = reader.ReadInt32();
                stripGroup.IndexOffset = reader.ReadInt32();

                stripGroup.NumStrips = reader.ReadInt32();
                stripGroup.StripOffset = reader.ReadInt32();

                stripGroup.Flags = (StripGroupFlags)reader.ReadByte();

                // V49 Stuff
                stripGroup.NumTopologyIndices = reader.ReadInt32();
                stripGroup.TopologyOffset = reader.ReadInt32();

                // Add the strip group to the list
                VTXStripGroups.Add(stripGroup);

            }

            // return to original position
            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

            return VTXStripGroups;
        }
    }
}
