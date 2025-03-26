using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace EasyXnb
{
    public class EasyBuilder
    {
        private const string DEFAULT = "default";

        public readonly bool CompileTexturesSetting = SettingOrDefault("CompileTextures", true);
        public readonly bool IgnorePngs = SettingOrDefault("IgnorePng", true);
        public readonly bool CompressOutputSetting = SettingOrDefault("CompressOutput", false);

        public readonly GraphicsProfile ProfileSetting = SettingOrDefault("TargetProfile", GraphicsProfile.Reach);

        public readonly string IntermediateDirectorySetting = SettingOrDefault("IntermediateDirectory", Environment.CurrentDirectory);
        public readonly string InputDirectorySetting = SettingOrDefault("InputDirectory", Environment.CurrentDirectory);
        public readonly string OutputDirectorySetting = SettingOrDefault("OutputDirectory", Environment.CurrentDirectory);

        public readonly bool CloseImmediatelySetting = SettingOrDefault("CloseImmediately", false);
        public readonly bool WaitForInputOnErrorSetting = SettingOrDefault("WaitForInputOnError", true);

        public void Build()
        {
            var exceptionCaught = false;
            try
            {
                var contentItems = GatherContentItems();

                var cb = new ContentBuilder(profile: ProfileSetting.ToString(), compress: CompressOutputSetting);
                cb.PackageContent(contentItems.ToArray(),
                    InputDirectorySetting, OutputDirectorySetting, IntermediateDirectorySetting,
                    false);
            }
            catch (Exception e)
            {
                RemoveCacheFile();

                exceptionCaught = true;
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine((e.InnerException ?? e).Message);
                if (WaitForInputOnErrorSetting)
                {
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            finally
            {
                RemoveCacheFile();
            }


            if (!exceptionCaught || !WaitForInputOnErrorSetting)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[EasyXnb] Done! (Closing in 10 seconds)");
                if (!CloseImmediatelySetting) Thread.Sleep(10000);
                Environment.Exit(0);
            }
        }

        private List<string> GatherContentItems()
        {
            List<string> contentItems = new List<string>(Directory.EnumerateFiles(InputDirectorySetting, "*.fx"));

            contentItems.AddRange(Directory.EnumerateFiles(InputDirectorySetting, "*.fbx"));

            if (CompileTexturesSetting)
            {
                List<string> extensions = ExtensionUtils.TypeToExtensions[ExtensionUtils.FileType.Texture].ToList();
                if (IgnorePngs) extensions.Remove("*.png");

                var list = extensions.AsParallel()
                    .SelectMany(searchPattern => Directory.EnumerateFiles(InputDirectorySetting, searchPattern));
                contentItems.AddRange(list);
            }

            return contentItems;
        }

        private static void RemoveCacheFile()
        {
            const string generatedCacheFile = "ContentPipeline.xml";
            if (File.Exists(generatedCacheFile)) File.Delete(generatedCacheFile);
        }

        private static bool SettingOrDefault(string setting, bool defaultValue)
        {
            return ParseUtils.ParseOrDefault(ConfigurationManager.AppSettings.Get(setting), defaultValue);
        }

        private static TEnum SettingOrDefault<TEnum>(string setting, TEnum defaultValue) where TEnum : struct
        {
            return ParseUtils.ParseOrDefault(ConfigurationManager.AppSettings.Get(setting), defaultValue);
        }

        private static string SettingOrDefault(string setting, string defaultValue)
        {
            var settingValue = ConfigurationManager.AppSettings.Get(setting);
            return string.IsNullOrEmpty(settingValue) || settingValue == DEFAULT ? defaultValue : settingValue;
        }
    }
}