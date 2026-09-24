using System;
using System.Security.Cryptography;

namespace ECommerce.Business.Services;

public static class PasswordHasher
{
    // PBKDF2 with SHA256
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    // Stored format: {iterations}.{saltBase64}.{hashBase64}
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public static bool Verify(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return false;

        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, iterations, HashAlgorithmName.SHA256, key.Length);

        return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
    }
}
