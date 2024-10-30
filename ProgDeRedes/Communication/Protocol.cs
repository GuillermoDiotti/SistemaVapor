using System.Text;

namespace Communication
{
    public static class Protocol
    {
        public static readonly int FixedDataSize = 4;

        public const int FixedFileSize = 8;
        public const int MaxPacketSize = 32768; //32KB

        public static async Task<long> CalculateFileParts(long fileSize)
        {
            var fileParts = fileSize / MaxPacketSize;
            return await Task.Run(()=> fileParts * MaxPacketSize == fileSize ? fileParts : fileParts + 1);
        }
    }
}