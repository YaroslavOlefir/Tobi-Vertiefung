using System.Security.Cryptography;
using System.Text;

namespace cerberus_pass;
public static class VaultEncryption
{
  public static byte[] DeriveKeyFromPassword(string password, byte[] salt, int keysize = 32, int iterations = 10_000)
  {
    var pbkdf2Bytes = Rfc2898DeriveBytes.Pbkdf2(
      password,
      salt,
      iterations,
      HashAlgorithmName.SHA256,
      keysize
    );
    return pbkdf2Bytes;
  }

  public static string HashPassword(string password, out string salt)
  {
    byte[] saltBytes = new byte[16];
    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(saltBytes);
    }
    salt = Convert.ToBase64String(saltBytes);
    var pbkdf2Bytes = Rfc2898DeriveBytes.Pbkdf2(
      password,
      saltBytes,
      10_000,
      HashAlgorithmName.SHA256,
      32
    );
    return Convert.ToBase64String(pbkdf2Bytes);
  }

  public static bool VerifyPassword(string enteredPassword, string hashedPassword, string salt)
  {
    byte[] saltBytes = Convert.FromBase64String(salt);
    var pbkdf2Bytes = Rfc2898DeriveBytes.Pbkdf2(
      enteredPassword,
      saltBytes,
      10_000,
      HashAlgorithmName.SHA256,
      32
    );
    var enteredHashedPassword = Convert.ToBase64String(pbkdf2Bytes);

    return enteredHashedPassword == hashedPassword;
  }

  public static string Encrypt(string plainText, string password)
  {
    byte[] salt = new byte[16];
    byte[] iv = new byte[16];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(salt);
      rng.GetBytes(iv);
    }

    byte[] key = DeriveKeyFromPassword(password, salt);

    using (var aesAlg = Aes.Create())
    {
      aesAlg.Key = key;
      aesAlg.IV = iv;
      aesAlg.Mode = CipherMode.CBC;
      aesAlg.Padding = PaddingMode.PKCS7;

      using (var encryptor = aesAlg.CreateEncryptor())
      {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(
          plainBytes,
          0,
           plainBytes.Length);

        // Gesammten verschlüsselten Daten abspeichern: Salt [16] + IV [16] + Ciphertext [N]
        byte[] encryptedData = new byte[salt.Length + iv.Length + cipherBytes.Length];

        // Kopiere Salt
        Buffer.BlockCopy(
          salt,
        0,
        encryptedData,
        0,
        salt.Length
        );

        // Kopiere IV
        Buffer.BlockCopy(
          iv,
          0,
          encryptedData,
          salt.Length,
          iv.Length
        );

        // Kopier Cipher-Text
        Buffer.BlockCopy(
          cipherBytes,
          0,
          encryptedData,
          salt.Length + iv.Length,
          cipherBytes.Length
        );

        return Convert.ToBase64String(encryptedData);
      }
    }
  }

  // todo: Decrypt
}