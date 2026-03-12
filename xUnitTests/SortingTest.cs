using System.Globalization;
using Avalonia.Controls;
using KingOfSortedFiles;
using KingOfSortedFiles.UiElements;

namespace TestProject1;

public class SortingTest : IDisposable
{
    public SortingSettings SortingSettings { get; set; } = null!;
    private readonly string _testDirectoryBase;
    private readonly string _sourceDirectory;
    private readonly string _targetDirectory;

    // Constructor - runs BEFORE each test (Setup)
    public SortingTest()
    {
        _testDirectoryBase = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory");
        _sourceDirectory = Path.Combine(_testDirectoryBase, "Source");
        _targetDirectory = Path.Combine(_testDirectoryBase, "Target");

        Directory.CreateDirectory(_sourceDirectory);
        Directory.CreateDirectory(_targetDirectory);
    }

    // Dispose - runs AFTER each test (Teardown)
    public void Dispose()
    {
        if (Directory.Exists(_testDirectoryBase))
            Directory.Delete(_testDirectoryBase, true);
    }

    public void SetUp()
    {
        SortingSettings = new SortingSettings();
        SortingSettings.SortCheckBoxes = GetSortCheckBoxes();
        SortingSettings.SearchByCheckBoxes!.SearchTags = new CheckBox() { IsChecked = false };
        SortingSettings.SearchByCheckBoxes.FileExtensions = new CheckBox() { IsChecked = false };
        SortingSettings.SourceDirectoryPathList.Add(_sourceDirectory);
        SortingSettings.TargetDirectoryPath = _targetDirectory;
        SortingSettings.MoveAndOrCopy =
            new MoveAndOrCopy(new CheckBox() { IsChecked = false }, new CheckBox() { IsChecked = false });
        SortingSettings.SearchTagList = new SearchTags(new ListBox());
    }

    public SortCheckBoxes GetSortCheckBoxes(bool createdOne = false, bool changedOne = false,
        bool lastAccessTimeOne = false, bool fileExtensionOne = false, bool searchTagOne = false,
        bool createdTwo = false, bool changedTwo = false, bool lastAccessTimeTwo = false,
        bool fileExtensionTwo = false, bool searchTagTwo = false)
    {
        return new SortCheckBoxes(
            new CheckBox() { IsChecked = createdOne },
            new CheckBox() { IsChecked = changedOne },
            new CheckBox() { IsChecked = lastAccessTimeOne },
            new CheckBox() { IsChecked = fileExtensionOne },
            new CheckBox() { IsChecked = searchTagOne },
            new CheckBox() { IsChecked = createdTwo },
            new CheckBox() { IsChecked = changedTwo },
            new CheckBox() { IsChecked = lastAccessTimeTwo },
            new CheckBox() { IsChecked = fileExtensionTwo },
            new CheckBox() { IsChecked = searchTagTwo });
    }

    // Creates the 6 standard test files in source with a specific timestamp
    private static void CreateSourceFiles(string sourcePath, DateTime timestamp)
    {
        var fileNames = new[] { "Invoice.txt", "Invoice.pdf", "Invoice.mp4", "Booking.txt", "Booking.pdf", "Booking.mp4" };
        foreach (var name in fileNames)
        {
            var filePath = Path.Combine(sourcePath, name);
            using (File.Create(filePath)) { }
            File.SetCreationTime(filePath, timestamp);
            File.SetLastWriteTime(filePath, timestamp);
            File.SetLastAccessTime(filePath, timestamp);
        }
    }

