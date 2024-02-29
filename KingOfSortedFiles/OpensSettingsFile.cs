using System.IO;
using Avalonia.Controls;
using KingOfSortedFiles.Views;

namespace KingOfSortedFiles;


//opens a new window that allows you to edit the settings file
public static class OpensSettingsFile
{
    private static SettingWindow? SettingWindow { get; set; }
    private static string SettingsString { get; set; } = null!;

    public static void OpenFile(string filePath)
    {
        
        using (StreamReader streamReader = new StreamReader(filePath))
        {
            SettingsString = streamReader.ReadToEnd();
        }

        SettingWindow = new SettingWindow();
        
        var textBox = SettingWindow.Find<TextBox>("TextBox");
        textBox!.Text = SettingsString;

        textBox.TextChanged += (sender, args) =>
        {
            SettingsString = textBox.Text;
        };
        
        SettingWindow.Show();
        
    }

    //saves the changes
    public static void SaveFile(string filePath)
    {
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            streamWriter.Write(SettingsString);
        }
        SettingWindow!.Close();
       new ProgramStartRoutine();
    }
}