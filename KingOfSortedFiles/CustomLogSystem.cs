using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media;

namespace KingOfSortedFiles;

//A class to write a logfile, as well as the possibility to output logs in the Ui
public class LogSystemSettings
{
    private  ListBox? _logListBox  = null!;
    private  FileInfo _logFile  = null!;

    //sets the listbox as LogListbox as well as the logfile
    public ListBox? LogListBox
    {
        get
        {
            if (_logListBox == null)
            {
                throw new InvalidOperationException("LogListBox was not initialized.");
            }

            return _logListBox;
        }
        set => _logListBox = value;
    }
    
    public FileInfo LogFile
    {
        get
        {
            if (_logFile == null)
            {
                throw new InvalidOperationException("LogFile was not initialized.");
            }
            return _logFile;
        }
        set => _logFile = value;
    }
}

//A TexBlock for error logs with red background
public class ErrorLogTab : TextBlock
{
    public ErrorLogTab(string message)
    {
        Text = message;
        Background = Brushes.Red;
        TextWrapping = TextWrapping.Wrap;
    }
}

//A TexBlock for warning logs with a yellow background
public class WarningLogTab: TextBlock
{
    public WarningLogTab(string message)
    {
        Text = message;
        Background = Brushes.Yellow;
        TextWrapping = TextWrapping.WrapWithOverflow;
    }
}

//A TexBlock for debug logs with a green background
public class DebugLogTab : TextBlock
{
    public DebugLogTab(string message)
    {
        Text = message;
        Background = Brushes.Green;
        TextWrapping = TextWrapping.WrapWithOverflow;
    }
}

//A TexBlock for information logs without background
public class InformationalLogTab : TextBlock
{
    public InformationalLogTab(string message)
    {
        Text = message;
        TextWrapping = TextWrapping.WrapWithOverflow;
    }
}

//class to write to the logfile as well as the optional output to the log ListBox
public static class CustomLogSystem
{
    private static readonly LogSystemSettings LogSystemSettings = new();


    public static LogSystemSettings BindListBox(ListBox? logListBox)
    {
        LogSystemSettings.LogListBox = logListBox;
        return LogSystemSettings;
    }

    public static void BindLogFile(this LogSystemSettings logSystemSettings,string logFilePath)
    {
        logSystemSettings.LogFile = new FileInfo(logFilePath);
    }

    private static void WriteToLogFile(string message,string logLevel)
    {
        using (StreamWriter writer = File.AppendText(LogSystemSettings.LogFile.FullName))
        {
            writer.WriteLine("{ " + $"[{logLevel}] [{DateTime.Now}]: {message}" + "},");
        }
    }
    
    public static void Error(string message,bool outputToListBox)
    {
        if(outputToListBox)
            LogSystemSettings
                .LogListBox!
                .Items
                .Add(new ErrorLogTab(message));
            
        WriteToLogFile(message,"Error");
        
    }

    public static void Warning(string message,bool outputToListBox)
    {
        if(outputToListBox)
            LogSystemSettings
                .LogListBox!
                .Items
                .Add(new WarningLogTab(message));
        
        WriteToLogFile(message,"Warning");
    }

    public static void Debug(string message,bool outputToListBox)
    {
        if(outputToListBox)
            LogSystemSettings
                .LogListBox!
                .Items
                .Add(new DebugLogTab(message));
        
        WriteToLogFile(message,"Debug");
    }

    public static void Informational(string message,bool outputToListBox)
    {
        if(outputToListBox)
            LogSystemSettings
                .LogListBox!
                .Items
                .Add(new InformationalLogTab(message));
        
        WriteToLogFile(message,"Informational");
    }
}