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
        public List<VTXBodyPart> BodyParts = new List<VTXBodyPart>();

        public VTXFormat(string path)
        {
            using var reader = new BinaryReader(File.Open(path, FileMode.Open), Encoding.UTF8, false);

            // Read header
            Header = ReadHeader(reader);

            // Read body parts
            BodyParts = ReadBodyParts(reader, Header.BodyPartOffset, Header.NumBodyParts);
        }

        private VTXHeader ReadHeader(BinaryReader reader)
        {
            return new VTXHeader
            {
                Version = reader.ReadInt32(),
                VertCacheSize = reader.ReadInt32(),
                MaxBonesPerStrip = reader.ReadInt16(),
                MaxBonesPerTri = reader.ReadInt16(),
                MaxBonesPerVert = reader.ReadInt32(),
                Checksum = reader.ReadInt32(),
                NumLODs = reader.ReadInt32(),
                MaterialReplacementListOffset = reader.ReadInt32(),
                NumBodyParts = reader.ReadInt32(),
                BodyPartOffset = reader.ReadInt32()
            };
        }

        private List<VTXBodyPart> ReadBodyParts(BinaryReader reader, int offset, int numBodyParts)
        {
            if (numBodyParts <= 0 || offset < 0)
            {
                return new List<VTXBodyPart>();
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            List<VTXBodyPart> bodyParts = new List<VTXBodyPart>(numBodyParts);

            for (int i = 0; i < numBodyParts; i++)
            {
                var bodyPart = new VTXBodyPart
                {
                    NumModels = reader.ReadInt32(),
                    ModelOffset = reader.ReadInt32()
                };

                // Save current position
                long currentPosition = reader.BaseStream.Position;

                // Read models for this body part
                bodyPart.Models = ReadModels(reader, offset + bodyPart.ModelOffset, bodyPart.NumModels);

                // Restore position to continue reading body parts
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

                bodyParts.Add(bodyPart);
            }

            return bodyParts;
        }

        private List<VTXModel> ReadModels(BinaryReader reader, int offset, int numModels)
        {
            if (numModels <= 0 || offset < 0)
            {
                return new List<VTXModel>();
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            List<VTXModel> models = new List<VTXModel>(numModels);

            for (int i = 0; i < numModels; i++)
            {
                var model = new VTXModel
                {
                    NumLODs = reader.ReadInt32(),
                    LodOffset = reader.ReadInt32()
                };

                // Save current position
                long currentPosition = reader.BaseStream.Position;

                // Read LODs for this model
                model.LODs = ReadModelLODs(reader, offset + model.LodOffset, model.NumLODs);

                // Restore position to continue reading models
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

                models.Add(model);
            }

            return models;
        }

        private List<VTXModelLOD> ReadModelLODs(BinaryReader reader, int offset, int numLODs)
        {
            if (numLODs <= 0 || offset < 0)
            {
                return new List<VTXModelLOD>();
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            List<VTXModelLOD> lods = new List<VTXModelLOD>(numLODs);

            for (int i = 0; i < numLODs; i++)
            {
                var lod = new VTXModelLOD
                {
                    NumMeshes = reader.ReadInt32(),
                    MeshOffset = reader.ReadInt32(),
                    SwitchPoint = reader.ReadSingle()
                };

                // Save current position
                long currentPosition = reader.BaseStream.Position;

                // Read meshes for this LOD
                lod.Meshes = ReadMeshes(reader, offset + lod.MeshOffset, lod.NumMeshes);

                // Restore position to continue reading LODs
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

                lods.Add(lod);
            }

            return lods;
        }

        private List<VTXMesh> ReadMeshes(BinaryReader reader, int offset, int numMeshes)
        {
            if (numMeshes <= 0 || offset < 0)
            {
                return new List<VTXMesh>();
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            List<VTXMesh> meshes = new List<VTXMesh>(numMeshes);

            for (int i = 0; i < numMeshes; i++)
            {
                var mesh = new VTXMesh
                {
                    NumStripGroups = reader.ReadInt32(),
                    StripGroupHeaderOffset = reader.ReadInt32(),
                    Flags = (StripGroupFlags)reader.ReadByte()
                };

                // Save current position
                long currentPosition = reader.BaseStream.Position;

                // Read strip groups for this mesh
                mesh.StripGroups = ReadStripGroups(reader, offset + mesh.StripGroupHeaderOffset, mesh.NumStripGroups);

                // Restore position to continue reading meshes
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

                meshes.Add(mesh);
            }

            return meshes;
        }

        private List<VTXStripGroup> ReadStripGroups(BinaryReader reader, int offset, int numStripGroups)
        {
            if (numStripGroups <= 0 || offset < 0)
            {
                return new List<VTXStripGroup>();
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            List<VTXStripGroup> stripGroups = new List<VTXStripGroup>(numStripGroups);

            for (int i = 0; i < numStripGroups; i++)
            {
                var stripGroup = new VTXStripGroup
                {
                    NumVerts = reader.ReadInt32(),
                    VertOffset = reader.ReadInt32(),
                    NumIndices = reader.ReadInt32(),
                    IndexOffset = reader.ReadInt32(),
                    NumStrips = reader.ReadInt32(),
                    StripOffset = reader.ReadInt32(),
                    Flags = (StripGroupFlags)reader.ReadByte(),
                    NumTopologyIndices = reader.ReadInt32(),
                    TopologyOffset = reader.ReadInt32()
                };

                // Save current position to continue reading strip groups
                long currentPosition = reader.BaseStream.Position;

                // Read vertices, indices, and strips for this strip group
                stripGroup.Vertices = ReadVertices(reader, offset + stripGroup.VertOffset, stripGroup.NumVerts);
                stripGroup.Indices = ReadIndices(reader, offset + stripGroup.IndexOffset, stripGroup.NumIndices);
                stripGroup.Strips = ReadStrips(reader, offset + stripGroup.StripOffset, stripGroup.NumStrips);

                // Restore position
                reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

                stripGroups.Add(stripGroup);
            }

            return stripGroups;
        }

        private List<ushort> ReadIndices(BinaryReader reader, int offset, int numIndices)
        {
            if (numIndices <= 0 || offset < 0)
            {
                return new List<ushort>();
            }

            // Store current position to restore later
            long currentPosition = reader.BaseStream.Position;

            // Seek to the correct absolute position
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            // Read all indices at once
            List<ushort> indices = new List<ushort>(numIndices);
            for (int i = 0; i < numIndices; i++)
            {
                indices.Add(reader.ReadUInt16());
            }

            // Restore original position
            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

            return indices;
        }

        private List<VTXVertex> ReadVertices(BinaryReader reader, int offset, int numVerts)
        {
            if (numVerts <= 0 || offset < 0)
            {
                return new List<VTXVertex>();
            }

            long currentPosition = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            List<VTXVertex> vertices = new List<VTXVertex>(numVerts);
            for (int i = 0; i < numVerts; i++)
            {
                var vertex = new VTXVertex
                {
                    BoneWeightIndex = reader.ReadByte(),
                    NumBones = reader.ReadByte(),
                    OriginalMeshVertexID = reader.ReadUInt16(),
                    BoneID = new byte[3]
                };
                vertex.BoneID[0] = reader.ReadByte();
                vertex.BoneID[1] = reader.ReadByte();
                vertex.BoneID[2] = reader.ReadByte();
                vertices.Add(vertex);
            }

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            return vertices;
        }

        private List<VTXStrip> ReadStrips(BinaryReader reader, int offset, int numStrips)
        {
            if (numStrips <= 0 || offset < 0)
            {
                return new List<VTXStrip>();
            }

            long currentPosition = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            List<VTXStrip> strips = new List<VTXStrip>(numStrips);
            for (int i = 0; i < numStrips; i++)
            {
                var strip = new VTXStrip
                {
                    NumIndices = reader.ReadInt32(),
                    IndexOffset = reader.ReadInt32(),
                    NumVerts = reader.ReadInt32(),
                    VertOffset = reader.ReadInt32(),
                    NumBones = reader.ReadInt16(),
                    Flags = (StripFlags)reader.ReadByte(),
                    NumBoneStateChanges = reader.ReadInt32(),
                    BoneStateChangeOffset = reader.ReadInt32(),
                    NumTopologyIndices = reader.ReadInt32(),
                    TopologyOffset = reader.ReadInt32()
                };
                strips.Add(strip);
            }

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            return strips;
        }
    }
}