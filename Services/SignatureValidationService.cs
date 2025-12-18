using System.Security.Cryptography;
using System.Text;

namespace YusurIntegration.Services
{
    public class SignatureValidationService
    {
        private readonly IConfiguration _config;
        private readonly string _secret;
        public SignatureValidationService(IConfiguration config)
        {
            _config = config;
            _secret = _config.GetValue<string>("Yusur:SecretKey");
        }

        //public bool Validate(string incoming)
        //{
        //    // simple equality check for now
        //    return !string.IsNullOrEmpty(_secret) && incoming == _secret;
        //}
        //public bool VerifyWebhookSignature(string body, string incomingSig, string secret)
        //{
        //    if (string.IsNullOrEmpty(incomingSig) || string.IsNullOrEmpty(secret))
        //        return false;

        //    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        //    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        //    var expected = "sha256=" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        //    // Constant-time comparison (available in .NET Core 2.1+)
        //    return CryptographicOperations.FixedTimeEquals(
        //        Encoding.UTF8.GetBytes(expected),
        //        Encoding.UTF8.GetBytes(incomingSig)
        //    );
        //}
        public bool Validate(string incoming)
        {
            if (string.IsNullOrEmpty(_secret) || string.IsNullOrEmpty(incoming))
                return false;

            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(_secret),
                Encoding.UTF8.GetBytes(incoming)
            );
        }
    }
}
