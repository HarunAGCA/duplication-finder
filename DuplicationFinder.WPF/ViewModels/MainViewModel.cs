using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DuplicationFinder.Core;
using Microsoft.Win32;
using System.Linq;

namespace DuplicationFinder.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DuplicationService _duplicationService = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    private string _selectedFolderPath = string.Empty;

    [ObservableProperty]
    private bool _isRecursive = true;

    [ObservableProperty]
    private bool _isDryRun = true;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _hasResults;

    [ObservableProperty]
    private string _statusMessage = "Select a folder to begin scanning for duplicates.";

    [ObservableProperty]
    private int _uniqueDuplicateCount;

    [ObservableProperty]
    private int _totalRedundantCopies;

    [ObservableProperty]
    private int _maxRedundancy;

    [ObservableProperty]
    private ObservableCollection<DuplicateGroupViewModel> _duplicateGroups = new();

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private string _progressText = string.Empty;

    [ObservableProperty]
    private string _currentScanningFile = string.Empty;

    [RelayCommand]
    private void BrowseFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Folder to Scan"
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedFolderPath = dialog.FolderName;
        }
    }

    private bool CanScan() => !string.IsNullOrWhiteSpace(SelectedFolderPath);

    [RelayCommand(CanExecute = nameof(CanScan))]
    private async Task ScanAsync()
    {
        if (!Directory.Exists(SelectedFolderPath))
        {
            StatusMessage = "⚠ The selected folder does not exist.";
            return;
        }

        IsScanning = true;
        HasResults = false;
        StatusMessage = "🔍 Scanning for duplicate files...";
        DuplicateGroups.Clear();

        try
        {
            var searchOption = IsRecursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var progress = new Progress<ScanProgress>(p =>
            {
                ProgressValue = (double)p.ProcessedFiles / p.TotalFiles * 100;
                ProgressText = $"{p.ProcessedFiles} / {p.TotalFiles}";
                CurrentScanningFile = p.CurrentFile;
            });

            var results = await Task.Run(() =>
                _duplicationService.FindDuplicateFiles(SelectedFolderPath, searchOption, progress));

            if (results.Count == 0)
            {
                StatusMessage = "✅ No duplicate files found!";
                HasResults = false;
                return;
            }

            // Update statistics
            UniqueDuplicateCount = results.Count;
            TotalRedundantCopies = results.Sum(g => g.Count - 1);
            MaxRedundancy = results.Max(g => g.Count - 1);

            // Populate groups
            int groupIndex = 1;
            foreach (var group in results)
            {
                var groupVm = new DuplicateGroupViewModel
                {
                    GroupNumber = groupIndex++,
                    FileHash = DuplicationService.GetFileHash(group[0])[..12] + "…",
                    FileSize = new FileInfo(group[0]).Length,
                    Files = new ObservableCollection<DuplicateFileViewModel>(
                        group.Select((f, i) => new DuplicateFileViewModel
                        {
                            FilePath = f,
                            FileName = Path.GetFileName(f),
                            Directory = Path.GetDirectoryName(f) ?? "",
                            IsOriginal = i == 0,
                            IsSelected = i > 0 // Pre-select duplicates, not original
                        }))
                };
                DuplicateGroups.Add(groupVm);
            }

            HasResults = true;
            StatusMessage = $"Found {UniqueDuplicateCount} duplicate group(s) with {TotalRedundantCopies} redundant copies.";
        }
        catch (UnauthorizedAccessException)
        {
            StatusMessage = "⚠ Access denied. Try running as administrator.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"⚠ Error: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
        }
    }

    public string FormatFileSize(long bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < suffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {suffixes[order]}";
    }
}

public partial class DuplicateGroupViewModel : ObservableObject
{
    [ObservableProperty]
    private int _groupNumber;

    [ObservableProperty]
    private string _fileHash = string.Empty;

    [ObservableProperty]
    private long _fileSize;

    [ObservableProperty]
    private ObservableCollection<DuplicateFileViewModel> _files = new();

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var file in Files.Where(f => !f.IsOriginal))
        {
            file.IsSelected = true;
        }
    }

    [RelayCommand]
    private void DeselectAll()
    {
        foreach (var file in Files.Where(f => !f.IsOriginal))
        {
            file.IsSelected = false;
        }
    }
}

public partial class DuplicateFileViewModel : ObservableObject
{
    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _directory = string.Empty;

    [ObservableProperty]
    private bool _isOriginal;

    [ObservableProperty]
    private bool _isSelected;
}
