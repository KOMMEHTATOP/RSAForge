using Microsoft.Win32;
using System.IO;

namespace RSAForge.Services;

public class FileService : IFileService
{
    public async Task SaveKeyPairAsync(string publicKey, string privateKey)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "RSA Key Pair Directory|*.folder", 
            FileName = "MyRsaKeys", 
            Title = "Выберите место и название для папки с ключами"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string targetPath = saveFileDialog.FileName;
            string? parentDirectory = Path.GetDirectoryName(targetPath);
            
            string folderAndFileName = Path.GetFileNameWithoutExtension(targetPath);

            if (parentDirectory != null)
            {
                string newFolder = Path.Combine(parentDirectory, folderAndFileName);

                try
                {
                    if (!Directory.Exists(newFolder))
                    {
                        Directory.CreateDirectory(newFolder);
                    }
                    string privateKeyPath = Path.Combine(newFolder, $"{folderAndFileName}.key");
                    string publicKeyPath = Path.Combine(newFolder, $"{folderAndFileName}.pub");

                    await File.WriteAllTextAsync(privateKeyPath, privateKey);
                    await File.WriteAllTextAsync(publicKeyPath, publicKey);
                }
                catch (Exception)
                {
                    // В будущем тут будет вызов уведомления через UI
                    throw;
                }
            }
        }
    }
}