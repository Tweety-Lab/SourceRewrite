using Sledge.Formats.Texture.Wad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceFormats.MDL
{
    // MDL Header (studiohdr_t)
    public class MDLHeader
    {
        public int ID { get; set; } // FourCC (IDST)
        public int Version { get; set; }
        public int Checksum { get; set; }
        public string Name { get; set; } // Name of model as defined in the file

        public int DataLength; // Data size of MDL file in bytes

        public Vector3 EyePosition; // Position of player viewpoint relative to model origin (????)
        public Vector3 IllumPosition; // Position of light source relative to model origin
        public Vector3 HullMin; // Corner of model hull box with the least X/Y/Z values
        public Vector3 HullMax; // Opposite corner of model hull box
        public Vector3 ViewBBMin; // Minimum bounding box of model
        public Vector3 ViewBBMax; // Maximum bounding box of model

        public MDLFlags Flags; // Flags for the model

        // OFFSETS

        // Bone Data
        public int BoneCount;
        public int BoneOffset;

        // Bone Controller Data
        public int BoneControllerCount;
        public int BoneControllerOffset;

        // Hitbox Data
        public int HitboxCount;
        public int HitboxOffset;

        // Animation Data
        public int LocalAnimCount;
        public int LocalAnimOffset;

        // Sequence Data
        public int LocalSequenceCount;
        public int LocalSequenceOffset;

        public int ActivityListVersion; // ??
        public int EventsIndexed; // ??

        // VMT Materials
        // Texture Data
        public int TextureCount;
        public int TextureOffset;

        public int TextureDirCount;
        public int TextureDirOffset;

        // Skins
        public int SkinReferenceCount;
        public int SkinFamilyCount;
        public int SkinReferenceIndex;

        // Body Parts
        public int BodyPartCount;
        public int BodyPartOffset;

        // Attatchment Points
        public int AttachmentCount;
        public int AttachmentOffset;

        // Node Values
        public int LocalNodeCount;
        public int LocalNodeIndex;
        public int LocalNodeNameIndex;

        // mstudioflexdesc
        public int FlexDescCount;
        public int FlexDescIndex;

        // mstudioflexcontroller
        public int FlexControllerCount;
        public int FlexControllerIndex;

        // mstudioflexrule
        public int FlexRulesCount;
        public int FlexRulesIndex;

        // Probably inverse kinematics
        public int IKChainCount;
        public int IKChainIndex;

        // Information about mouth on the model for speech animations
        public int MouthsCount;
        public int MouthsIndex;

        // mstudioposeparamdesc
        public int LocalPoseParamCount;
        public int LocalPoseParamIndex;

        // We skip to byte 308

        // Surface Property value
        public int SurfacePropIndex;

        // Key Value data
        public int KeyValueIndex;
        public int KeyValueCount;

        // More Inverse Kinematics
        public int IKLockCount;
        public int IKLockIndex;

        public float Mass;

        public MDLHeader(BinaryReader reader)
        {
            // Read the header
            ID = reader.ReadInt32();
            Version = reader.ReadInt32();
            Checksum = reader.ReadInt32();

            // Read the name
            Name = new string(reader.ReadChars(64)).Trim('\0');
            DataLength = reader.ReadInt32();

            // Read Vectors, three 4-byte floats in a row
            EyePosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            IllumPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            HullMin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            HullMax = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            ViewBBMin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            ViewBBMax = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            // Read Model Flags
            Flags = (MDLFlags)reader.ReadUInt32();

            // Read Offsets
            BoneCount = reader.ReadInt32();
            BoneOffset = reader.ReadInt32();

            BoneControllerCount = reader.ReadInt32();
            BoneControllerOffset = reader.ReadInt32();

            HitboxCount = reader.ReadInt32();
            HitboxOffset = reader.ReadInt32();

            LocalAnimCount = reader.ReadInt32();
            LocalAnimOffset = reader.ReadInt32();

            LocalSequenceCount = reader.ReadInt32();
            LocalSequenceOffset = reader.ReadInt32();

            ActivityListVersion = reader.ReadInt32();
            EventsIndexed = reader.ReadInt32();

            TextureCount = reader.ReadInt32();
            TextureOffset = reader.ReadInt32();



            // This Offset Points to a series of ints
            // Each int value, in turn, is an offset relative to the start of the file
            // at which there is a null-terminated string
            TextureDirCount = reader.ReadInt32();
            TextureDirOffset = reader.ReadInt32();


            SkinReferenceCount = reader.ReadInt32();
            SkinFamilyCount = reader.ReadInt32();
            SkinReferenceIndex = reader.ReadInt32();

            BodyPartCount = reader.ReadInt32();
            BodyPartOffset = reader.ReadInt32();

            AttachmentCount = reader.ReadInt32();
            AttachmentOffset = reader.ReadInt32();

            LocalNodeCount = reader.ReadInt32();
            LocalNodeIndex = reader.ReadInt32();
            LocalNodeNameIndex = reader.ReadInt32();

            FlexDescCount = reader.ReadInt32();
            FlexDescIndex = reader.ReadInt32();

            FlexControllerCount = reader.ReadInt32();
            FlexControllerIndex = reader.ReadInt32();

            FlexRulesCount = reader.ReadInt32();
            FlexRulesIndex = reader.ReadInt32();

            IKChainCount = reader.ReadInt32();
            IKChainIndex = reader.ReadInt32();

            MouthsCount = reader.ReadInt32();
            MouthsIndex = reader.ReadInt32();

            LocalPoseParamCount = reader.ReadInt32();
            LocalPoseParamIndex = reader.ReadInt32();

            SurfacePropIndex = reader.ReadInt32();

            KeyValueIndex = reader.ReadInt32();
            KeyValueCount = reader.ReadInt32();

            IKLockCount = reader.ReadInt32();
            IKLockIndex = reader.ReadInt32();

            // Read Mass
            Mass = reader.ReadSingle();
        }

        /// <summary>
        /// Read a null terminated string as stored in the MDL Header.
        /// </summary>
        public static string ReadNullTerminatedString(BinaryReader reader, int maxLength)
        {
            var bytes = new List<byte>();
            for (int i = 0; i < maxLength; i++)
            {
                byte b = reader.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }
            return Encoding.ASCII.GetString(bytes.ToArray());
        }

    }
}
