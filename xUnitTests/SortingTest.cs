using Avalonia.Controls;
using KingOfSortedFiles;

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
    
    

}