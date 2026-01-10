using System;

namespace Domains.Services.Security
{
    public class Authentication
    {
        private string _secret;

        public string Secret
        {
            get => _secret;
            set 
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 32)
                {
                    throw new ArgumentException("Secret must be at least 256 bits (32 bytes).", nameof(Secret));                    
                }

                _secret = value;
            }
        }
    }
}