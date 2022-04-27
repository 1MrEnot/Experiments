using System.Security.Cryptography;
using System.Text;

var original = "Here is some data to encrypt!";

// Create a new instance of the Rijndael
// class.  This generates a new key and initialization
// vector (IV).
using var myAlgo = Aes.Create();

// Encrypt the string to an array of bytes.
var utf8Bytes = Encoding.UTF8.GetBytes(original);


var encrypted = EncryptStringToBytes(utf8Bytes, myAlgo);

// Decrypt the bytes to a string.
var roundtrip = DecryptStringFromBytes(encrypted, myAlgo);

var decodedUtf8 = Encoding.UTF8.GetString(roundtrip);

//Display the original data and the decrypted data.
Console.WriteLine("Original:   {0}", original);
Console.WriteLine("Round Trip: {0}", decodedUtf8);



static byte[] EncryptStringToBytes(byte[] bytes, SymmetricAlgorithm algo)
{
    var encryptor = algo.CreateEncryptor();

    using var msEncrypt = new MemoryStream();
    using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
    csEncrypt.Write(bytes);

    return msEncrypt.ToArray();
}

static byte[] DecryptStringFromBytes(byte[] cipherText, SymmetricAlgorithm algo)
{
    var decryptor = algo.CreateDecryptor();

    using var msDecrypt = new MemoryStream(cipherText);
    using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

    var buffer = new byte[16*1024];
    using var ms = new MemoryStream();
    int read;
    while ((read = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
    {
        ms.Write(buffer, 0, read);
    }
    return ms.ToArray();
}