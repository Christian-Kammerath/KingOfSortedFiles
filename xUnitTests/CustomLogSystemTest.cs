using Avalonia.Controls;
using Avalonia.Media;
using KingOfSortedFiles;

namespace TestProject1;

public class CustomLogSystemTest
{
    // Note: CustomLogSystem is static, so its LogSystemSettings state is shared across all tests.
    // Each test re-initializes it via BindListBox + BindLogFile to avoid interference.

    [Fact]
    public void Creates_A_Log_File_If_One_Does_Not_Exist_And_Writes_It_To_The_Log_File()
    {
        //Arrange
        var testListBox = new ListBox();
        var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Test_LogFile.txt");
        if (File.Exists(logFilePath)) File.Delete(logFilePath); // ensure clean state

        var debugCallArray        = new[] { "Debug call 1",        "Debug call 2"        };
        var warningCallArray      = new[] { "Warning call 1",      "Warning call 2"      };
        var errorCallArray        = new[] { "Error call 1",        "Error call 2"        };
        var informationalCallArray = new[] { "Informational call 1", "Informational call 2" };

        CustomLogSystem.BindListBox(testListBox).BindLogFile(logFilePath);

        //Act
        CustomLogSystem.Debug(debugCallArray[0], true);
        CustomLogSystem.Debug(debugCallArray[1], true);

        CustomLogSystem.Warning(warningCallArray[0], true);
        CustomLogSystem.Warning(warningCallArray[1], true);

        CustomLogSystem.Error(errorCallArray[0], true);
        CustomLogSystem.Error(errorCallArray[1], true);

        CustomLogSystem.Informational(informationalCallArray[0], true);
        CustomLogSystem.Informational(informationalCallArray[1], true);

        //Assert: log file was created and contains all messages
        Assert.True(File.Exists(logFilePath));
        var logFileText = File.ReadAllText(logFilePath);

        foreach (var message in debugCallArray.Concat(warningCallArray).Concat(errorCallArray).Concat(informationalCallArray))
            Assert.Contains(message, logFileText);

        // Assert: Debug tabs — green background
        var debugTab0 = testListBox.Items[0] as DebugLogTab;
        var debugTab1 = testListBox.Items[1] as DebugLogTab;
        Assert.NotNull(debugTab0);
        Assert.NotNull(debugTab1);
        Assert.Contains(debugCallArray[0], debugTab0!.Text);
        Assert.Contains(debugCallArray[1], debugTab1!.Text);
        Assert.Equal(Brushes.Green, debugTab0!.Background);
        Assert.Equal(Brushes.Green, debugTab1!.Background);

        // Assert: Warning tabs — yellow background
        var warningTab0 = testListBox.Items[2] as WarningLogTab;
        var warningTab1 = testListBox.Items[3] as WarningLogTab;
        Assert.NotNull(warningTab0);
        Assert.NotNull(warningTab1);
        Assert.Contains(warningCallArray[0], warningTab0!.Text);
        Assert.Contains(warningCallArray[1], warningTab1!.Text);
        Assert.Equal(Brushes.Yellow, warningTab0!.Background);
        Assert.Equal(Brushes.Yellow, warningTab1!.Background);

        // Assert: Error tabs — red background
        var errorTab0 = testListBox.Items[4] as ErrorLogTab;
        var errorTab1 = testListBox.Items[5] as ErrorLogTab;
        Assert.NotNull(errorTab0);
        Assert.NotNull(errorTab1);
        Assert.Contains(errorCallArray[0], errorTab0!.Text);
        Assert.Contains(errorCallArray[1], errorTab1!.Text);
        Assert.Equal(Brushes.Red, errorTab0!.Background);
        Assert.Equal(Brushes.Red, errorTab1!.Background);

        // Assert: Informational tabs — no background (distinguishable from other levels)
        var infoTab0 = testListBox.Items[6] as InformationalLogTab;
        var infoTab1 = testListBox.Items[7] as InformationalLogTab;
        Assert.NotNull(infoTab0);
        Assert.NotNull(infoTab1);
        Assert.Contains(informationalCallArray[0], infoTab0!.Text);
        Assert.Contains(informationalCallArray[1], infoTab1!.Text);
        Assert.Null(infoTab0!.Background);
        Assert.Null(infoTab1!.Background);

        File.Delete(logFilePath);
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void Log_File_Appends_New_Entries_Without_Overwriting_Existing_Content()
    {
        //Arrange
        var testListBox = new ListBox();
        var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Test_AppendLogFile.txt");
        if (File.Exists(logFilePath)) File.Delete(logFilePath);

        CustomLogSystem.BindListBox(testListBox).BindLogFile(logFilePath);

        //Act — write two separate batches
        CustomLogSystem.Debug("First batch entry", false);
        CustomLogSystem.Warning("Second batch entry", false);

        // Simulate another component re-binding and writing more entries
        CustomLogSystem.BindListBox(testListBox).BindLogFile(logFilePath);
        CustomLogSystem.Error("Third batch entry", false);

        //Assert — all three entries must be present (AppendText, not overwrite)
        var content = File.ReadAllText(logFilePath);
        Assert.Contains("First batch entry",  content);
        Assert.Contains("Second batch entry", content);
        Assert.Contains("Third batch entry",  content);

        File.Delete(logFilePath);
    }

    // -------------------------------------------------------------------------

    [Fact]
    public void Each_Log_Level_Uses_The_Correct_Log_Level_Label_In_The_File()
    {
        //Arrange
        var testListBox = new ListBox();
        var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Test_LogLevelFile.txt");
        if (File.Exists(logFilePath)) File.Delete(logFilePath);

        CustomLogSystem.BindListBox(testListBox).BindLogFile(logFilePath);

        //Act
        CustomLogSystem.Debug("msg", false);
        CustomLogSystem.Warning("msg", false);
        CustomLogSystem.Error("msg", false);
        CustomLogSystem.Informational("msg", false);

        //Assert — log file format must contain the correct level labels
        var content = File.ReadAllText(logFilePath);
        Assert.Contains("[Debug]",         content);
        Assert.Contains("[Warning]",       content);
        Assert.Contains("[Error]",         content);
        Assert.Contains("[Informational]", content);

        File.Delete(logFilePath);
    }
}
