namespace RSAForge.Services;

public interface IFileService
{
    Task SaveKeyPairAsync(string publicKey, string privateKey);
}