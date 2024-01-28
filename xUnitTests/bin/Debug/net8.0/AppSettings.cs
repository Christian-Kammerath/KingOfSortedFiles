namespace KingOfSortedFiles;

public class AppSettings
{
    public FileExtensions FileExtensions { get; set; } = null!;
    public string StartPath { get; set; } = null!;
}

public class FileExtensions
{
    public string[] TextFiles { get; set; } = null!;
    public string[] AudioImageAndVideoFiles { get; set; } = null!;
    public string[] CompressedFiles { get; set; } = null!;
    public string[] SystemFiles { get; set; } = null!;
    public string[] OtherFileExtensions { get; set; } = null!;
}