﻿namespace WAUZ.BL
{
    public interface IBusinessLogic
    {
        string SourceFolder { get; set; }
        string DestFolder { get; set; }

        void LoadSettings();
        void SaveSettings();
        void ValidateSettings();
        IEnumerable<string> GetSourceFolderZipFiles();
        Task Unzip(IProgress<ProgressData>? progress = default, CancellationToken cancellationToken = default);
    }
}
