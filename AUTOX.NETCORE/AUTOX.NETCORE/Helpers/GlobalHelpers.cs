using System.Security.Cryptography;

namespace AUTOX.NETCORE.Helpers
{
    public static class GlobalHelpers
    {
        public static string GenerateOtp()
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
            return otp.ToString();
        }
    }
}
