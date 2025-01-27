﻿using AAXClean;
using Dinah.Core;

namespace AaxDecrypter
{
    public class DownloadOptions
    {
        public string DownloadUrl { get; }
        public string UserAgent { get; }
        public string AudibleKey { get; init; }
        public string AudibleIV { get; init; }
        public OutputFormat OutputFormat { get; init; }
        public bool TrimOutputToChapterLength { get; init; }
        public bool RetainEncryptedFile { get; init; }
        public bool StripUnabridged { get; init; }
        public bool CreateCueSheet { get; init; }
        public ChapterInfo ChapterInfo { get; set; }
        public NAudio.Lame.LameConfig LameConfig { get; set; }
        public bool Downsample { get; set; }
        public bool MatchSourceBitrate { get; set; }       

        public DownloadOptions(string downloadUrl, string userAgent)
        {
            DownloadUrl = ArgumentValidator.EnsureNotNullOrEmpty(downloadUrl, nameof(downloadUrl));
            UserAgent = ArgumentValidator.EnsureNotNullOrEmpty(userAgent, nameof(userAgent));

            // no null/empty check for key/iv. unencrypted files do not have them
        }
    }
}
