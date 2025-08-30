namespace ECommerce.File.Domain.Enums;

public enum FileType
{
    Image = 1,
    Document = 2,
    Video = 3,
    Audio = 4,
    Archive = 5,
    Other = 6
}

public enum FileStatus
{
    Uploaded = 1,
    Processing = 2,
    Processed = 3,
    Failed = 4,
    Deleted = 5,
    Expired = 6
}

public enum FileAccessLevel
{
    Private = 1,
    Public = 2,
    Shared = 3,
    Restricted = 4
}
