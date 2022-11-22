﻿namespace WAUZ.BL
{
    public interface IPathHelper
    {
        bool IsValidAbsolutePath(string path);
        bool IsValidAbsolutePathToExistingFile(string path);
        bool IsValidAbsolutePathToExistingDirectory(string path);
        bool IsValidAbsolutePathToExistingFileOrDirectory(string path);

        string TrimEndingDirectorySeparatorIfExistingFromValidAbsolutePath(string path);
        string GetFileOrDirectoryNameFromValidAbsolutePath(string path);
    }
}
