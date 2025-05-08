namespace ExpenseSplitter.Shared.Utils;

public static class AuthUtils
{
    public static string HashPassword(string password)
    {
        // Generate a hash with a default work factor of 11
        // The salt is automatically generated and embedded in the hash
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}