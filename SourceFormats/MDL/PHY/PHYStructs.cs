using System.Numerics;

namespace SourceFormats.MDL.PHY
{
    public struct PHYHeader
    {
        public int Size;
        public int ID;
        public int SolidCount;
        public long CheckSum;
    }

    // There will be a series of these sections, back-to-back, numbering the same as the header's solidCount. (https://developer.valvesoftware.com/wiki/PHY)
    public struct PHYSurface
    {
        public int Size;
        public int VPhysicsID; // ASCII For "VPHY"
        public short Verson;
        public short ModelType;
        public int SurfaceSize;
        public Vector3 DragAxisAreas;
        public int AxisMapSize;
    }
}
