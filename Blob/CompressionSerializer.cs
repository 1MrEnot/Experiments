namespace Blob;

using System.IO.Compression;
using Microsoft.IO;
using ProtoBuf;

public static class ProtoBufSerializerWithCompress
{
    private static readonly RecyclableMemoryStreamManager Manager = new();
    
    public static byte[] Serialize<TContent>(TContent obj, CompressionLevel level=CompressionLevel.Optimal)
    {
        using var ms = new RecyclableMemoryStream(Manager);
        using var gzipStream = new GZipStream(ms, level);
        Serializer.Serialize(gzipStream, obj);
        
        gzipStream.Flush();

        return ms.ToArray();
    }

    public static T Deserialize<T>(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return Deserialize<T>(ms);
    }

    public static T Deserialize<T>(Stream stream)
    {
        using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        return Serializer.Deserialize<T>(gzipStream);
    }

    public static T DeepCopy<T>(T content)
    {
        using var ms = new RecyclableMemoryStream(Manager);
        Serializer.Serialize(ms, content);
        
        ms.Seek(0, SeekOrigin.Begin);
        return Serializer.Deserialize<T>(ms);
    }
}
