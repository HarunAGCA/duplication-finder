using System.Security.Cryptography;

namespace DuplicationFinder.Core;

public class DuplicationService
{
    public List<List<string>> FindDuplicateFiles(string folderPath, SearchOption searchOption, IProgress<ScanProgress>? progress = null)
    {
        var fileHashes = new Dictionary<string, List<string>>();
        var filePaths = Directory.GetFiles(folderPath, "*.*", searchOption);
        int totalFiles = filePaths.Length;
        int processedCount = 0;

        foreach (var filePath in filePaths)
        {
            processedCount++;
            progress?.Report(new ScanProgress(processedCount, totalFiles, Path.GetFileName(filePath)));

            try
            {
                string fileHash = GetFileHash(filePath);

                if (fileHashes.TryGetValue(fileHash, out var filePathList))
                {
                    filePathList.Add(filePath);
                }
                else
                {
                    fileHashes[fileHash] = new List<string> { filePath };
                }
            }
            catch (IOException)
            {
                // Skip files that are in use or otherwise inaccessible
                continue;
            }
        }

        return fileHashes.Values
            .Where(filePathListForSameHash => filePathListForSameHash.Count > 1)
            .Select(group => group.OrderBy(f => f.Length).ToList()) // Order by length to keep the shortest path as original
            .ToList();
    }

    public static string GetFileHash(string filePath)
    {
        using var hashAlgorithm = SHA256.Create();
        using var fileStream = File.OpenRead(filePath);
        byte[] hashBytes = hashAlgorithm.ComputeHash(fileStream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public static int DeleteDuplicates(List<List<string>> duplicatedFileGroups, WorkingMode mode, Action<string, bool> logAction)
    {
        int deletedFileCount = 0;
        foreach (var group in duplicatedFileGroups)
        {
            var filesToBeDeletedInGroup = group.Skip(1);
            foreach (var file in filesToBeDeletedInGroup)
            {
                if (mode == WorkingMode.DryRun)
                {
                    logAction($"[DRY-RUN] File would be deleted: {file}", true);
                }
                else
                {
                    try
                    {
                        File.Delete(file);
                        logAction($"Deleted file: {file}", false);
                    }
                    catch (Exception ex)
                    {
                        logAction($"Failed to delete {file}: {ex.Message}", false);
                        continue;
                    }
                }
                deletedFileCount++;
            }
        }
        return deletedFileCount;
    }
}
