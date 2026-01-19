
using Core;
using Core.FileIO;
using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Newtonsoft.Json;
using Reqnroll;

namespace Generic.Elements.Steps.Audio
{
    [Binding]
    public class GivenAudioSteps : StepsBase
    {
        public GivenAudioSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        private AudioFileInfo _audioFileInfo = new AudioFileInfo();
        
        private class AudioFileInfo
        {
            public string FilePath { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
            public double Duration { get; set; } = 0.0;
            public string DurationFormatted { get; set; } = string.Empty;
            public int BitRate { get; set; } = 0;
            public int SampleRate { get; set; } = 0;
            public int Channels { get; set; } = 0;
            public string Codec { get; set; } = string.Empty;
            public long FileSize { get; set; } = 0;
            public string FileIntegrity { get; set; } = string.Empty;
            public string MediaType { get; set; } = string.Empty;
        }


        [When("I Query Audio File {string}")]
        public bool WhenIQueryAudioFile(string audioFileAndPath)
        {
            string proc = $"When I Query Audio File \"{audioFileAndPath}\"";
                
            if (CombinedSteps.OutputProc(proc))
            {
                if (!AudioFileExists(audioFileAndPath))
                {
                    CombinedSteps.Failure(proc);
                    return false;   
                }
                DebugOutput.OutputMethod("WhenIQueryAudioFile", $"Audio file exists: {audioFileAndPath}");
                var stringReturn = CallSingleFile(audioFileAndPath);
                DebugOutput.OutputMethod("WhenIQueryAudioFile", $"Audio file query returned: {stringReturn}");
                if (stringReturn.StartsWith("error"))
                {
                    CombinedSteps.Failure(proc);
                    return false;
                }
                // Set Class
                if (!ConvertResponseToAudioClass(stringReturn))
                {
                    CombinedSteps.Failure(proc);
                    return false;
                }
                DebugOutput.Log("WhenIQueryAudioFile stored successfully");
                return true;
            }
            CombinedSteps.Failure(proc);
            return false;
        }

        [Then("Audio File Is Valid")]
        public bool ThenAudioFileIsValid()
        {
            string proc = "Then Audio File Is Valid";
                
            if (CombinedSteps.OutputProc(proc))
            {
                if (_audioFileInfo.FilePath == string.Empty)
                {
                    DebugOutput.Log("ThenAudioFileIsValid found empty file path! Make sure to run When I Query Audio File to set the class values first!");
                    CombinedSteps.Failure(proc);
                    return false;   
                }
                else
                {
                    DebugOutput.Log($"ThenAudioFileIsValid found file path: {_audioFileInfo.FilePath} we assume everything else is fine!");
                    return true;
                }
            }
            CombinedSteps.Failure(proc);
            return false;
        }

