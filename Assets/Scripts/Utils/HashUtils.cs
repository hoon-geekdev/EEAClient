using System;
using System.IO;
using System.Security.Cryptography;

public class HashUtils {
    static public string GetMD5Hash(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    static public string GetMD5Hash(byte[] data)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
