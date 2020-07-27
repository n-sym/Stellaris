﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stellaris.Graphics
{
    public class DynamicTexture
    {
        public string name;
        public int Width => _width[frame];
        public int Height => _height[frame];
        public Vector2 Size => new Vector2(Width, Height);
        protected int[] _width;
        protected int[] _height;
        public int frame;
        public int maxFrame;
        public Texture2D Texture
        {
            get
            {
                if (PrivateGenerate()) return _texture[frame];
                return null;
            }
            set
            {
                _texture[frame] = value;
            }
        }
        public GraphicsDevice graphicsDevice;
        public Texture2D[] _texture;
        public Func<List<Color[]>, int, List<Color[]>> preGenerate;
        public static string cachePath = Environment.CurrentDirectory;
        #region Ctor
        private void Initialize(GraphicsDevice graphicsDevice, int[] width, int[] height, int frame, int maxFrame, string name = "")
        {
            _width = width;
            _height = height;
            _texture = new Texture2D[maxFrame];
            this.graphicsDevice = graphicsDevice;
            this._width = width;
            this._height = height;
            this.frame = frame;
            this.maxFrame = maxFrame;
            this.name = name;
        }
        public DynamicTexture(GraphicsDevice graphicsDevice, int width, int height, string name = "")
        {
            Initialize(graphicsDevice, new int[] { width }, new int[] { height }, 0, 1, name);
        }
        public DynamicTexture(GraphicsDevice graphicsDevice, int[] width, int[] height, int maxFrame, string name = "")
        {
            Initialize(graphicsDevice, width, height, 0, maxFrame, name);
        }
        public DynamicTexture(GraphicsDevice graphicsDevice, int[] width, int[] height, int frame, int maxFrame, string name = "")
        {
            Initialize(graphicsDevice, width, height, frame, maxFrame, name);
        }
        /// <summary>
        /// 希望DynamicTexture没事----一般不用这个构造方法
        /// </summary>
        public DynamicTexture(GraphicsDevice graphicsDevice, string name = "")
        {
            Initialize(graphicsDevice, new int[] { 1 }, new int[] { 1 }, 0, 1, name);
        }
        #endregion
        /// <summary>
        /// 生成DynamicTexture的数据
        /// </summary>
        /// <returns>数据</returns>
        protected virtual List<Color[]> Generate()
        {
            List<Color[]> data = new List<Color[]>();
            for (int i = 0; i < maxFrame; i++)
            {
                data.Add(new Color[_width[i] * _height[i]]);
            }
            if (preGenerate != null)
            {
                data = preGenerate.Invoke(data, frame);
                //执行了自定义的生成函数了，无需进行默认生成
                return data;
            }
            //默认生成光球
            for (int i = 0; i < maxFrame; i++)
            {
                Vector2 vector2 = new Vector2(_width[i], _height[i]) / 2;
                float length = vector2.Length() / 1.41f;
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = Color.White * (1 - (Vector2.Distance(IndexToPoint(j).ToVector2(), vector2) / length));
                }
            }
            return data;
        }
        bool PrivateGenerate()
        {
            if (_texture[frame] != null) return true; //有贴图了，无需进行生成
            for (int i = 0; i < maxFrame; i++)
            {
                _texture[i] = new Texture2D(graphicsDevice, _width[i], _height[i]);
            }
            List<Color[]> data = Generate();
            for (int i = 0; i < maxFrame; i++)
            {
                _texture[i].SetData(data[i]);
            }
            return true;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            PrivateGenerate();
            spriteBatch.Draw(Texture, position, source, color, rotation, origin, scale, effects, layerDepth);
        }
        public void Draw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            PrivateGenerate();
            spriteBatch.Draw(_texture[frame], position, source, color, rotation, origin, scale, effects, layerDepth);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            PrivateGenerate();
            spriteBatch.Draw(Texture, position, source, color, rotation, origin, scale, effects, layerDepth);
        }
        public void Draw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            PrivateGenerate();
            spriteBatch.Draw(_texture[frame], position, source, color, rotation, origin, scale, effects, layerDepth);
        }
        protected Point IndexToPoint(int index)
        {
            int y = index / _width[frame];
            int x = y * _width[frame];
            return new Point(index - x + 1, y + 1);
        }
        protected Vector2 IndexToVector(int index)
        {
            float y = index / _width[frame];
            float x = y * _width[frame];
            return new Vector2(index - x + 1, y + 1);
        }
        protected int PointToIndex(Point point)
        {
            return point.X - 1 + (point.Y - 1) * _width[frame];
        }
        protected int PointToIndex(int x, int y)
        {
            return x - 1 + (y - 1) * _width[frame];
        }
        /// <summary>
        /// 缓存DynamicTexture到一文件
        /// </summary>
        /// <returns>是否成功</returns>
        public bool DoCache()
        {
            if (name == "") return false;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write("dynamictexture");
                for (int index = 0; index < maxFrame; index++)
                {
                    binaryWriter.Write(_width[index]);
                    binaryWriter.Write(_height[index]);
                    Color[] data = new Color[_width[index] * _height[index]];
                    _texture[index].GetData(data);
                    for (int i = 0; i < data.Length; i++)
                    {
                        binaryWriter.Write(data[i].R);
                        binaryWriter.Write(data[i].G);
                        binaryWriter.Write(data[i].B);
                        binaryWriter.Write(data[i].A);
                    }
                }
                FileStream fileStream = new FileStream(cachePath + Path.DirectorySeparatorChar + name + ".dynamictexture", FileMode.Create);
                fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                fileStream.Close();
            }
            return true;
        }
        private List<Texture2D> ReadTexture(BinaryReader binaryReader)
        {
            List<Texture2D> textures = new List<Texture2D>();
            for (int index = 0; binaryReader.BaseStream.Position != binaryReader.BaseStream.Length; index++)
            {
                _width[index] = binaryReader.ReadInt32();
                _height[index] = binaryReader.ReadInt32();
                Color[] data = new Color[_width[index] * _height[index]];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i].R = binaryReader.ReadByte();
                    data[i].G = binaryReader.ReadByte();
                    data[i].B = binaryReader.ReadByte();
                    data[i].A = binaryReader.ReadByte();
                }
                Texture2D texture = new Texture2D(graphicsDevice, _width[index], _height[index]);
                texture.SetData(data);
                textures.Add(texture);
            }
            return textures;
        }
        /// <summary>
        /// 从一文件中读取DynamicTexture
        /// </summary>
        /// <returns>是否成功</returns>
        public bool LoadCache()
        {
            if (name == "") return false;
            string path = cachePath + Path.DirectorySeparatorChar + name + ".dynamictexture";
            if (!File.Exists(path)) return false;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                if (binaryReader.ReadString() != "dynamictexture") return false;
                _texture = ReadTexture(binaryReader).ToArray();
            }
            return true;
        }
    }
}
