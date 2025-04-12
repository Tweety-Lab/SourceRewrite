using FileFormats.MDL.VVD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileFormats.MDL.VTX
{
    public class VTXFormat
    {
        public VTXHeader Header;

        public VTXBodyPart BodyPart;
        public VTXModel Model;
        public VTXModelLOD LOD;
        public VTXMesh Mesh;
        public VTXStripGroup StripGroup;
        public VTXStrip Strip;

        public ushort[] MeshIndices;

        public VTXFormat(string filePath)
        {
            // Load the VTX
            Header = new VTXHeader();

            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
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

                // Navigate to BodyPart array
                reader.BaseStream.Seek(Header.BodyPartOffset, SeekOrigin.Begin);

                // BodyPart
                BodyPart = new VTXBodyPart()
                {
                    NumModels = reader.ReadInt32(),
                    ModelOffset = reader.ReadInt32()
                };
                long bodyPartOffset = Header.BodyPartOffset;

                // Navigate to the Model array
                reader.BaseStream.Seek(bodyPartOffset + BodyPart.ModelOffset, SeekOrigin.Begin);
                Model = new VTXModel()
                {
                    NumLODs = reader.ReadInt32(),
                    LODOffset = reader.ReadInt32()
                };
                long modelOffset = bodyPartOffset + BodyPart.ModelOffset;

                // Navigate to the LOD Mesh array
                reader.BaseStream.Seek(modelOffset + Model.LODOffset, SeekOrigin.Begin);
                LOD = new VTXModelLOD()
                {
                    NumMeshes = reader.ReadInt32(),
                    MeshOffset = reader.ReadInt32(),
                    SwitchPoint = reader.ReadSingle()
                };
                long lodOffset = modelOffset + Model.LODOffset;

                // Navigate to the Mesh Array
                reader.BaseStream.Seek(lodOffset + LOD.MeshOffset, SeekOrigin.Begin);
                Mesh = new VTXMesh()
                {
                    NumStripGroups = reader.ReadInt32(),
                    StripGroupHeaderOffset = reader.ReadInt32(),
                    Flags = (StripGroupFlags)reader.ReadChar()
                };
                long meshOffset = lodOffset + LOD.MeshOffset;

                // Navigate to the Strip Group array
                reader.BaseStream.Seek(meshOffset + Mesh.StripGroupHeaderOffset, SeekOrigin.Begin);
                StripGroup = new VTXStripGroup()
                {
                    NumVerts = reader.ReadInt32(),
                    VertOffset = reader.ReadInt32(),
                    NumIndices = reader.ReadInt32(),
                    IndexOffset = reader.ReadInt32(),
                    NumStrips = reader.ReadInt32(),
                    StripOffset = reader.ReadInt32(),
                    Flags = reader.ReadChar(),
                    NumTopologyIndices = reader.ReadInt32(),
                    TopologyIndexOffset = reader.ReadInt32()
                };
                long stripGroupOffset = meshOffset + Mesh.StripGroupHeaderOffset;

                // After reading StripGroup, load the index and vertex data
                reader.BaseStream.Seek(stripGroupOffset + StripGroup.IndexOffset, SeekOrigin.Begin);
                StripGroup.IndexData = reader.ReadBytes(StripGroup.NumIndices * 2); // 2 bytes per UInt16

                reader.BaseStream.Seek(stripGroupOffset + StripGroup.VertOffset, SeekOrigin.Begin);
                StripGroup.VertexData = reader.ReadBytes(StripGroup.NumVerts * 9); // 9 bytes per vertex (adjust if needed)

                // Navigate to the Strip Array
                reader.BaseStream.Seek(stripGroupOffset + StripGroup.StripOffset, SeekOrigin.Begin);
                Strip = new VTXStrip()
                {
                    NumIndices = reader.ReadInt32(),
                    IndexOffset = reader.ReadInt32(),
                    NumVerts = reader.ReadInt32(),
                    VertOffset = reader.ReadInt32(),
                    NumBones = reader.ReadInt16(),
                    Flags = reader.ReadChar(),
                    NumBoneStateChanges = reader.ReadInt32(),
                    BoneStateChangeOffset = reader.ReadInt32(),
                    NumTopologyIndices = reader.ReadInt32(),
                    TopologyIndexOffset = reader.ReadInt32()
                };
                long stripOffset = stripGroupOffset + StripGroup.StripOffset;

                // Navigate to indices
                reader.BaseStream.Seek(stripGroupOffset + StripGroup.IndexOffset + Strip.IndexOffset, SeekOrigin.Begin);
                MeshIndices = new ushort[Strip.NumIndices];
                for (int i = 0; i < Strip.NumIndices; i++)
                {
                    MeshIndices[i] = reader.ReadUInt16();
                }
            };
        }

    }
}