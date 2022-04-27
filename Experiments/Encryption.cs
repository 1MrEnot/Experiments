namespace Experiments;

using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public static class Encryption
{
    private static readonly JsonSerializer Serializer = new();

    public static Stream Encrypt(ICryptoTransform encryptor, object obj)
    {
        var ms = new MemoryStream();
        var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        var writer = new BsonWriter(cryptoStream);
        Serializer.Serialize(writer, obj);
        return ms;
    }

    public static T Decrypt<T>(ICryptoTransform encryptor, Stream stream)
    {
        var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Read);
        var reader = new BsonReader(cryptoStream);
        return Serializer.Deserialize<T>(reader);
    }
}