using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
namespace Windflare.Core {
    class ProgressiveNode {
        SphericalSubsurface surface;
        Terrain terrain;
        Mesh mesh;
        ProgressiveNode[] subnodes;

        public ProgressiveNode(SphericalSubsurface surface, Terrain heightmap) {
            this.surface = surface;
            this.terrain = heightmap;
        }

        public void Render(GraphicsDevice device, Vector3 camera) {
            if (subnodes == null && CameraTooClose(camera)) {
                FetchSubnodes();
            }
            if (mesh == null) {
                mesh = terrain.GenerateMesh(device, surface);
            }

            if (subnodes != null && CameraTooClose(camera)) {
                foreach (var node in subnodes) {
                    node.Render(device, camera);
                }
            } else {
                mesh.Render(device);
            }
        }

        bool CameraTooClose(Vector3 cameraPos) {
            var toSurface = (cameraPos - surface.SurfaceCenter).Length();
            var detailSize = surface.EdgeSize / terrain.DetailCount;
            return (detailSize < 0.3) ? false : toSurface < 150 * detailSize;
        }

        void FetchSubnodes() {
            if (terrain.CanSplit) {
                var submaps = terrain.Split();
                var subsurfaces = surface.Split();

                subnodes = subsurfaces
                    .Zip(submaps, (s, h) => new ProgressiveNode(s, h))
                    .ToArray();
            }
        }
    }
}
