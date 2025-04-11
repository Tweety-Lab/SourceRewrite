using FileFormats.MDL.VVD;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileFormats.MDL.VTX
{
    public class VTXFormat
    {
        public VTXHeader Header;
        public List<VTXBodyPart> BodyParts = new List<VTXBodyPart>();

        public VTXFormat(string path)
        {
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Read header
                Header = new VTXHeader
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

                // Read body parts
                reader.BaseStream.Seek(Header.BodyPartOffset, SeekOrigin.Begin);
                for (int i = 0; i < Header.NumBodyParts; i++)
                {
                    var bodyPart = new VTXBodyPart
                    {
                        NumModels = reader.ReadInt32(),
                        ModelOffset = reader.ReadInt32()
                    };
                    BodyParts.Add(bodyPart);
                }

                // Read models for each body part
                foreach (var bodyPart in BodyParts)
                {
                    reader.BaseStream.Seek(Header.BodyPartOffset + bodyPart.ModelOffset, SeekOrigin.Begin);
                    for (int i = 0; i < bodyPart.NumModels; i++)
                    {
                        var model = new VTXModel
                        {
                            NumLODs = reader.ReadInt32(),
                            LodOffset = reader.ReadInt32()
                        };
                        bodyPart.Models.Add(model);
                    }
                }

                // Read LODs for each model
                foreach (var bodyPart in BodyParts)
                {
                    foreach (var model in bodyPart.Models)
                    {
                        reader.BaseStream.Seek(Header.BodyPartOffset + bodyPart.ModelOffset + model.LodOffset, SeekOrigin.Begin);
                        for (int i = 0; i < model.NumLODs; i++)
                        {
                            var lod = new VTXModelLOD
                            {
                                NumMeshes = reader.ReadInt32(),
                                MeshOffset = reader.ReadInt32(),
                                SwitchPoint = reader.ReadSingle()
                            };
                            model.LODs.Add(lod);
                        }
                    }
                }

                // Read meshes for each LOD
                foreach (var bodyPart in BodyParts)
                {
                    foreach (var model in bodyPart.Models)
                    {
                        foreach (var lod in model.LODs)
                        {
                            reader.BaseStream.Seek(Header.BodyPartOffset + bodyPart.ModelOffset + model.LodOffset + lod.MeshOffset, SeekOrigin.Begin);
                            for (int i = 0; i < lod.NumMeshes; i++)
                            {
                                var mesh = new VTXMesh
                                {
                                    NumStripGroups = reader.ReadInt32(),
                                    StripGroupHeaderOffset = reader.ReadInt32(),
                                    Flags = (StripGroupFlags)reader.ReadByte()
                                };
                                lod.Meshes.Add(mesh);
                            }
                        }
                    }
                }

                // Read strip groups for each mesh
                foreach (var bodyPart in BodyParts)
                {
                    foreach (var model in bodyPart.Models)
                    {
                        foreach (var lod in model.LODs)
                        {
                            foreach (var mesh in lod.Meshes)
                            {
                                reader.BaseStream.Seek(
                                    Header.BodyPartOffset +
                                    bodyPart.ModelOffset +
                                    model.LodOffset +
                                    lod.MeshOffset +
                                    mesh.StripGroupHeaderOffset,
                                    SeekOrigin.Begin);

                                for (int i = 0; i < mesh.NumStripGroups; i++)
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
                                    mesh.StripGroups.Add(stripGroup);
                                }
                            }
                        }
                    }
                }

                // Read vertices, indices and strips for each strip group
                foreach (var bodyPart in BodyParts)
                {
                    foreach (var model in bodyPart.Models)
                    {
                        foreach (var lod in model.LODs)
                        {
                            foreach (var mesh in lod.Meshes)
                            {
                                foreach (var stripGroup in mesh.StripGroups)
                                {
                                    // Read vertices
                                    reader.BaseStream.Seek(
                                        Header.BodyPartOffset +
                                        bodyPart.ModelOffset +
                                        model.LodOffset +
                                        lod.MeshOffset +
                                        mesh.StripGroupHeaderOffset +
                                        stripGroup.VertOffset,
                                    SeekOrigin.Begin);

                                    stripGroup.Vertices = new List<VTXVertex>();
                                    for (int i = 0; i < stripGroup.NumVerts; i++)
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
                                        stripGroup.Vertices.Add(vertex);
                                    }

                                    // Read indices
                                    reader.BaseStream.Seek(
                                        Header.BodyPartOffset +
                                        bodyPart.ModelOffset +
                                        model.LodOffset +
                                        lod.MeshOffset +
                                        mesh.StripGroupHeaderOffset +
                                        stripGroup.IndexOffset,
                                        SeekOrigin.Begin);

                                    stripGroup.Indices = new List<ushort>();
                                    for (int i = 0; i < stripGroup.NumIndices; i++)
                                    {
                                        stripGroup.Indices.Add(reader.ReadUInt16());
                                    }

                                    // Read strips
                                    reader.BaseStream.Seek(
                                        Header.BodyPartOffset +
                                        bodyPart.ModelOffset +
                                        model.LodOffset +
                                        lod.MeshOffset +
                                        mesh.StripGroupHeaderOffset +
                                        stripGroup.StripOffset,
                                        SeekOrigin.Begin);

                                    for (int i = 0; i < stripGroup.NumStrips; i++)
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
                                        stripGroup.Strips.Add(strip);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}