    // Removes all content from target directory without deleting the directory itself
    private static void CleanTargetDirectory(string targetPath)
    {
        foreach (var dir in Directory.GetDirectories(targetPath))
            Directory.Delete(dir, true);
        foreach (var file in Directory.GetFiles(targetPath))
            File.Delete(file);
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void Checks_Whether_A_Folder_Exists_If_Not_It_Should_Be_Created()
    {
        //Arrange
        SetUp();
        var sorting = new Sorting(SortingSettings);
        var random = new Random(); // created ONCE outside the loop to avoid same-seed issue

        //Act & Assert
        for (int i = 0; i < 10; i++)
        {
            var randomNumber = random.Next(1000, 9999).ToString();
            var toCreatePath = Path.Combine(_targetDirectory, $"TestFolder{randomNumber}");
            var alreadyExistsPath = Path.Combine(_targetDirectory, "Already_Exists");

            // New directory: should be created and returned
            var created = sorting.IsNotPresentCreateTargetDirectory(toCreatePath);
            Assert.True(Directory.Exists(toCreatePath));
            Assert.Equal(toCreatePath, created.FullName);

            // Existing directory: should be returned as-is without error
            var firstCall = sorting.IsNotPresentCreateTargetDirectory(alreadyExistsPath);
            var secondCall = sorting.IsNotPresentCreateTargetDirectory(alreadyExistsPath);
            Assert.Equal(firstCall.FullName, secondCall.FullName);

            Directory.Delete(toCreatePath);
        }
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void Generates_Target_Directory_Path_Based_On_CheckboxTags()
    {
        // Arrange
        SetUp();
        var searchTagListBox = new ListBox();

        SortingSettings.FileExtensionList = new List<string>() { ".pdf", ".txt" };
        var targetPath = _targetDirectory;

        var createdCheckBox        = new CheckBox() { Tag = "Created" };
        var changedCheckBox        = new CheckBox() { Tag = "changed" };
        var lastAccessTimeCheckBox = new CheckBox() { Tag = "LastAccessTime" };
        var fileExtensionCheckBox  = new CheckBox() { Tag = "FileExtension" };
        var searchTagCheckBox      = new CheckBox() { Tag = "SearchTag" };

        // Create test files — Test0.txt to Test4.txt are registered as search tags
        var subDirectory = Path.Combine(_sourceDirectory, "Generates_Target_Directory");
        Directory.CreateDirectory(subDirectory);

        for (int i = 0; i < 10; i++)
        {
            var fileName = $"Test{i}.txt";
            if (i < 5)
                searchTagListBox.Items.Add(new SearchTagTab(fileName));
            using (File.Create(Path.Combine(subDirectory, fileName))) { }
        }

        SortingSettings.SearchTagList = new SearchTags(searchTagListBox);
        var sort = new Sorting(SortingSettings);

        var fileList = Directory
            .GetFiles(subDirectory, "*", SearchOption.AllDirectories)
            .Select(d => new FileInfo(d))
            .ToList();

        // Act & Assert per file
        foreach (var file in fileList)
        {
            var todayPath = Path.Combine(targetPath, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture));

            var createdResult        = sort.CreateTargetDirectoryPath(file, targetPath, createdCheckBox);
            var changedResult        = sort.CreateTargetDirectoryPath(file, targetPath, changedCheckBox);
            var lastAccessTimeResult = sort.CreateTargetDirectoryPath(file, targetPath, lastAccessTimeCheckBox);
            var fileExtensionResult  = sort.CreateTargetDirectoryPath(file, targetPath, fileExtensionCheckBox);
            var searchTagResult      = sort.CreateTargetDirectoryPath(file, targetPath, searchTagCheckBox);

            // Date-based paths must match today's date (files were just created)
            Assert.Equal(todayPath, createdResult);
            Assert.Equal(todayPath, changedResult);
            Assert.Equal(todayPath, lastAccessTimeResult);

            // Extension path must match one of the registered extensions
            Assert.Contains(fileExtensionResult,
                SortingSettings.FileExtensionList.Select(e => Path.Combine(targetPath, e.TrimStart('.'))));

            // SearchTag path: tagged files → tag-subfolder, untagged → could-not-be-classified
            var isTaggedFile = SortingSettings.SearchTagList.SearchTagList.Contains(file.Name);
            if (isTaggedFile)
                Assert.Contains(searchTagResult,
                    SortingSettings.SearchTagList.SearchTagList.Select(e => Path.Combine(targetPath, e)));
            else
                Assert.Equal(Path.Combine(targetPath, "could-not-be-classified"), searchTagResult);

            File.Delete(file.FullName);
        }
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void Searches_For_Files_Based_On_The_File_Extension_A_Search_Tag_Or_Both_Combined()
    {
        //Arrange
        SetUp();

        // Use known, explicit files instead of random ones — makes assertions deterministic
        var knownFiles = new[]
        {
            "5001Invoice.pdf",  // tag: Invoice  ext: .pdf  → hits: extension, tag, combined
            "5002Invoice.txt",  // tag: Invoice  ext: .txt  → hits: extension, tag, combined
            "5003Invoice.mp4",  // tag: Invoice  ext: .mp4  → hits: tag only (not .pdf/.txt)
            "5004Booking.pdf",  // tag: Booking  ext: .pdf  → hits: extension only
            "5005Booking.txt",  // tag: Booking  ext: .txt  → hits: extension only
            "5006Booking.mp3",  // tag: Booking  ext: .mp3  → hits: nothing
            "5007Billing.jpg",  // tag: Billing  ext: .jpg  → hits: nothing
            "5008Billing.txt",  // tag: Billing  ext: .txt  → hits: extension only
        };

        foreach (var name in knownFiles)
            using (File.Create(Path.Combine(_sourceDirectory, name))) { }

        SortingSettings.FileExtensionList = [".pdf", ".txt"];
        SortingSettings.SearchTagList.SearchTagList = ["Invoice"];

        // Act — called ONCE, not once per file
        var resultByExtension = new Sorting(SortingSettings).GetAllFiles(true, false, _sourceDirectory);
        var resultBySearchTag = new Sorting(SortingSettings).GetAllFiles(false, true, _sourceDirectory);
        var resultByCombined  = new Sorting(SortingSettings).GetAllFiles(true, true, _sourceDirectory);

        // Assert: filter by extension — positive
        Assert.NotEmpty(resultByExtension);
        Assert.All(resultByExtension, f => Assert.Contains(f.Extension, new[] { ".pdf", ".txt" }));

        // Assert: filter by extension — negative (non-matching extensions excluded)
        Assert.DoesNotContain(resultByExtension, f => f.Extension == ".mp4");
        Assert.DoesNotContain(resultByExtension, f => f.Extension == ".mp3");
        Assert.DoesNotContain(resultByExtension, f => f.Extension == ".jpg");

        // Assert: filter by search tag — positive
        Assert.NotEmpty(resultBySearchTag);
        Assert.All(resultBySearchTag, f => Assert.Matches("Invoice", f.Name));

        // Assert: filter by search tag — negative (Booking/Billing excluded)
        Assert.DoesNotContain(resultBySearchTag, f => f.Name.Contains("Booking"));
        Assert.DoesNotContain(resultBySearchTag, f => f.Name.Contains("Billing"));

        // Assert: combined filter — positive
        Assert.NotEmpty(resultByCombined);
        Assert.All(resultByCombined, f =>
        {
            Assert.Matches("Invoice", f.Name);
            Assert.Contains(f.Extension, new[] { ".pdf", ".txt" });
        });

        // Assert: combined filter — negative (Invoice.mp4 must be excluded, Booking/* excluded)
        Assert.DoesNotContain(resultByCombined, f => f.Extension == ".mp4");
        Assert.DoesNotContain(resultByCombined, f => f.Name.Contains("Booking"));

        // Assert: combined result must be smaller than both single-filter results
        Assert.True(resultByCombined.Count < resultByExtension.Count);
        Assert.True(resultByCombined.Count < resultBySearchTag.Count);
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void Sorts_Files_Into_Folders_And_Subfolders_According_To_Parameters_In_The_Target_Folder()
    {
        //Arrange
        SetUp();
        SortingSettings.MoveAndOrCopy.MoveOnly = true;
        SortingSettings.SearchTagList.SearchTagList = ["Invoice"];

        var testDate     = new DateTime(2020, 6, 15);
        var expectedDate = "2020-06-15";

        var checkBoxCreated    = new CheckBox() { IsChecked = true, Tag = "Created" };
        var checkBoxLastAccess = new CheckBox() { IsChecked = true, Tag = "LastAccessTime" };
        var checkBoxExtension  = new CheckBox() { IsChecked = true, Tag = "FileExtension" };
        var checkBoxSearchTag  = new CheckBox() { IsChecked = true, Tag = "SearchTag" };

        // --- Scenario 1: Sort by creation date ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxCreated, null);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "Booking.pdf")));
        Assert.False(File.Exists(Path.Combine(_targetDirectory, "Invoice.txt")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 2: Sort by last access time ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxLastAccess, null);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "Booking.mp4")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 3: Sort by file extension ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxExtension, null);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "txt", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "pdf", "Booking.pdf")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "mp4", "Invoice.mp4")));
        Assert.False(File.Exists(Path.Combine(_targetDirectory, "pdf", "Invoice.txt")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 4: Sort by search tag ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxSearchTag, null);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "Invoice", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "Invoice", "Invoice.pdf")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "could-not-be-classified", "Booking.txt")));
        Assert.False(File.Exists(Path.Combine(_targetDirectory, "Booking", "Booking.txt")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 5: Sort by search tag, then file extension ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxSearchTag, checkBoxExtension);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "Invoice", "txt", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "Invoice", "pdf", "Invoice.pdf")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "could-not-be-classified", "txt", "Booking.txt")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 6: Sort by file extension, then search tag ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxExtension, checkBoxSearchTag);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "txt", "Invoice", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "txt", "could-not-be-classified", "Booking.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "pdf", "Invoice", "Invoice.pdf")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 7: Sort by creation date, then file extension ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxCreated, checkBoxExtension);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "txt", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "pdf", "Booking.pdf")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "mp4", "Invoice.mp4")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 8: Sort by file extension, then creation date ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxExtension, checkBoxCreated);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "txt", expectedDate, "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "pdf", expectedDate, "Invoice.pdf")));
        CleanTargetDirectory(_targetDirectory);

        // --- Scenario 9: Sort by creation date, then search tag ---
        CreateSourceFiles(_sourceDirectory, testDate);
        new Sorting(SortingSettings).SortingTask(_sourceDirectory, checkBoxCreated, checkBoxSearchTag);
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "Invoice", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, expectedDate, "could-not-be-classified", "Booking.txt")));
        CleanTargetDirectory(_targetDirectory);
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void SortingTask_Does_Not_Crash_When_Both_CheckBoxes_Are_Null()
    {
        // Regression test: previously (bool)sortCheckBoxOne!.IsChecked was evaluated
        // BEFORE the null check, causing a NullReferenceException when no sort
        // parameter was selected in the UI.

        //Arrange
        SetUp();
        SortingSettings.MoveAndOrCopy.MoveOnly = true;
        CreateSourceFiles(_sourceDirectory, new DateTime(2020, 6, 15));
        var sorting = new Sorting(SortingSettings);

        //Act & Assert — must not throw
        var exception = Record.Exception(() => sorting.SortingTask(_sourceDirectory, null, null));
        Assert.Null(exception);

        // Files must remain untouched in source — nothing was sorted
        Assert.True(File.Exists(Path.Combine(_sourceDirectory, "Invoice.txt")));
        Assert.Empty(Directory.GetDirectories(_targetDirectory));
    }

    [Fact]
    public void SortingTask_Does_Not_Crash_When_Only_CheckBoxOne_Is_Null()
    {
        // Regression test: the second branch "sortCheckBoxTwo != null && sortCheckBoxOne == null"
        // previously crashed because (bool)sortCheckBoxTwo!.IsChecked was evaluated first.

        //Arrange
        SetUp();
        SortingSettings.MoveAndOrCopy.MoveOnly = true;
        CreateSourceFiles(_sourceDirectory, new DateTime(2020, 6, 15));

        var checkBoxExtension = new CheckBox() { IsChecked = true, Tag = "FileExtension" };
        var sorting = new Sorting(SortingSettings);

        //Act & Assert — must not throw, and must sort using checkBoxTwo
        var exception = Record.Exception(() => sorting.SortingTask(_sourceDirectory, null, checkBoxExtension));
        Assert.Null(exception);

        Assert.True(File.Exists(Path.Combine(_targetDirectory, "txt", "Invoice.txt")));
        Assert.True(File.Exists(Path.Combine(_targetDirectory, "pdf", "Invoice.pdf")));
    }
}
