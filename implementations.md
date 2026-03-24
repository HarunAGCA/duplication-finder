# Implementation Plan: DuplicationFinder to WPF

This document tracks the progress of converting the DuplicationFinder CLI tool into a modern WPF application.

## ✅ Phase 1: Core Refactoring
- [x] Create `DuplicationFinder.Core` project as a Class Library.
- [x] Move hashing and duplication detection logic to `DuplicationService`.
- [x] Move shared models (`WorkingMode`, `DuplicationStatistic`) to `Core`.
- [x] Update CLI project to reference `Core` and use the service.

## ✅ Phase 2: UI Foundation
- [x] Create `DuplicationFinder.WPF` project.
- [x] Implement Basic UI with Folder Picker.
- [x] Set up ViewModel and Command binding.

## ✅ Phase 3: Scanning & Results UI
- [x] Implement async scanning to prevent UI freeze.
- [x] Add Progress Bar for file hashing.
- [x] Visual Grid for duplicates with group selection.

## ✅ Phase 4: UX & Final Polish
- [x] Implement secure deletion with confirmation.
- [x] Add "Dry Run" visual cues.
- [x] Theme polish (Modern/Dark mode support).
