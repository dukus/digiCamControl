namespace Canon.Eos.Framework
{
    public enum EosCompressLevel : byte
    {
        Unknown = 0xF,
        JpegUncompressed = 0,
        JpegCompression1 = 1,
        Normal = 2,
        Fine = 3,
        Lossless = 4,
        SuperFine = 5,
        JpegCompression6 = 6,
        JpegCompression7 = 7,
        JpegCompression8 = 8,
        JpegCompression9 = 9,
        JpegCompression10 = 10,
    }
}
