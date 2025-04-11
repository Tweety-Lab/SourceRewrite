using System;
using System.Collections.Generic;
using System.IO;

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

        // This is where the core of the index data is kept
        public List<VTXStripGroupHeader> StripGroupHeaders;
        public List<VTXStripHeader> StripHeaders;

        // Mesh Indices
        public List<uint> MeshIndices;


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

                Console.WriteLine($"BodyPartOffset: {Header.BodyPartOffset}");

                // Order here DOES matter
                BodyPartHeaders = ReadBodyParts(reader);
                ModelHeaders = ReadModels(reader);
                ModelLODHeaders = ReadModelLODs(reader);
                MeshHeaders = ReadMeshes(reader);
                StripGroupHeaders = ReadStripGroups(reader);
                StripHeaders = ReadStrips(reader);

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

                Console.WriteLine($"Body Part {i}: NumModels = {bodyPart.NumModels}, ModelOffset = {bodyPart.ModelOffset}");
                VTXBodyParts.Add(bodyPart);
            }

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
                // Compute absolute offset: BodyPartOffset + ModelOffset
                long absoluteModelOffset = Header.BodyPartOffset + BodyPartHeaders[i].ModelOffset;
                reader.BaseStream.Seek(absoluteModelOffset, SeekOrigin.Begin);

                VTXModelHeader model = new VTXModelHeader();
                model.NumLODs = reader.ReadInt32();
                model.LodOffset = reader.ReadInt32();

                Console.WriteLine($"Model Header {i}: NumLODs = {model.NumLODs}, LodOffset = {model.LodOffset}");
                VTXModels.Add(model);
            }

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
                // Compute absolute offset: BodyPartOffset + ModelOffset + LodOffset
                long absoluteLodOffset = Header.BodyPartOffset + BodyPartHeaders[i].ModelOffset + ModelHeaders[i].LodOffset;
                reader.BaseStream.Seek(absoluteLodOffset, SeekOrigin.Begin);

                VTXModelLODHeader modelLOD = new VTXModelLODHeader();
                modelLOD.NumMeshes = reader.ReadInt32();
                modelLOD.MeshOffset = reader.ReadInt32();
                modelLOD.SwitchPoint = reader.ReadSingle();

                Console.WriteLine($"Model LOD {i}: NumMeshes = {modelLOD.NumMeshes}, MeshOffset = {modelLOD.MeshOffset}, SwitchPoint = {modelLOD.SwitchPoint}");
                VTXModelLODs.Add(modelLOD);
            }

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
                // Compute absolute offset: BodyPartOffset + ModelOffset + LodOffset + MeshOffset
                long absoluteMeshOffset = Header.BodyPartOffset + BodyPartHeaders[i].ModelOffset + ModelHeaders[i].LodOffset + ModelLODHeaders[i].MeshOffset;
                reader.BaseStream.Seek(absoluteMeshOffset, SeekOrigin.Begin);

                VTXMeshHeader mesh = new VTXMeshHeader();
                mesh.NumStripGroups = reader.ReadInt32();
                mesh.StripGroupHeaderOffset = reader.ReadInt32();
                mesh.Flags = (StripGroupFlags)reader.ReadByte();

                Console.WriteLine($"Mesh Header {i}: NumStripGroups = {mesh.NumStripGroups}, StripGroupHeaderOffset = {mesh.StripGroupHeaderOffset}, Flags = {mesh.Flags}");
                VTXMeshes.Add(mesh);
            }

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
                // Calculate the absolute position of the strip group in the file:
                long absoluteStripGroupOffset = Header.BodyPartOffset
                             + BodyPartHeaders[i].ModelOffset
                             + ModelHeaders[i].LodOffset
                             + ModelLODHeaders[i].MeshOffset
                             + MeshHeaders[i].StripGroupHeaderOffset;

                reader.BaseStream.Seek(absoluteStripGroupOffset, SeekOrigin.Begin);

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

                Console.WriteLine($"Strip Group {i}: NumVerts = {stripGroup.NumVerts}, VertOffset = {stripGroup.VertOffset}, NumIndices = {stripGroup.NumIndices}, IndexOffset = {stripGroup.IndexOffset}, NumStrips = {stripGroup.NumStrips}, StripOffset = {stripGroup.StripOffset}, Flags = {stripGroup.Flags}");
                VTXStripGroups.Add(stripGroup);
            }

            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);
            return VTXStripGroups;
        }

        // Read Strips
        public List<VTXStripHeader> ReadStrips(BinaryReader reader)
        {
            int originalPos = (int)reader.BaseStream.Position;
            List<VTXStripHeader> strips = new List<VTXStripHeader>();

            for (int i = 0; i < StripGroupHeaders.Count; i++)
            {
                var stripGroup = StripGroupHeaders[i];
                if (stripGroup.NumStrips <= 0)
                    continue;

                // Calculate the absolute position of the strip in the file:
                // BodyPartOffset + ModelOffset + LODOffset + MeshOffset + StripGroupHeaderOffset
                long stripGroupStart =
                    Header.BodyPartOffset
                    + BodyPartHeaders[i].ModelOffset
                    + ModelHeaders[i].LodOffset
                    + ModelLODHeaders[i].MeshOffset
                    + MeshHeaders[i].StripGroupHeaderOffset;

                // Seek to the strip data
                reader.BaseStream.Seek(stripGroupStart + stripGroup.StripOffset, SeekOrigin.Begin);

                for (int j = 0; j < stripGroup.NumStrips; j++)
                {
                    VTXStripHeader strip = new VTXStripHeader();

                    strip.NumIndices = reader.ReadInt32();
                    strip.IndexOffset = reader.ReadInt32();

                    strip.NumVerts = reader.ReadInt32();
                    strip.VertOffset = reader.ReadInt32();

                    strip.NumBones = reader.ReadInt16();
                    strip.Flags = (StripFlags)reader.ReadByte();

                    strip.NumBoneStateChanges = reader.ReadInt32();
                    strip.BoneStateChangeOffset = reader.ReadInt32();

                    // v49 stuff
                    strip.NumTopologyIndices = reader.ReadInt32();
                    strip.TopologyOffset = reader.ReadInt32();

                    Console.WriteLine($"Strip {j}: NumIndices={strip.NumIndices}, Flags={strip.Flags}, NumBones={strip.NumBones}");
                    strips.Add(strip);
                }
            }

            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);
            return strips;
        }
    }
}