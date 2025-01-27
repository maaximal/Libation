﻿using System;
using System.IO;
using AAXClean;
using AAXClean.Codecs;
using Dinah.Core.StepRunner;
using FileManager;

namespace AaxDecrypter
{
	public class AaxcDownloadSingleConverter : AaxcDownloadConvertBase
	{
		protected override StepSequence Steps { get; }

        public AaxcDownloadSingleConverter(string outFileName, string cacheDirectory, DownloadOptions dlLic)
			: base(outFileName, cacheDirectory, dlLic)
        {
            Steps = new StepSequence
            {
                Name = "Download and Convert Aaxc To " + DownloadOptions.OutputFormat,

                ["Step 1: Get Aaxc Metadata"] = Step_GetMetadata,
                ["Step 2: Download Decrypted Audiobook"] = Step_DownloadAudiobookAsSingleFile,
                ["Step 3: Create Cue"] = Step_CreateCue,
                ["Step 4: Cleanup"] = Step_Cleanup,
            };
        }

        private bool Step_DownloadAudiobookAsSingleFile()
        {
            var zeroProgress = Step_DownloadAudiobook_Start();

            FileUtility.SaferDelete(OutputFileName);

            var outputFile = File.Open(OutputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            OnFileCreated(OutputFileName);

            AaxFile.ConversionProgressUpdate += AaxFile_ConversionProgressUpdate;
            var decryptionResult
                = DownloadOptions.OutputFormat == OutputFormat.M4b
                ? AaxFile.ConvertToMp4a(outputFile, DownloadOptions.ChapterInfo, DownloadOptions.TrimOutputToChapterLength)
                : AaxFile.ConvertToMp3(outputFile, DownloadOptions.LameConfig, DownloadOptions.ChapterInfo, DownloadOptions.TrimOutputToChapterLength);
            AaxFile.ConversionProgressUpdate -= AaxFile_ConversionProgressUpdate;

            DownloadOptions.ChapterInfo = AaxFile.Chapters;

            Step_DownloadAudiobook_End(zeroProgress);

            var success = decryptionResult == ConversionResult.NoErrorsDetected && !IsCanceled;
            if (success)
                base.OnFileCreated(OutputFileName);

            return success;
        }
    }
}
