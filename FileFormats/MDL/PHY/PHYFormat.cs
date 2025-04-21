namespace SourceFormats.MDL.PHY
{
    /// <summary>
    /// Valve's Source 1 .phy physics format.
    /// </summary>
    public class PHYFormat
    {
        public PHYHeader Header;
        public PHYFormat(string path)
        {
            Header = new PHYHeader();

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Read the header
                Header.Size = reader.ReadInt32();
                Header.ID = reader.ReadInt32();
                Header.SolidCount = reader.ReadInt32();
                Header.CheckSum = reader.ReadInt64();
            }
        }
    }
}
