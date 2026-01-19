# CommunicationAudio

## Overview

`CommunicationAudio` is a standalone .NET application designed to extract metadata from audio files and compare two audio files. It outputs results in JSON format for easy integration with test automation frameworks.

## Features

### Metadata Extraction
Extracts the following audio file metadata:
- **Duration** - Total length in seconds and formatted HH:MM:SS
- **Bitrate** - Audio bitrate in kbps
- **Sample Rate** - Sample rate in Hz
- **Channels** - Number of audio channels (mono, stereo, etc.)
- **Codec** - Audio codec name
- **File Size** - Total file size in bytes
- **File Integrity** - MD5 hash for integrity verification
- **Media Type** - MIME type of the audio file

### Audio File Comparison
Compares two audio files and reports differences in:
- Duration
- Bitrate
- Sample Rate
- Channels
- Codec
- File Size
- File Integrity (MD5 hash)

## Prerequisites

- .NET 6.0 SDK or higher
- Supported audio formats: MP3, FLAC, OGG, WAV, M4A, and most common audio formats

## Building

```bash
dotnet build
```

Required NuGet packages will be automatically restored:
- `TagLibSharp` - Audio metadata extraction
- `NAudio` - Audio file handling
- `Newtonsoft.Json` - JSON serialization

## Usage

### Extract Metadata from Audio File

```bash
dotnet run --project ./CommunicationAudio.csproj -- metadata <filePath>
```

**Parameters:**
- `filePath` - Path to the audio file (supports relative and absolute paths)

**Example:**
```bash
dotnet run --project ./CommunicationAudio.csproj -- metadata "c:\audio\song.mp3"
dotnet run --project ./CommunicationAudio.csproj -- metadata "./TestAudio/sample.wav"
```

**Output (JSON):**
```json
{
  "filePath": "C:\\audio\\song.mp3",
  "fileName": "song.mp3",
  "duration": 245.5,
  "durationFormatted": "00:04:05",
  "bitRate": 320,
  "sampleRate": 44100,
  "channels": 2,
  "codec": "MPEG Version 1 Audio, Layer III",
  "fileSize": 9830400,
  "fileIntegrity": "7A8B9C0D1E2F3A4B5C6D7E8F9A0B1C2D",
  "mediaType": "audio/mpeg"
}
```

### Compare Two Audio Files

```bash
dotnet run --project ./CommunicationAudio.csproj -- compare <file1Path> <file2Path>
```

**Parameters:**
- `file1Path` - Path to the first audio file
- `file2Path` - Path to the second audio file

**Example:**
```bash
dotnet run --project ./CommunicationAudio.csproj -- compare "c:\audio\original.mp3" "c:\audio\converted.mp3"
dotnet run --project ./CommunicationAudio.csproj -- compare "./audio/song_v1.wav" "./audio/song_v2.wav"
```

**Output (JSON) - Files are identical:**
```json
{
  "file1": "original.mp3",
  "file2": "converted.mp3",
  "filesIdentical": true,
  "differences": [
    "Files are identical - all metadata matches and file hashes match"
  ]
}
```

**Output (JSON) - Files differ:**
```json
{
  "file1": "original.mp3",
  "file2": "converted.mp3",
  "filesIdentical": false,
  "differences": [
    "Duration differs: 00:04:05 vs 00:04:10 (245.5s vs 250.2s)",
    "BitRate differs: 320 kbps vs 192 kbps",
    "File Integrity (MD5 Hash) differs: 7A8B9C0D1E2F3A4B5C6D7E8F9A0B1C2D vs A1B2C3D4E5F6A7B8C9D0E1F2A3B4C5D6"
  ]
}
```

## To Execute from Other .NET Projects

```bash
dotnet run --project <path-to-CommunicationAudio.csproj> -- <action> <parameters>
```

**Examples:**
```bash
# From another .NET project in the same solution
dotnet run --project ../CommunicationAudio/CommunicationAudio.csproj -- metadata "TestAudio/sample.mp3"

# Get metadata and pipe to file
dotnet run --project ./CommunicationAudio/CommunicationAudio.csproj -- metadata "audio.mp3" > metadata.json

# Compare files
dotnet run --project ./CommunicationAudio/CommunicationAudio.csproj -- compare "file1.mp3" "file2.mp3" > comparison.json
```

## Supported Audio Formats

- MP3 (MPEG Audio)
- WAV (Waveform Audio File)
- FLAC (Free Lossless Audio Codec)
- OGG (Ogg Vorbis)
- M4A (MPEG-4 Audio)
- WMA (Windows Media Audio)
- APE (Monkey's Audio)
- And most common audio formats supported by TagLib#

## Error Handling

The application will exit with error code 1 if:
- Required parameters are missing
- Audio file is not found
- Audio file format is not supported
- File cannot be read due to permission or corruption issues

Errors are output to the console in plain text format.

## Exit Codes

- `0` - Success
- `1` - Error occurred

## Notes

- File integrity is calculated using MD5 hash of the complete file content
- Metadata extraction works on file properties; actual audio content is not analyzed for frequency response or quality assessment
- Relative paths are resolved from the current working directory when the application is executed
- JSON output ensures easy parsing and integration with automation frameworks

