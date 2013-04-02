using Microsoft.Xna.Framework;
using System;
namespace Windflare.Core {
    class SphericalSubsurface {
        float s, t, u, v;
        float radius;
        Vector3 origin;
        Quaternion transform;

        public SphericalSubsurface(float s, float t, float u, float v,
            float radius, Vector3 origin, Quaternion transform) {
            this.s = s; this.t = t;
            this.u = u; this.v = v;
            this.radius = radius;
            this.origin = origin;
            this.transform = transform;
        }

        public SphericalSubsurface[] Split() {
            var subsurfaces = new SphericalSubsurface[4];
            for (int i = 0; i < 4; i++) {
                float u2 = u / 2,
                      v2 = v / 2,
                      s2 = s + (i % 2 == 1 ? u2 : 0),
                      t2 = t + (i / 2 == 1 ? v2 : 0);
                subsurfaces[i] = new SphericalSubsurface(
                    s2, t2, u2, v2, radius, origin, transform);
            } return subsurfaces;
        }

        public Vector3 Interpolate(float x, float y, float h) {
            var plot = MathExt.SphericalToCartesian(
                s + u * x, t + v * y, radius + h);
            return origin + Vector3.Transform(plot, transform);
        }

        public float EdgeSize {
            get { return radius * u; }
        }

        public Vector3 SurfaceCenter { get { return Interpolate(.5f, .5f, 0); } }
    }
}