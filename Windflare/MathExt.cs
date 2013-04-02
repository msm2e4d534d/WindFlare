using System;
using Microsoft.Xna.Framework;
namespace Windflare {
    static class MathExt {
        public static float Rad(this int deg) {
            return deg * (float)Math.PI / 180f;
        }

        public static Vector3 SphericalToCartesian(float s, float t, float r) {
            return new Vector3(
                (float)(r * Math.Sin(s) * Math.Cos(t)),
                (float)(r * Math.Cos(s)),
                (float)(r * Math.Sin(s) * Math.Sin(t)));
        }
    }
}
