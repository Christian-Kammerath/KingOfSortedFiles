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

            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory", "Source","Generates_Target_Directory",fileName);
            File.Create(sourcePath);
        }
        
        SortingSettings.SearchTagList = new SearchTags(searchTagListBox);
        var sort = new Sorting(SortingSettings);


        var fileList = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),
            "TestDirectory", "Source","Generates_Target_Directory"),"*",SearchOption.AllDirectories).Select(d => new FileInfo(d)).ToList();

        
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

            
            File.Delete(files.FullName);
        }
        
    }

    [Fact]
    public void Searches_For_Files_Based_On_The_File_Extension_A_Search_Tag_Or_Both_Combined()
    {
        //Arrange 
        SetUp();

        var sourceDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory", "Source");
        
        var fileExtensionList = new List<string>
        {
            ".pdf",".txt",".asc",".doc",".odt",".xls",
            ".avi",".bmp",".dss",".gif",".jpeg",".mov",
            ".mp3",".mp4",".png",".ram",".rmvb",".tif",".wav",".wmf"
        };

        var searchTagList = new List<string>
        {
            "Booking",
            "Invoice",
            "Billing"
        };
        
        var randomFileList = new List<FileInfo>();

        for (int i = 0; i < 100; i++)
        {
            var random = new Random();
            var fileName = random.Next(5000, 10000);
            
            var filePath = Path.Combine(sourceDirectoryPath,
                $"{fileName}{searchTagList[random.Next(0, searchTagList.Count-1)]}{fileExtensionList[random.Next(0, fileExtensionList.Count-1)]}");

            File.Create(filePath);
            randomFileList.Add(new FileInfo(filePath));
        }
        
        //Act
        foreach (var file in randomFileList)
        {
            SortingSettings.FileExtensionList = [".pdf", ".txt"];
            var resultFilesWithFileExtension = new Sorting(SortingSettings)
                .GetAllFiles( true, false,sourceDirectoryPath);

            SortingSettings.SearchTagList.SearchTagList = ["Invoice"];
            var resultFilesWithSearchTag = new Sorting(SortingSettings)
                .GetAllFiles( false, true,sourceDirectoryPath);
            
            var resultFilesWithSearchTagAndFileExtension = new Sorting(SortingSettings)
                .GetAllFiles( true, true,sourceDirectoryPath);

            //Assert
            Assert.All(resultFilesWithFileExtension, element => 
                Assert.Contains(element.Extension, fileExtensionList));
            
            Assert.All(resultFilesWithSearchTag, element =>
                Assert.Matches("Invoice", element.Name));
            
            Assert.All(resultFilesWithSearchTagAndFileExtension,element =>
            {
                Assert.Matches("Invoice", element.Name);
                Assert.Contains(element.Extension, fileExtensionList);
            });
            
        }
        
        randomFileList.ForEach(f => File.Delete(f.FullName));
        
    }
    [Fact]
    public void Sorts_Files_Into_Folders_And_Subfolders_According_To_Parameters_In_The_Target_Folder()
    {
        //Arrange
        SetUp();

        
        

        var isSortingByCreateTime = new CheckBox() { IsChecked = true ,Tag = "Created"};
        var isSortingByLastAccessTime = new CheckBox() { IsChecked = true,Tag = "LastAccessTime"};
        var isSortingByFileExtension = new CheckBox() { IsChecked = true ,Tag = "FileExtension"};
        var isSortingBySearchTag = new CheckBox() { IsChecked = true,Tag = "SearchTag"};
        CheckBox isFalseCheckBox = null;
        
        var sourceTestFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory", "Source");
        var targetTestFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestDirectory", "Target");  

        SortingSettings.SearchTagList.SearchTagList = ["Invoice"];
        
        
        var testFiles = new List<FileInfo>()
        {
            new ( Path.Combine(sourceTestFilePath,"Invoice.txt")),
            new( Path.Combine(sourceTestFilePath,"Invoice.pdf")),
            new ( Path.Combine(sourceTestFilePath,"Invoice.mp4")),
            new ( Path.Combine(sourceTestFilePath,"Booking.txt")),
            new ( Path.Combine(sourceTestFilePath,"Booking.pdf")),
            new ( Path.Combine(sourceTestFilePath,"Booking.mp4"))
        };

        foreach (var file in testFiles)
        {
            var containsSearchTag = SortingSettings
                .SearchTagList
                .SearchTagList
                .Contains(file.Name.Split('.')[0]);


            var random = new Random();
            
            var randomYear = random.Next(1980, 2024);
            var randomMonth = random.Next(1, 12);
            var randomDay = random.Next(1, 30);

            using (FileStream fileStream = file.Create())
            {
                file.LastAccessTime = new DateTime(randomYear, randomMonth, randomDay);
                file.CreationTime = new DateTime(randomYear, randomMonth, randomDay); 
            }
            
            

            var expectedPathIsSortingByLastAccessTime = Path.Combine(targetTestFilePath, $"{randomYear}-{randomMonth:D2}-{randomDay:D2}");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByLastAccessTime,isFalseCheckBox);
            
            var expectedPathIsSortingByCreateTime = Path.Combine(targetTestFilePath, $"{randomYear}-{randomMonth:D2}-{randomDay:D2}");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByCreateTime,isFalseCheckBox);
            
            var expectedPathIsSortingByFileExtension = Path.Combine(targetTestFilePath, $"{file.Extension.TrimStart('.')}");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByFileExtension,isFalseCheckBox);
            
            var expectedPathIsSortingBySearchTag = Path.Combine(targetTestFilePath, $"{file.Name.Split('.')[0]}");
            var alternativePathIsSortingBySearchTag = Path.Combine(targetTestFilePath, "could-not-be-classified");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingBySearchTag,isFalseCheckBox);
            
            var expectedPathIsSortingBySearchTagAndFileExtension = Path.Combine(targetTestFilePath, $"{file.Name.Split('.')[0]}", $"{file.Extension.TrimStart('.')}"); 
            var alternativePathIsSortingBySearchTagAndFileExtension = Path.Combine(targetTestFilePath,"could-not-be-classified",$"{file.Extension.TrimStart('.')}");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingBySearchTag,isSortingByFileExtension);  
            
            var expectedPathIsSortingByFileExtensionAndSearchTag = Path.Combine( targetTestFilePath,$"{file.Extension.TrimStart('.')}", $"{file.Name.Split('.')[0]}");
            var alternativePathIsSortingByFileExtensionAndSearchTag = Path.Combine( targetTestFilePath,$"{file.Extension.TrimStart('.')}", "could-not-be-classified");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByFileExtension,isSortingBySearchTag);                                 
            
            var expectedPathIsSortingByCreateTimeAndSearchTag = Path.Combine(targetTestFilePath, $"{randomYear}-{randomMonth:D2}-{randomDay:D2}", $"{file.Name.Split('.')[0]}");
            var alternativePathIsSortingByCreateTimeAndSearchTag = Path.Combine(targetTestFilePath, $"{randomYear}-{randomMonth:D2}-{randomDay:D2}", "could-not-be-classified");
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByCreateTime,isSortingBySearchTag);                 
            
            var expectedPathIsSortingByCreateTimeAndFileExtension = Path.Combine(targetTestFilePath, $"{randomYear}-{randomMonth:D2}-{randomDay:D2}", $"{file.Extension.TrimStart('.')}"); 
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByCreateTime,isSortingByFileExtension);
            
            var expectedPathIsSortingByFileExtensionAndCreateTime = Path.Combine(targetTestFilePath, $"{file.Extension.TrimStart('.')}",$"{randomYear}-{randomMonth:D2}-{randomDay:D2}");  
            new Sorting(SortingSettings).SortingTask(sourceTestFilePath,isSortingByFileExtension,isSortingByCreateTime);    
            
            //Assert
            Assert.True(Directory.Exists(expectedPathIsSortingByLastAccessTime));
            Assert.True(Directory.Exists(expectedPathIsSortingByCreateTime));
            Assert.True(Directory.Exists(expectedPathIsSortingByFileExtension));

            Assert.True( containsSearchTag
                ? Directory.Exists(expectedPathIsSortingBySearchTag)
                : Directory.Exists(alternativePathIsSortingBySearchTag));
            
            Assert.True( containsSearchTag
                ? Directory.Exists(expectedPathIsSortingBySearchTagAndFileExtension)
                : Directory.Exists(alternativePathIsSortingBySearchTagAndFileExtension));

            Assert.True( containsSearchTag
                ? Directory.Exists(expectedPathIsSortingByFileExtensionAndSearchTag)
                : Directory.Exists(alternativePathIsSortingByFileExtensionAndSearchTag));
            
            Assert.True( containsSearchTag
                ? Directory.Exists(expectedPathIsSortingByCreateTimeAndSearchTag)
                : Directory.Exists(alternativePathIsSortingByCreateTimeAndSearchTag));
            
            Assert.True(Directory.Exists(expectedPathIsSortingByCreateTimeAndFileExtension));   
            Assert.True(Directory.Exists(expectedPathIsSortingByFileExtensionAndCreateTime));
            
            
            Directory.Delete(targetTestFilePath,true);
            
        }
        
        
    }
}