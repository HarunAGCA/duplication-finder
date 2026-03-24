# DuplicationFinder 🔍

DuplicationFinder is a powerful and user-friendly .NET 10 tool designed to find and manage duplicate files on your system. It identifies duplicates by comparing file content hashes, ensuring accuracy even if filenames differ.

The project provides two ways to interact with the tool:
1. **DuplicationFinder.WPF**: A modern graphical user interface for interactive use.
2. **DuplicationFinder**: A versatile command-line interface (CLI) for advanced users and automation.(Not tested with the latest changes)

## ✨ Features

- **Accurate Detection**: Uses robust hashing algorithms to find identical files.
- **Dry Run Mode**: Safely simulate deletion operations to see what would happen without modifying any files.
- **Recursive Scanning**: Optionally scan subdirectories for duplicates.
- **Smart Selection (WPF)**: Automatically protects the "original" file in each group while allowing easy selection of redundant copies.
- **Space Management**: Previews how much disk space will be reclaimed before you commit to deletions.
- **Cross-Interface Logic**: Core logic is shared in `DuplicationFinder.Core`, ensuring consistent behavior across CLI and GUI.

## 🛠 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Windows OS** (required for the WPF application)

## 🚀 Getting Started

### Installation

Clone the repository to your local machine:

```bash
git clone https://github.com/YourUsername/duplication-finder.git
cd duplication-finder
```

### Building the Project

Build the entire solution using the .NET CLI:

```bash
dotnet build
```

## 🖥 Using the WPF Application

The WPF app provides a visual experience for managing duplicates.

### How to Run:
```bash
dotnet run --project DuplicationFinder.WPF
```

### Steps:
1. Click **Browse** to select a target folder.
2. Toggle **Recursive Scan** if you want to include subdirectories.
3. Toggle **Dry Run Mode** (enabled by default for safety).
4. Click **Scan** to find duplicates.
5. Review the results, select the files you wish to remove, and click **Remove Selected**.

## 💻 Using the Command Line Interface

The CLI is ideal for quick scans or integration into scripts.

### How to Run:
```bash
dotnet run --project DuplicationFinder -- [options] <folder-path>
```

### Options:
- `-d`, `--dry-run`: Performs a simulation (no files are deleted).
- `-r`, `--recursive`: Scans all subdirectories within the target path.

### Example:
```bash
# Safely scan and simulate deletion in the Downloads folder recursively
dotnet run --project DuplicationFinder -- --dry-run --recursive "C:\Users\Username\Downloads"
```

## 🏗 Project Structure

- `DuplicationFinder.Core`: The engine that handles file scanning, hashing, and deletion logic.
- `DuplicationFinder.WPF`: Windows Desktop application built with WPF and MVVM.
- `DuplicationFinder`: Console application for terminal use.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
