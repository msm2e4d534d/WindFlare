using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Windflare.Core {
    class TerrainGenerator {
        public float[,] FromImage(Texture2D image) {
            Color[] data = new Color[image.Width * image.Height];
            image.GetData(data);

            float[,] map = new float[image.Width, image.Height];
            for (int x = 0; x < image.Width; x++) {
                for (int y = 0; y < image.Height; y++) {
                    map[x, y] = data[x + y * image.Width].B * 10;
                }
            } return map;
        }
    }
}
