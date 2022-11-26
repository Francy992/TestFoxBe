using System.Security.Cryptography;
using System.Text;

namespace TestTL.Extensions;

public static class StringExtensions
{
    public static string HashSha1(this string input)
    {
        var sha1 = SHA1.Create();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hash = sha1.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var t in hash)
        {
            sb.Append(t.ToString("X2"));
        }
        return sb.ToString();
    }
}