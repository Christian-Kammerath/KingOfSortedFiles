using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using KingOfSortedFiles;
using KingOfSortedFiles.UiElements;

namespace TestProject1;

public class SortingTest
{

    public SortingSettings SortingSettings { get; set; } = null!;


    public void SetUp()
    {
        SortingSettings = new SortingSettings();
        SortingSettings.SortCheckBoxes = GetSortCheckBoxes();
        SortingSettings.SearchByCheckBoxes!.SearchTags = new CheckBox() { IsChecked = false };
        SortingSettings.SearchByCheckBoxes.FileExtensions = new CheckBox() { IsChecked = false };
        SortingSettings.SourceDirectoryPathList.Add(Path.Combine(Directory.GetCurrentDirectory(),"TestDirectory","Source"));
        SortingSettings.TargetDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory","Target");
        SortingSettings.MoveAndOrCopy =
            new MoveAndOrCopy(new CheckBox() { IsChecked = false }, new CheckBox() { IsChecked = false });
        SortingSettings.SearchTagList = new SearchTags(new ListBox());

    }

    public SortCheckBoxes GetSortCheckBoxes(bool createdOne = false,bool changedOne = false,bool lastAccessTimeOne = false,
        bool fileExtensionOne = false, bool searchTagOne = false, bool createdTwo = false, bool changedTwo = false, bool lastAccessTimeTwo = false,
        bool fileExtensionTwo = false, bool searchTagTwo = false)
    {
        return new SortCheckBoxes(new CheckBox() { IsChecked = createdOne },
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
    
    [Fact]
    public void Checks_Whether_A_Folder_Exists_If_Not_It_Should_Be_Created()
    {
        //Arrange
        SetUp();
        var sorting = new Sorting(SortingSettings);

        //Act
        for (int i = 0; i < 10; i++)
        {
            var randomNumber = new Random().Next(1000, 5000).ToString();
            var toCreateDirectoryPath = Path.Combine(SortingSettings.TargetDirectoryPath, $"TestFolder{randomNumber}");
            var alreadyExistsDirectoryPath = Path.Combine(SortingSettings.TargetDirectoryPath, "Already_Exists");

            sorting.IsNotPresentCreateTargetDirectory(toCreateDirectoryPath);
            var alreadyExistsDirectoryInfo = sorting.IsNotPresentCreateTargetDirectory(alreadyExistsDirectoryPath);
            
            //Assert
            Assert.True(Directory.Exists(toCreateDirectoryPath));
            Assert.True(alreadyExistsDirectoryInfo is DirectoryInfo);
            
            Directory.Delete(toCreateDirectoryPath);
        }
        
    }

    [Fact]
    public void Generates_Target_Directory_Path_Based_On_CheckboxTags()
    {
        // Arrange
        SetUp();
        var searchTagListBox = new ListBox();
        
        SortingSettings.FileExtensionList = new List<string>() { ".pdf", ".txt" };
        
        var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory", "Target");
        
        var createdCheckBox = new CheckBox() { Tag = "Created" };
        var changedCheckBox = new CheckBox() { Tag = "changed" };
        var lastAccessTimeCheckBox = new CheckBox() { Tag = "LastAccessTime" };
        var fileExtensionCheckBox = new CheckBox() { Tag = "FileExtension" };
        var searchTagCheckBox = new CheckBox() { Tag = "SearchTag" };
        
        
        for (int i = 0; i < 10; i++)
        {
            var fileName = $"Test{i}.txt";

            if(i < 5)
                searchTagListBox.Items.Add(new SearchTagTab(fileName));

            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory", "Source",fileName);
            File.Create(sourcePath);
        }
        
        SortingSettings.SearchTagList = new SearchTags(searchTagListBox);
        var sort = new Sorting(SortingSettings);


        var fileList = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),
            "TestDirectory", "Source"),"*",SearchOption.AllDirectories).Select(d => new FileInfo(d)).ToList();


        // Act

        foreach (var files in fileList)
        {
            var createdResult = sort.CreateTargetDirectoryPath(files, targetPath, createdCheckBox);
            var changedResult = sort.CreateTargetDirectoryPath(files, targetPath, changedCheckBox);
            var lastAccessTimeResult = sort.CreateTargetDirectoryPath(files, targetPath, lastAccessTimeCheckBox);
            var fileExtensionResult = sort.CreateTargetDirectoryPath(files, targetPath, fileExtensionCheckBox);
            var searchTagResult = sort.CreateTargetDirectoryPath(files, targetPath, searchTagCheckBox);
            
            //Assert
            Assert.True(Path.Combine(targetPath,DateTime.Now
                .ToString("yyyy-MM-dd",CultureInfo.CurrentCulture)) == createdResult);
            
            Assert.True(Path.Combine(targetPath,DateTime.Now
                .ToString("yyyy-MM-dd",CultureInfo.CurrentCulture))==changedResult);
            
            Assert.True(Path.Combine(targetPath,DateTime.Now
                .ToString("yyyy-MM-dd",CultureInfo.CurrentCulture))==lastAccessTimeResult);

            Assert.Contains(fileExtensionResult, SortingSettings.FileExtensionList.Select(e => Path.Combine(targetPath, e.TrimStart('.'))));
            
            if(Regex.IsMatch(files.Name,"^[0-4]$"))
                Assert.Contains(searchTagResult, SortingSettings.SearchTagList.SearchTagList.Select(e => Path.Combine(targetPath,e) ));

        }
        
        
    }
    
}