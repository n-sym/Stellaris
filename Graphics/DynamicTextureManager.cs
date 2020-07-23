/*using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellaris.Graphics
{
    public static class DynamicTextureManager
    {
        public static string cachePath = Environment.CurrentDirectory;
        public static bool DoCache(DynamicTexture dynamicTexture)
        {
            if (dynamicTexture.name == "") return false;
            using(MemoryStream memoryStream = new MemoryStream())
            {
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write("dynamictexture");
                for (int index = 0; index < dynamicTexture.maxFrame; index++)
                {
                    binaryWriter.Write("data");
                    binaryWriter.Write(dynamicTexture._width[index]);
                    binaryWriter.Write(dynamicTexture._height[index]);
                    Color[] data = new Color[dynamicTexture._width[index] * dynamicTexture._height[index]];
                    dynamicTexture._texture[index].GetData(data);
                    for (int i = 0; i < data.Length; i++)
                    {
                        binaryWriter.Write(data[i].R);
                        binaryWriter.Write(data[i].G);
                        binaryWriter.Write(data[i].B);
                        binaryWriter.Write(data[i].A);
                    }
                }
                FileStream fileStream = new FileStream(cachePath + Path.DirectorySeparatorChar + dynamicTexture.name + ".dynamictexture", FileMode.Create);
                fileStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
                fileStream.Close();
            }
            return true;
        }
        public static List<Texture2D> ReadTexture(BinaryReader binaryReader, DynamicTexture dynamicTexture)
        {
            List<Texture2D> textures = new List<Texture2D>();
            for (int index = 0; binaryReader.ReadString() == "data"; index++)
            {
                dynamicTexture._width[index] = binaryReader.ReadInt32();
                dynamicTexture._height[index] = binaryReader.ReadInt32();
                Color[] data = new Color[dynamicTexture._width[index] * dynamicTexture._height[index]];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i].R = binaryReader.ReadByte();
                    data[i].G = binaryReader.ReadByte();
                    data[i].B = binaryReader.ReadByte();
                    data[i].A = binaryReader.ReadByte();
                }
                Texture2D texture = new Texture2D(dynamicTexture.graphicsDevice, dynamicTexture._width[index], dynamicTexture._height[index]);
                texture.SetData(data);
                textures.Add(texture);
            }
            return textures;
        }
        public static bool LoadCache(ref DynamicTexture dynamicTexture)
        {
            if (dynamicTexture.name == "") return false;
            using (FileStream fileStream = new FileStream(cachePath + Path.DirectorySeparatorChar + dynamicTexture.name + ".dynamictexture", FileMode.Open))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                if (binaryReader.ReadString() != "dynamictexture") return false;
                dynamicTexture._texture = ReadTexture(binaryReader, dynamicTexture).ToArray();
            }
            return true;
        }
        public static bool LoadFromFile(ref DynamicTexture dynamicTexture)
        {
            return false;
        }
    }}
    */