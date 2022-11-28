﻿namespace WAUZ.BL
{
    public interface IBusinessLogic
    {
        string SourceFolder { get; set; }
        string DestFolder { get; set; }

        void LoadSettings();
        void SaveSettings();

        IEnumerable<string> GetZipFiles();
        Task UnzipAsync(IProgress<ProgressData>? progress = default, CancellationToken cancellationToken = default);
    }
}
