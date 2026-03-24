namespace DuplicationFinder.Core;

public enum WorkingMode
{
    DryRun = 1,
    Normal = 2
}

public record DuplicationStatistic(int MaxDuplicationCountForAfile, int UniqueDuplicatedFileCount, int TotalDuplicationCount, int DeletedFileCount, WorkingMode Mode)
{
    public override string ToString()
    {
        string deletionLabel = Mode == WorkingMode.DryRun ? "Files That Would Be Deleted" : "Files Actually Deleted";
        return $"""
               =======================
               Duplication Statistics
               =======================
               Unique Files with Duplicates: {UniqueDuplicatedFileCount}
               Total Redundant Copies Found: {TotalDuplicationCount}
               Max Redundancy for a Single File: {MaxDuplicationCountForAfile}
               {deletionLabel}: {DeletedFileCount}
               """;
    }
}
