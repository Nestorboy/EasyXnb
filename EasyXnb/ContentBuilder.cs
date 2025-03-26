using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework.Content.Pipeline.Tasks;

namespace EasyXnb
{
    // Based on https://github.com/mediaexplorer74/XNBBuilder/tree/main
    public class ContentBuilder : BuildContent
    {
        private BuildEngine _buildEngine;

        public ContentBuilder(string platform = "Windows", string profile = "Reach", bool compress = false)
        {
            TargetPlatform = platform;
            TargetProfile = profile;
            CompressContent = compress;

            _buildEngine = new BuildEngine();
        }

        public string[] PackageContent(string[] fileNames,
            string rootDirectory, string outputDirectory, string intermediateDirectory = null,
            bool shouldLog = false)
        {
            string[] processedFileNames = null;
            try
            {
                if (!shouldLog)
                {
                    _buildEngine.ShouldLog = false;
                }
                else
                {
                    _buildEngine = new BuildEngine("logfile.txt");
                }

                OutputDirectory = outputDirectory;
                IntermediateDirectory = intermediateDirectory;
                RootDirectory = rootDirectory;
                SourceAssets = new ITaskItem[fileNames.Length];

                for (int i = 0; i < SourceAssets.Length; ++i)
                {
                    Dictionary<string, object> metaData = new Dictionary<string, object>();
                    ExtensionUtils.FileType type = ExtensionUtils.GetFileType(fileNames[i]);
                    if (type == ExtensionUtils.FileType.Texture)
                    {
                        metaData.Add("Importer", "TextureImporter");
                        metaData.Add("Processor", "TextureProcessor");
                    }
                    else if (type == ExtensionUtils.FileType.Model)
                    {
                        metaData.Add("Importer", "FbxImporter");
                        metaData.Add("Processor", "ModelProcessor");
                    }
                    else if (type == ExtensionUtils.FileType.Shader)
                    {
                        metaData.Add("Importer", "EffectImporter");
                        metaData.Add("Processor", "EffectProcessor");
                    }

                    // else if (".spritefont".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "FontDescriptionImporter");
                    //     metaData.Add("Processor", "FontDescriptionProcessor");
                    // }
                    // else if (".x".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "XImporter");
                    //     metaData.Add("Processor", "ModelProcessor");
                    // }
                    // else if (".xml".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "XmlImporter");
                    //     metaData.Add("Processor", "PassThroughProcessor");
                    // }
                    // else if (".mp3".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "Mp3Importer");
                    //     if (BuildAudioAsSoundEffects)
                    //         metaData.Add("Processor", "SoundEffectProcessor");
                    //     else if (BuildAudioAsSongs)
                    //         metaData.Add("Processor", "SongProcessor");
                    //     else
                    //         metaData.Add("Processor", "SoundEffectProcessor");
                    // }
                    // else if (".wma".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "WmaImporter");
                    //     if (BuildAudioAsSoundEffects)
                    //         metaData.Add("Processor", "SoundEffectProcessor");
                    //     else if (BuildAudioAsSongs)
                    //         metaData.Add("Processor", "SongProcessor");
                    //     else
                    //         metaData.Add("Processor", "SoundEffectProcessor");
                    // }
                    // else if (".wav".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "WavImporter");
                    //     if (BuildAudioAsSoundEffects)
                    //         metaData.Add("Processor", "SoundEffectProcessor");
                    //     else if (BuildAudioAsSongs)
                    //         metaData.Add("Processor", "SongProcessor");
                    //     else
                    //         metaData.Add("Processor", "SoundEffectProcessor");
                    // }
                    // else if (".wmv".Contains(fileType))
                    // {
                    //     metaData.Add("Importer", "WmvImporter");
                    //     metaData.Add("Processor", "VideoProcessor");
                    // }
                    metaData.Add("Name", Path.GetFileNameWithoutExtension(fileNames[i]));

                    SourceAssets[i] = new TaskItem(fileNames[i], metaData);
                }

                _buildEngine.Begin();

                string xnaFolder = $@"{Environment.CurrentDirectory}\";

                PipelineAssemblies = new ITaskItem[]
                {
                    new TaskItem(xnaFolder + "Microsoft.Xna.Framework.dll"),
                    new TaskItem(xnaFolder + "Microsoft.Xna.Framework.Content.Pipeline.dll"),
                    // new TaskItem(xnaInstallFolder + "Microsoft.Xna.Framework.Content.Pipeline.AudioImporters.dll"),
                    new TaskItem(xnaFolder + "Microsoft.Xna.Framework.Content.Pipeline.EffectImporter.dll"),
                    new TaskItem(xnaFolder + "Microsoft.Xna.Framework.Content.Pipeline.FBXImporter.dll"),
                    new TaskItem(xnaFolder + "Microsoft.Xna.Framework.Content.Pipeline.TextureImporter.dll"),
                    // new TaskItem(xnaInstallFolder + "Microsoft.Xna.Framework.Content.Pipeline.VideoImporters.dll"),
                    // new TaskItem(xnaInstallFolder + "Microsoft.Xna.Framework.Content.Pipeline.XImporter.dll"),
                };

                BuildEngine = _buildEngine;
                IntermediateDirectory = Directory.GetCurrentDirectory();

                if (!Execute())
                {
                    List<string> errors = _buildEngine.GetErrors();
                    errors.Insert(0, "[EasyXnb] Failed to execute BuildContent Task:");
                    throw new Exception(string.Join("\n", errors));
                }

                if (OutputContentFiles != null)
                {
                    processedFileNames = new string[OutputContentFiles.Length];
                    for (int i = 0; i < processedFileNames.Length; ++i)
                    {
                        processedFileNames[i] = OutputContentFiles[i].ToString();
                    }
                }
            }
            finally
            {
                //No matter what, we want to flush and close the logger in the BuildEngine.
                _buildEngine.End();
            }

            //Returns a list of files with their full path, allowing a file to be converted and then moved into appropriate
            //locations based on where the programmer decides it should go.  Mainly useful for dynamic conversions in game.
            return processedFileNames;
        }
    }
}