using RSAForge.Models;

namespace RSAForge.Services;

public interface IRsaService
{
    RsaKeyPair GenerateKeyPair(int keySize, string format);
    bool ValidateKeyPair(string publicKey, string privateKey, string testPhrase);
    
    string FormatKey(string key, bool stripHeaders, bool isSingleLine);
}