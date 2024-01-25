using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Media;
using KingOfSortedFiles;

namespace TestProject1;

public class CustomLogSystemTest
{
    [Fact]
    public void Creates_A_Log_File_If_One_Does_Not_Exist_And_Writes_It_To_The_Log_File()
    {
        //Arrange

        var testListBox = new ListBox();
        var logFileName = "Test_LogFile.txt";
        var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), logFileName);
        
        var debugCallArray = new[] { "Debug call 1","Debug call 2"};
        var warningCallArray = new[] { "Warning call 1","Warning call 2"};
        var errorCallArray = new[] {"Error call 1", "Error call 2"};
        var informationalCallArray = new[] { "Informational call 1", "Informational call 2" };

        var combinedArrays = debugCallArray
            .Concat(warningCallArray)
            .Concat(errorCallArray)
            .Concat(informationalCallArray);

        CustomLogSystem
            .BindListBox(testListBox)
            .BindLogFile(logFileName);
        
        //Act
        CustomLogSystem.Debug(debugCallArray[0]);
        CustomLogSystem.Debug(debugCallArray[1]);
        
        CustomLogSystem.Warning(warningCallArray[0]);
        CustomLogSystem.Warning(warningCallArray[1]);
        
        CustomLogSystem.Error(errorCallArray[0]);
        CustomLogSystem.Error(errorCallArray[1]);

        CustomLogSystem.Informational(informationalCallArray[0]);
        CustomLogSystem.Informational(informationalCallArray[1]);
        
        //Assert
        var currentDirectoryFiles = Directory.GetFiles(Directory.GetCurrentDirectory()).ToList();

        Assert.Contains( logFilePath,currentDirectoryFiles);

        using StreamReader streamReader = new StreamReader(logFilePath);
        var logFileText = streamReader.ReadToEnd();

        foreach (var sentence in combinedArrays)
        {
            Assert.Contains(sentence, logFileText);
        }

        
        // Testing the ListBox entries (debug)
        DebugLogTab? debugLogTab0 = testListBox.Items[0] as DebugLogTab;
        DebugLogTab? debugLogTab1 = testListBox.Items[1] as DebugLogTab;
        
        Assert.Contains(debugCallArray[0],debugLogTab0!.Text);
        Assert.True(Equals(debugLogTab0!.Background, Brushes.Green));
        
        Assert.Contains(debugCallArray[1],debugLogTab1!.Text);
        Assert.True(Equals(debugLogTab1!.Background, Brushes.Green));

        
        
        //Test of the ListBox entries (Warning)
        WarningLogTab?  warningLogTab0 = testListBox.Items[2] as WarningLogTab;
        WarningLogTab?  warningLogTab1 = testListBox.Items[3] as WarningLogTab;
        
        Assert.Contains(warningCallArray[0],warningLogTab0!.Text);
        Assert.True(Equals(warningLogTab0!.Background, Brushes.Yellow));
        
        Assert.Contains(warningCallArray[1],warningLogTab1!.Text);
        Assert.True(Equals(warningLogTab1!.Background, Brushes.Yellow));

        
        //Test of the ListBox entries (Error)
        ErrorLogTab? errorLogTab0 = testListBox.Items[4] as ErrorLogTab;
        ErrorLogTab? errorLogTab1 = testListBox.Items[5] as ErrorLogTab;
        
        Assert.Contains(errorCallArray[0],errorLogTab0!.Text);
        Assert.True(Equals(errorLogTab0!.Background, Brushes.Red));
        
        Assert.Contains(errorCallArray[1],errorLogTab1!.Text);
        Assert.True(Equals(errorLogTab1!.Background, Brushes.Red));

        
        
        //Test of the ListBox entries (Informational)
        InformationalLogTab? informationalLogTab0 = testListBox.Items[6] as InformationalLogTab;
        InformationalLogTab? informationalLogTab1 = testListBox.Items[7] as InformationalLogTab;
        
        Assert.Contains(informationalCallArray[0],informationalLogTab0!.Text);
        Assert.Contains(informationalCallArray[1],informationalLogTab1!.Text);
        
        
        File.Delete(logFilePath);
        
    }
}