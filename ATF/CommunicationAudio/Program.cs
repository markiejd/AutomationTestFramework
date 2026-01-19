using TagLib;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;

namespace CommunicationAudio
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    PrintUsage();
                    return;
                }

                string action = args[0].ToLower();

                switch (action)
                {
                    case "metadata":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: metadata action requires a file path");
                            PrintUsage();
                            return;
                        }
                        GetMetadata(args[1]);
                        break;

                    case "compare":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("Error: compare action requires two file paths");
                            PrintUsage();
                            return;
                        }
                        CompareAudioFiles(args[1], args[2]);
                        break;

                    default:
                        Console.WriteLine($"Error: Unknown action '{action}'");
                        PrintUsage();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void GetMetadata(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"Audio file not found: {filePath}");
            }

            try
            {
                var file = TagLib.File.Create(filePath);
                var audioProperties = file.Properties;

                var metadata = new AudioMetadata
                {
                    FilePath = Path.GetFullPath(filePath),
                    FileName = Path.GetFileName(filePath),
                    Duration = audioProperties.Duration.TotalSeconds,
                    DurationFormatted = $"{audioProperties.Duration.Hours:D2}:{audioProperties.Duration.Minutes:D2}:{audioProperties.Duration.Seconds:D2}",
                    BitRate = audioProperties.AudioBitrate,
                    SampleRate = audioProperties.AudioSampleRate,
                    Channels = audioProperties.AudioChannels,
                    Codec = audioProperties.CodecName ?? "Unknown",
                    FileSize = new FileInfo(filePath).Length,
                    FileIntegrity = CalculateFileHash(filePath),
                    MediaType = file.MimeType ?? "Unknown"
                };

                string json = JsonConvert.SerializeObject(metadata, Formatting.Indented);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to read audio metadata from '{filePath}': {ex.Message}");
            }
        }

        static void CompareAudioFiles(string file1Path, string file2Path)
        {
            if (!System.IO.File.Exists(file1Path))
                throw new FileNotFoundException($"Audio file not found: {file1Path}");
            if (!System.IO.File.Exists(file2Path))
                throw new FileNotFoundException($"Audio file not found: {file2Path}");

            var metadata1 = GetMetadataObject(file1Path);
            var metadata2 = GetMetadataObject(file2Path);

            var comparison = new AudioComparison
            {
                File1 = Path.GetFileName(file1Path),
                File2 = Path.GetFileName(file2Path),
                FilesIdentical = false,
                Differences = new List<string>()
            };

            // Compare metadata
            if (metadata1.Duration != metadata2.Duration)
                comparison.Differences.Add($"Duration differs: {metadata1.DurationFormatted} vs {metadata2.DurationFormatted} ({metadata1.Duration}s vs {metadata2.Duration}s)");

            if (metadata1.BitRate != metadata2.BitRate)
                comparison.Differences.Add($"BitRate differs: {metadata1.BitRate} kbps vs {metadata2.BitRate} kbps");

            if (metadata1.SampleRate != metadata2.SampleRate)
                comparison.Differences.Add($"Sample Rate differs: {metadata1.SampleRate} Hz vs {metadata2.SampleRate} Hz");

            if (metadata1.Channels != metadata2.Channels)
                comparison.Differences.Add($"Channels differ: {metadata1.Channels} vs {metadata2.Channels}");

            if (metadata1.Codec != metadata2.Codec)
                comparison.Differences.Add($"Codec differs: {metadata1.Codec} vs {metadata2.Codec}");

            if (metadata1.FileSize != metadata2.FileSize)
                comparison.Differences.Add($"File Size differs: {metadata1.FileSize} bytes vs {metadata2.FileSize} bytes");

            if (metadata1.FileIntegrity != metadata2.FileIntegrity)
                comparison.Differences.Add($"File Integrity (MD5 Hash) differs: {metadata1.FileIntegrity} vs {metadata2.FileIntegrity}");

            // Check if files are identical
            if (comparison.Differences.Count == 0)
            {
                comparison.FilesIdentical = true;
                comparison.Differences.Add("Files are identical - all metadata matches and file hashes match");
            }

            string json = JsonConvert.SerializeObject(comparison, Formatting.Indented);
            Console.WriteLine(json);
        }

        static AudioMetadata GetMetadataObject(string filePath)
        {
            var file = TagLib.File.Create(filePath);
            var audioProperties = file.Properties;

            return new AudioMetadata
            {
                FilePath = Path.GetFullPath(filePath),
                FileName = Path.GetFileName(filePath),
                Duration = audioProperties.Duration.TotalSeconds,
                DurationFormatted = $"{audioProperties.Duration.Hours:D2}:{audioProperties.Duration.Minutes:D2}:{audioProperties.Duration.Seconds:D2}",
                BitRate = audioProperties.AudioBitrate,
                SampleRate = audioProperties.AudioSampleRate,
                Channels = audioProperties.AudioChannels,
                Codec = audioProperties.CodecName ?? "Unknown",
                FileSize = new FileInfo(filePath).Length,
                FileIntegrity = CalculateFileHash(filePath),
                MediaType = file.MimeType ?? "Unknown"
            };
        }

        static string CalculateFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return Convert.ToHexString(hash);
                }
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("CommunicationAudio - Audio File Metadata and Comparison Tool");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run <action> <parameters>");
            Console.WriteLine();
            Console.WriteLine("Actions:");
            Console.WriteLine("  metadata <filePath>           - Extract metadata from an audio file");
            Console.WriteLine("  compare <file1Path> <file2Path> - Compare two audio files");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project ./CommunicationAudio.csproj -- metadata c:\\audio\\song.mp3");
            Console.WriteLine("  dotnet run --project ./CommunicationAudio.csproj -- compare c:\\audio\\song1.mp3 c:\\audio\\song2.mp3");
        }
    }

    public class AudioMetadata
    {
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("durationFormatted")]
        public string DurationFormatted { get; set; }

        [JsonProperty("bitRate")]
        public int BitRate { get; set; }

        [JsonProperty("sampleRate")]
        public int SampleRate { get; set; }

        [JsonProperty("channels")]
        public int Channels { get; set; }

        [JsonProperty("codec")]
        public string Codec { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        [JsonProperty("fileIntegrity")]
        public string FileIntegrity { get; set; }

        [JsonProperty("mediaType")]
        public string MediaType { get; set; }
    }

    public class AudioComparison
    {
        [JsonProperty("file1")]
        public string File1 { get; set; }

        [JsonProperty("file2")]
        public string File2 { get; set; }

        [JsonProperty("filesIdentical")]
        public bool FilesIdentical { get; set; }

        [JsonProperty("differences")]
        public List<string> Differences { get; set; }
    }
}
