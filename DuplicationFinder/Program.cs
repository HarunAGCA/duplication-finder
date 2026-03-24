using System;
using System.IO;
using System.Linq;
using DuplicationFinder.Core;

class Program
{
    static void Main(string[] args)
    {
        var workingMode = WorkingMode.Normal;
        var searchOption = SearchOption.TopDirectoryOnly;

        if (args.Length < 1)
        {
            Console.WriteLine("Insufficient argument count!");
            Console.WriteLine("Usage: DuplicationFinder [--dry-run|-d] [--recursive|-r] <path>");
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

        var service = new DuplicationService();
        var duplicatedFileGroups = service.FindDuplicateFiles(folderPath, searchOption);

        int uniqueDuplicatedFileCount = duplicatedFileGroups.Count;
        int totalDuplicationCount = duplicatedFileGroups.Sum(group => group.Count - 1);
        int maxDuplicationCountForAFile = duplicatedFileGroups.DefaultIfEmpty().Max(group => group?.Count - 1 ?? 0);

        int deletedFileCount = DuplicationService.DeleteDuplicates(duplicatedFileGroups, workingMode, (msg, isDryRun) => Console.WriteLine(msg));

        if (uniqueDuplicatedFileCount == 0)
        {
            Console.WriteLine("\n\nNo duplicate files were found.");
            return;
        }

        DuplicationStatistic statistic = new(maxDuplicationCountForAFile, uniqueDuplicatedFileCount, totalDuplicationCount, deletedFileCount, workingMode);
        Console.WriteLine($"\n\n{statistic}");
    }
}
