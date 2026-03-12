# KingOfSortedFiles

A cross-platform desktop application for intelligent file organization and automation, built with .NET and Avalonia UI.

![KingOfSortedFiles Screenshot](https://github.com/Christian-Kammerath/KingOfSortedFiles/assets/35398348/9114ed25-a062-4031-9dbd-be04b19d8d7e)

## Features

- **Smart File Sorting**: Automatically organize files based on multiple criteria
  - Sort by creation date, modification date, or last access time
  - Sort by file extension
  - Sort by custom search tags (regex-based)
  
- **Flexible Organization**: 
  - Single or dual-level sorting (e.g., sort by date, then by file type)
  - Multiple source directories support
  - Automatic subfolder creation based on sorting parameters

- **Safe Operations**:
  - Choose between moving or copying files
  - Automatic duplicate handling with unique IDs
  - Directory verification before operations

- **Cross-Platform**:
  - Runs on Windows, Linux, and macOS
  - Native look and feel on each platform

- **Customizable**:
  - Add custom file extensions
  - Define custom search patterns
  - Configurable through JSON settings

## Tech Stack

- **.NET 10.0** - Modern cross-platform framework
- **Avalonia UI 11.0** - Cross-platform XAML-based UI framework
- **ReactiveUI** - MVVM framework for reactive programming
- **xUnit** - Unit testing framework with full test coverage

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Christian-Kammerath/KingOfSortedFiles.git
   cd KingOfSortedFiles
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run --project KingOfSortedFiles
   ```

## How to Use

1. **Configure Search Method**:
   - Select whether to search by file extensions, search tags, or both

2. **Select File Types** (optional):
   - Click on file extensions to filter (e.g., `.pdf`, `.txt`, `.jpg`)
   - Add custom extensions via the Settings button

3. **Choose Sorting Parameters**:
   - Primary sort: Select first sorting criterion (date, extension, or tag)
   - Secondary sort (optional): Add second-level organization

4. **Select Operation Mode**:
   - **Move**: Transfer files to target directory
   - **Copy then Move**: Create copies before organizing

5. **Add Search Tags** (if using tag-based sorting):
   - Define regex patterns to match filenames
   - Multiple tags supported

6. **Select Directories**:
   - Source: Choose one or more folders to organize (multiple selection supported)
   - Target: Select destination folder (single selection)

7. **Start Sorting**:
   - Click the sort button to begin automatic organization
   - Progress bar shows current status

## Running Tests

The project includes comprehensive unit tests for core functionality:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~Checks_Whether_A_Folder_Exists"
```

### Test Coverage

- Directory creation and validation
- File sorting logic with various parameters
- Search functionality (extensions, tags, combined)
- Duplicate file handling
- Multi-level sorting scenarios

## Architecture

```
KingOfSortedFiles/
├── KingOfSortedFiles/          # Main application
│   ├── ViewModels/             # MVVM ViewModels
│   ├── Views/                  # UI Views (AXAML)
│   ├── UiElements/             # Custom UI components
│   ├── Sorting.cs              # Core sorting logic
│   ├── DirectorySearch.cs      # File system search
│   ├── CustomLogSystem.cs      # Logging functionality
│   └── appSettings.json        # Configuration
└── xUnitTests/                 # Unit tests
    ├── SortingTest.cs
    ├── CustomLogSystemTest.cs
    └── DriveInfoLinuxTest.cs
```

## Key Components

### Sorting Engine
- **Pattern Matching**: Regex-based file filtering
- **Smart Naming**: GUID-based unique file identifiers to prevent conflicts
- **Flexible Operations**: Move or copy modes with automatic error handling

### Custom Logging
- Color-coded log levels (Debug, Info, Warning, Error)
- File and UI output support
- Real-time feedback during operations

### Settings Management
- JSON-based configuration
- Persistent user preferences
- Customizable file extensions and search patterns

## Configuration

Settings are stored in `appSettings.json` and include:
- File extension filters
- Custom search tags
- Default sorting parameters
- UI preferences

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Author

**Christian Kammerath**
- GitHub: [@Christian-Kammerath](https://github.com/Christian-Kammerath)
- LinkedIn: [Christian Kammerath](https://linkedin.com/in/Christian-Kammerath)

## Acknowledgments

- Built with [Avalonia UI](https://avaloniaui.net/)
- Inspired by the need for efficient file organization workflows
