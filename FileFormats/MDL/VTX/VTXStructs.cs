using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.MDL.VTX
{
    public struct VTXHeader
    {
        public int Version;
        public int VertCacheSize;

        public short MaxBonesPerStrip;
        public short MaxBonesPerTri;
        public int MaxBonesPerVert;

        public int Checksum;

        public int NumLODs;

        public int MaterialReplacementListOffset;

        public int NumBodyParts;
        public int BodyPartOffset;
    }

    public struct VTXBodyPart
    {
        public int NumModels;
        public int ModelOffset;
    }
}
