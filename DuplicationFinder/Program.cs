using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        var workingMode = WorkingMode.Normal;
        var maxDuplicationCountForAFile = 0;
        var totalDuplicationCount = 0;
        var uniqueDuplicatedFileCount = 0;
        var deletedFileCount = 0;
        var searchOption = SearchOption.TopDirectoryOnly;

        if (args.Length < 1)
        {
            Console.WriteLine("Unsufficent argument count!");
            Console.WriteLine("Usage: DuplicationFinder <path> [--dry-run|-d] [--recursive|-r]");
            return;
        }

        var folderPath = args.Last();
        if(!Directory.Exists(folderPath))
        {
             Console.WriteLine($"The folder '{folderPath}' does not exist.");
             return;
        }

        foreach (var arg in args.Take(args.Length - 1))
        {
            if (arg == "--dry-run" || arg == "-d")
            {
                workingMode = WorkingMode.DryRun;
            }
            else if (arg == "--recursive" || arg == "-r") 
            {
                searchOption = SearchOption.AllDirectories;
            }
        }

        var duplicatedFileGroups = FindDuplicateFiles(folderPath, searchOption);

        uniqueDuplicatedFileCount = duplicatedFileGroups.Count;

        foreach (var group in duplicatedFileGroups)
        {
            /*
             * 1 is substructed because one of the files 
             * is considered the orginal not the duplication
             */

            totalDuplicationCount += group.Count - 1;

            if (group.Count - 1 > maxDuplicationCountForAFile)
            {
                maxDuplicationCountForAFile = group.Count - 1;
            }

        }

        for (var i = 0; i < duplicatedFileGroups.Count; i++)
        {
            duplicatedFileGroups[i] = duplicatedFileGroups[i].OrderBy(g=>g.Length).ToList();
        }

        foreach (var group in duplicatedFileGroups)
        {
            var filesToBeDeletedInGroup = group.Skip(1);
            foreach (var file in filesToBeDeletedInGroup)
            {
                switch (workingMode)
                {
                    case WorkingMode.DryRun:
                        Console.WriteLine($"The file would be deleted : {file}");
                        deletedFileCount++;
                        break;
                    case WorkingMode.Normal:
                        File.Delete(file);
                        Console.WriteLine($"The file has been deleted : {file}");
                        deletedFileCount++;
                        break;
                    default:
                        throw new NotImplementedException("Unsupported working mode");
                }
            }
        }

        DuplicationStatistic statistic = new(maxDuplicationCountForAFile, uniqueDuplicatedFileCount, totalDuplicationCount, deletedFileCount);
        
        Console.WriteLine($"\n\n\n{statistic}");

    }

    static List<List<string>> FindDuplicateFiles(string folderPath, SearchOption searchOption)
    {
        var fileHashes = new Dictionary<string, List<string>>();
        var filePaths = Directory.GetFiles(folderPath, "*.*", searchOption);

        foreach (var filePath in filePaths)
        {
            string fileHash = GetFileHash(filePath);

            if (fileHashes.ContainsKey(fileHash))
            {
                fileHashes[fileHash].Add(filePath);
            }
            else
            {
                fileHashes[fileHash] = new List<string> { filePath };
            }
        }

        return fileHashes.Values.Where(filePathListForSameHash => filePathListForSameHash.Count > 1).ToList();
    }

    static string GetFileHash(string filePath)
    {
        using (var hashAlgorithm = SHA256.Create())
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(fileStream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}

enum WorkingMode
{
    DryRun = 1,
    Normal = 2
}

record DuplicationStatistic(int MaxDuplicationCountForAfile, int UniqueDuplicatedFileCount, int TotalDuplicationCount, int DeletedFileCount);
