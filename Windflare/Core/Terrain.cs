using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using VPNT = Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture;
using System;

namespace Windflare.Core {
    class Terrain {
        float[,] heightmap;
        Vector2 start, size;

        public Terrain(float[,] heightmap, int detailCount)
            : this(heightmap, detailCount, Vector2.Zero, new Vector2(heightmap.GetLength(0) - 1)) { }

        private Terrain(float[,] heightmap, int detailCount,
            Vector2 start, Vector2 size) {
            this.heightmap = heightmap;
            this.DetailCount = detailCount;
            this.start = start; this.size = size;
        }

        public Terrain[] Split() {
            var submaps = new Terrain[4];
            var halfSize = size / 2;
            for (int i = 0; i < 4; i++) {
                var substart = new Vector2(
                    start.X + (i % 2 == 1 ? halfSize.X : 0),
                    start.Y + (i / 2 == 1 ? halfSize.Y : 0));
                submaps[i] = new Terrain(heightmap, DetailCount, substart, halfSize);
            } return submaps;
        }

        public Mesh GenerateMesh(GraphicsDevice device, SphericalSubsurface surface) {
            var verticles = new List<VPNT>();
            var details = (float)DetailCount;
            for (int y = 0; y < DetailCount; y++) {
                for (int x = 0; x < DetailCount; x++) {
                    float xPos0 = start.X + size.X / details * x,
                          yPos0 = start.Y + size.Y / details * y,
                          xPos1 = start.X + size.X / details * (x + 1),
                          yPos1 = start.Y + size.Y / details * (y + 1);
                    float height00 = SubhexelInterpolate(xPos0, yPos0),
                          height01 = SubhexelInterpolate(xPos0, yPos1),
                          height10 = SubhexelInterpolate(xPos1, yPos0),
                          height11 = SubhexelInterpolate(xPos1, yPos1);
                    Vector3 pos00 = surface.Interpolate(x / details, y / details, height00),
                            pos01 = surface.Interpolate(x / details, (y + 1) / details, height01),
                            pos10 = surface.Interpolate((x + 1) / details, y / details, height10),
                            pos11 = surface.Interpolate((x + 1) / details, (y + 1) / details, height11),
                            normal = Vector3.Cross(pos01 - pos00, pos10 - pos00);
                    VPNT v00 = new VPNT(pos00, normal, Vector2.Zero),
                         v01 = new VPNT(pos01, normal, Vector2.Zero),
                         v10 = new VPNT(pos10, normal, Vector2.Zero),
                         v11 = new VPNT(pos11, normal, Vector2.Zero);
                    verticles.AddRange(new[] { v00, v01, v10, v01, v10, v11 });
                }
            }
            var vbuffer = new VertexBuffer(device, VPNT.VertexDeclaration, verticles.Count, BufferUsage.None);
            vbuffer.SetData(verticles.ToArray());
            return new Mesh(vbuffer, PrimitiveType.TriangleList, verticles.Count / 3);
        }

        float SubhexelInterpolate(float x, float y) {
            int x0 = (int)Math.Floor(x), y0 = (int)Math.Floor(y),
                x1 = (int)Math.Ceiling(x), y1 = (int)Math.Ceiling(y);
            float sx = x - x0, sy = y - y0;
            var row0 = Interpolate(heightmap[x0, y0], heightmap[x1, y0], sx);
            var row1 = Interpolate(heightmap[x0, y1], heightmap[x1, y1], sx);
            return Interpolate(row0, row1, sy);
        }

        float Interpolate(float a, float b, float frac) {
            float smoothstep = frac * frac * (3 - 2 * frac);
            return a * (1 - smoothstep) + b * smoothstep;
        }

        public int DetailCount { get; private set; }
        public bool CanSplit { get { return true; } }
    }
}