        [Then("Audio File Metadata {string} Is Equal To {string}")]
        public bool ThenAudioFileMetadataIsEqualTo(string variable, string value)
        {
            string proc = $"Then Audio File Metadata \"{variable}\" Is Equal To \"{value}\"";
                
            if (CombinedSteps.OutputProc(proc))
            {
                switch (variable.ToLower())
                {
                    case "filepath":
                        if (_audioFileInfo.FilePath == value)
                        {
                            DebugOutput.Log($"ThenAudioFileMetadataIsEqualTo FilePath matches: {value}");
                            return true;
                        }
                        break;
                    case "filename":
                        if (_audioFileInfo.FileName == value)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo FileName matches: {value}");
                            return true;
                        }
                        break;
                    // Add more cases as needed for other metadata fields
                    case "duration":
                        if (double.TryParse(value, out double durationValue) && _audioFileInfo.Duration == durationValue)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo Duration matches: {value}");
                            return true;
                        }
                        break;
                    case "bitrate":
                        if (int.TryParse(value, out int bitRateValue) && _audioFileInfo.BitRate == bitRateValue)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo BitRate matches: {value}");
                            return true;
                        }
                        break;
                    case "samplerate":
                        if (int.TryParse(value, out int sampleRateValue) && _audioFileInfo.SampleRate == sampleRateValue)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo SampleRate matches: {value}");
                            return true;
                        }
                        break;
                    case "channels":
                        if (int.TryParse(value, out int channelsValue) && _audioFileInfo.Channels == channelsValue)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo Channels matches: {value}");
                            return true;
                        }   
                        break;
                    case "codec":
                        if (_audioFileInfo.Codec == value)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo Codec matches: {value}");
                            return true;
                        }
                        break;
                    case "filesize":
                        if (long.TryParse(value, out long fileSizeValue) && _audioFileInfo.FileSize == fileSizeValue)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo FileSize matches: {value}");
                            return true;
                        }
                        break;
                    case "fileintegrity":
                        if (_audioFileInfo.FileIntegrity == value)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo FileIntegrity matches: {value}");
                            return true;
                        }
                        break;  
                    case "mediatype":
                        if (_audioFileInfo.MediaType == value)
                        {
                            DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo MediaType matches: {value}");
                            return true;
                        }
                        break;
                    default:
                        DebugOutput.Log($"ThenAudopFileMetadataIsEqualTo unknown variable: {variable}");
                        CombinedSteps.Failure(proc);
                        return false;
                }
                return false;
            // make me true!     //return true;
            }
            CombinedSteps.Failure(proc);
            return false;
        }



        private bool AudioFileExists(string filePath)
        {
            DebugOutput.OutputMethod("AudioFileExists", $"Checking existence of file: {filePath}");
            return FileUtils.OSFileCheck(filePath);
        }

        public string CallSingleFile(string filePath)
        {
            DebugOutput.OutputMethod("CallSingleFile", $"Calling audio program for file: {filePath}");
            try
            {
                var response = CmdUtil.ExecuteDotnet("./CommunicationAudio/CommunicationAudio.csproj", $"metadata \"{filePath}\"").Trim();
                DebugOutput.Log($"{response}");
                return response;
            }
            catch (Exception ex)
            {
                DebugOutput.Log($"CallSingleFile Exception: {ex.Message}");
                return "error";
            }
        }

        [When(@"I Compare Audio Files ""([^""]*)"" And ""([^""]*)"" They Are Identical")]
        public bool WhenICompareAudioFilesAndTheyAreIdentical(string a, string b)
        {
            string proc = $"When I Compare Audio Files \"{a}\" And \"{b}\" They Are Identical";
                
            if (CombinedSteps.OutputProc(proc))
            {
                // Check both files exist first
                if (!AudioFileExists(a))
                {
                    DebugOutput.Log($"WhenICompareAudioFilesAndTheyAreIdentical File A does not exist: {a}");
                    CombinedSteps.Failure(proc);
                    return false;   
                }
                if (!AudioFileExists(b))
                {
                    DebugOutput.Log($"WhenICompareAudioFilesAndTheyAreIdentical File B does not exist: {b}");
                    CombinedSteps.Failure(proc);
                    return false;   
                }
                var stringReturn = CmdUtil.ExecuteDotnet("./CommunicationAudio/CommunicationAudio.csproj", $"compare \"{a}\" \"{b}\"").Trim();
                DebugOutput.OutputMethod("WhenICompareAudioFilesAndTheyAreIdentical", $"Audio file comparison returned: {stringReturn}");
                if (!stringReturn.Contains("Files are identical"))
                {
                    CombinedSteps.Failure(proc);
                    return false;
                }
                DebugOutput.Log($"WhenICompareAudioFilesAndTheyAreIdentical comparison successful");
                return true;
            }
            CombinedSteps.Failure(proc);
            return false;
        }


        [When(@"I Compare Audio Files ""([^""]*)"" And ""([^""]*)"" They Are Not The Same")]
        public bool WhenICompareAudioFilesAndTheyAreNotTheSame(string a, string b)
        {
            string proc = $"When I Compare Audio Files \"{a}\" And \"{b}\" They Are Not The Same";
                
            if (CombinedSteps.OutputProc(proc))
            {
                // Check both files exist first
                if (!AudioFileExists(a))
                {
                    DebugOutput.Log($"WhenICompareAudioFilesAndTheyAreNotTheSame File A does not exist: {a}");
                    CombinedSteps.Failure(proc);
                    return false;   
                }
                if (!AudioFileExists(b))
                {
                    DebugOutput.Log($"WhenICompareAudioFilesAndTheyAreNotTheSame File B does not exist: {b}");
                    CombinedSteps.Failure(proc);
                    return false;   
                }
                var stringReturn = CmdUtil.ExecuteDotnet("./CommunicationAudio/CommunicationAudio.csproj", $"compare \"{a}\" \"{b}\"").Trim();
                DebugOutput.OutputMethod("WhenICompareAudioFilesAndTheyAreNotTheSame", $"Audio file comparison returned: {stringReturn}");
                if (!stringReturn.Contains("Files are identical"))
                {
                    DebugOutput.Log("When I Compare I do not get the message saying they are Identical, so they must NOT be the same! AND in this case that is what we want.");
                    return true;
                }
                DebugOutput.Log($"WhenICompareAudioFilesAndTheyAreNotTheSame comparison successful  - files are the same, which is not expected.");
                CombinedSteps.Failure(proc);
                return false;
            }
            CombinedSteps.Failure(proc);
            return false;
        }

        

        private bool ConvertResponseToAudioClass(string response)
        {
            DebugOutput.OutputMethod("ConvertResponseToAudioClass", $"Converting response to AudioFileInfo class");
            try
            {                
                var audioInfo = JsonConvert.DeserializeObject<AudioFileInfo>(response);
                if (audioInfo == null)
                {
                    DebugOutput.Log("ConvertResponseToAudioClass Deserialization returned null");
                    return false;
                }
                DebugOutput.Log("ConvertResponseToAudioClass Deserialization successful AND stored in audioInfo");
                try
                {
                    _audioFileInfo = audioInfo;
                    return true;
                }
                catch (Exception ex)
                {
                    DebugOutput.Log($"ConvertResponseToAudioClass Assignment Exception: {ex.Message}");
                    return false;
                }
            }
            catch (JsonException jsonEx)
            {
                DebugOutput.Log($"ConvertResponseToAudioClass JSON Exception: {jsonEx.Message}");
                return false;
            }
        }   


    }
}