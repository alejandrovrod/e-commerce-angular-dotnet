namespace ECommerce.File.Domain.ValueObjects;

public class FileInfo
{
    public string FileName { get; private set; }
    public string Extension { get; private set; }
    public long Size { get; private set; }
    public string ContentType { get; private set; }

    public FileInfo(string fileName, string extension, long size, string contentType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("File extension cannot be empty", nameof(extension));
        
        if (size <= 0)
            throw new ArgumentException("File size must be greater than 0", nameof(size));
        
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty", nameof(contentType));

        FileName = fileName;
        Extension = extension;
        Size = size;
        ContentType = contentType;
    }

    public string FullFileName => $"{FileName}.{Extension}";
    public bool IsImage => ContentType.StartsWith("image/");
    public bool IsDocument => ContentType.StartsWith("application/") || ContentType.StartsWith("text/");
    public bool IsVideo => ContentType.StartsWith("video/");
    public bool IsAudio => ContentType.StartsWith("audio/");
    public bool IsArchive => ContentType.Contains("zip") || ContentType.Contains("rar") || ContentType.Contains("7z");
}

public class FilePath
{
    public string Path { get; private set; }
    public string Directory { get; private set; }
    public string FileName { get; private set; }

    public FilePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        Path = path;
        Directory = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
        FileName = System.IO.Path.GetFileName(path);
    }

    public bool Exists => System.IO.File.Exists(Path);
    public string GetRelativePath(string basePath) => System.IO.Path.GetRelativePath(basePath, Path);
}
