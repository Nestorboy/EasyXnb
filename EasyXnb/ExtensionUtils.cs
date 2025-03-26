using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace EasyXnb
{
    public static class ExtensionUtils
    {
        public enum FileType
        {
            Unsupported,
            Texture,
            Model,
            Shader,
        }

        public static readonly ReadOnlyDictionary<FileType, string[]> TypeToExtensions =
            new ReadOnlyDictionary<FileType, string[]>(new Dictionary<FileType, string[]>
            {
                {
                    FileType.Texture, new[]
                    {
                        ".bmp",
                        ".dds",
                        ".dib",
                        ".hdr",
                        ".jpg",
                        ".jpeg",
                        ".pfm",
                        ".png",
                        ".tga",
                    }
                },
                {
                    FileType.Model, new[] { ".fbx" }
                },
                {
                    FileType.Shader, new[] { ".fx" }
                },
            });

        public static readonly ReadOnlyDictionary<string, FileType> ExtensionToType =
            new ReadOnlyDictionary<string, FileType>(new Dictionary<string, FileType>
            {
                { ".bmp", FileType.Texture },
                { ".dds", FileType.Texture },
                { ".dib", FileType.Texture },
                { ".hdr", FileType.Texture },
                { ".jpg", FileType.Texture },
                { ".jpeg", FileType.Texture },
                { ".pfm", FileType.Texture },
                { ".png", FileType.Texture },
                { ".ppm", FileType.Texture },
                { ".tga", FileType.Texture },
                { ".fbx", FileType.Model },
                { ".fx", FileType.Shader },
            });

        public static FileType GetFileType(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return ExtensionToType.TryGetValue(extension, out FileType fileType) ? fileType : FileType.Unsupported;
        }
    }
}