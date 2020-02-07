﻿using ASPNetScape.Abstractions.Cache;
using ASPNetScape.Modules.SevenOneEight.Cache.Cache.FileTypes;
using ASPNetScape.Abstractions.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASPNetScape.Modules.SevenOneEight.Cache.Cache.FlatFile
{
    public class FlatFileCache : CacheBase
    {
        private ILogger _logger;

        public FlatFileCache(ILogger logger)
        {
            _logger = logger;
        }

        private static readonly Regex FileNameRegex = new Regex(@"[/\\](\d+)(\..+)?$");

        private string _baseDirectory;

        /// <summary>
        /// The base directory where all files will be stored in/retrieved from.
        /// </summary>
        public string BaseDirectory
        {
            get { return this._baseDirectory; }
            set { this._baseDirectory = PathExtensions.FixDirectory(value); }
        }

        public override IEnumerable<CacheIndex> GetIndexes()
        {
            if (!Directory.Exists(this.BaseDirectory))
            {
                return Enumerable.Empty<CacheIndex>();
            }

            return Directory.EnumerateDirectories(this.BaseDirectory)
                .Select(Path.GetFileName)
                .Select(indexIdString =>
                {
                    int value;
                    return int.TryParse(indexIdString, out value) ? value : -1;
                })
                .Where(indexId => indexId != -1)
                .Cast<CacheIndex>();
        }

        public override IEnumerable<int> GetFileIds(CacheIndex index)
        {
            var indexDirectory = this.GetIndexDirectory(index);

            if (!Directory.Exists(indexDirectory))
            {
                return Enumerable.Empty<int>();
            }

            return Directory.EnumerateFileSystemEntries(this.GetIndexDirectory(index))
                .Where(fileSystemEntry =>
                    FlatFileCache.FileNameRegex.IsMatch(fileSystemEntry))
                .Select(fileSystemEntry =>
                    int.Parse(FlatFileCache.FileNameRegex.Match(fileSystemEntry).Groups[1].Value));
        }

        public override CacheFileInfo GetFileInfo(CacheIndex index, int fileId)
        {
            var info = new CacheFileInfo
            {
                Index = index,
                FileId = fileId
            };

            var filePath = this.GetExistingFilePaths(index, fileId).FirstOrDefault();
            if (filePath != null)
            {
                var filePathInfo = new FileInfo(filePath);

                info.CompressionType = CompressionType.None;
                info.UncompressedSize = (int)filePathInfo.Length;
                info.EntryInfo.Add(0, new CacheFileEntryInfo
                {
                    EntryId = 0
                });

                return info;
            }

            var entryPaths = this.GetExistingEntryPaths(index, fileId);
            if (entryPaths.Any())
            {
                foreach (var entryId in entryPaths.Keys)
                {
                    info.EntryInfo.Add(entryId, new CacheFileEntryInfo
                    {
                        EntryId = entryId
                    });
                }

                return info;
            }

            throw new FileNotFoundException("Requested file does not exist.");
        }

        protected override void PutFileInfo(CacheFileInfo fileInfo)
        {
            // Nothing interesting happens.
        }

        public FlatFileCache(string baseDirectory)
        {
            this.BaseDirectory = baseDirectory;
        }

        protected override BinaryFile GetBinaryFile(CacheFileInfo fileInfo)
        {
            // Single file
            if (!fileInfo.UsesEntries)
            {
                return new BinaryFile
                {
                    Info = fileInfo,
                    Data = File.ReadAllBytes(this.GetExistingFilePaths(fileInfo.Index, fileInfo.FileId.Value).First())
                };
            }

            // Entries
            var entryFile = new EntryFile
            {
                Info = fileInfo
            };

            foreach (var existingEntryPath in this.GetExistingEntryPaths(fileInfo.Index, fileInfo.FileId.Value))
            {
                entryFile.AddEntry(existingEntryPath.Key, File.ReadAllBytes(existingEntryPath.Value));
            }

            // TODO: Return EntryFile directly so it doesn't have to be needlessly encoded
            return entryFile.ToBinaryFile();
        }

        protected override void PutBinaryFile(BinaryFile file)
        {
            // Throw an exception if the output directory is not yet set or does not exist
            if (string.IsNullOrWhiteSpace(this.BaseDirectory))
            {
                throw new InvalidOperationException("Base directory must be set before writing files.");
            }

            var indexDirectory = this.GetIndexDirectory(file.Info.Index);

            // Create index directory for when it does not exist yet
            Directory.CreateDirectory(indexDirectory);

            // Clean existing files/entries
            foreach (var existingFilePath in this.GetExistingFilePaths(file.Info.Index, file.Info.FileId.Value))
            {
                File.Delete(existingFilePath);
            }

            var entryDirectory = this.GetEntryDirectory(file.Info.Index, file.Info.FileId.Value);

            if (Directory.Exists(entryDirectory))
            {
                Directory.Delete(entryDirectory, true);
            }

            if (!file.Info.UsesEntries)
            {
                // Extract file
                if (file.Data.Length > 0)
                {
                    var extension = ExtensionGuesser.GuessExtension(file.Data);
                    extension = extension != null ? $".{extension}" : "";

                    var filePath = $"{indexDirectory}{file.Info.FileId}{extension}";
                    File.WriteAllBytes(filePath, file.Data);

                    _logger.Information($"Wrote {(int)file.Info.Index}/{file.Info.FileId}.");
                }
                else
                {
                    _logger.Information($"Did not write {(int)file.Info.Index}/{file.Info.FileId} because it is empty.");
                }
            }
            else
            {
                // Extract entries
                var entryFile = new EntryFile();
                entryFile.FromBinaryFile(file);

                if (!entryFile.Empty)
                {
                    Directory.CreateDirectory(entryDirectory);

                    var entryBinaryFiles = entryFile.GetEntries<BinaryFile>().ToList();
                    foreach (var entryBinaryFile in entryBinaryFiles)
                    {
                        var extension = ExtensionGuesser.GuessExtension(entryBinaryFile.Data);
                        extension = extension != null ? $".{extension}" : "";

                        var entryPath = $"{entryDirectory}{entryBinaryFile.Info.EntryId}{extension}";
                        File.WriteAllBytes(entryPath, entryBinaryFile.Data);
                    }

                    _logger.Information($"Wrote {(int)file.Info.Index}/{file.Info.FileId} ({entryBinaryFiles.Count} entries).");
                }
                else
                {
                    _logger.Information($"Did not write {(int)file.Info.Index}/{file.Info.FileId} because it contains no entries.");
                }
            }
        }

        private IEnumerable<string> GetExistingFilePaths(CacheIndex index, int fileId)
        {
            return Directory.EnumerateFiles(this.GetIndexDirectory(index), $"{fileId}*")
                // Filter out false-positivies like 2345 when looking for 234.ext
                .Where(matchedFilePath => Regex.IsMatch(matchedFilePath, $@"[/\\]{fileId}(\..+)?$"));
        }

        private SortedDictionary<int, string> GetExistingEntryPaths(CacheIndex index, int fileId)
        {
            try
            {
                var unsortedDictionary = Directory.EnumerateFiles(this.GetEntryDirectory(index, fileId))
                    .Where(matchedFilePath => FlatFileCache.FileNameRegex.IsMatch(matchedFilePath))
                    .ToDictionary(matchedFilePath =>
                    {
                        var match = FlatFileCache.FileNameRegex.Match(matchedFilePath);
                        return int.Parse(match.Groups[1].Value);
                    }, matchedFilePath => matchedFilePath);

                return new SortedDictionary<int, string>(unsortedDictionary);
            }
            catch (DirectoryNotFoundException exception)
            {
                throw new FileNotFoundException($"Directory for entry {(int)index}/{fileId} does not exist.", exception);
            }
        }

        private string GetIndexDirectory(CacheIndex index)
        {
            return $"{this.BaseDirectory}{(int)index}/";
        }

        private string GetEntryDirectory(CacheIndex index, int fileId)
        {
            return this.GetIndexDirectory(index) + fileId + "/";
        }
    }
}
