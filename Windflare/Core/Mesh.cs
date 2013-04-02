using Microsoft.Xna.Framework.Graphics;
namespace Windflare.Core {
    class Mesh {
        VertexBuffer buffer;
        PrimitiveType type;
        int primitiveCt;

        public Mesh(VertexBuffer buffer, PrimitiveType type, int primitiveCt) {
            this.buffer = buffer;
            this.type = type;
            this.primitiveCt = primitiveCt;
        }

        public void Render(GraphicsDevice device) {
            device.SetVertexBuffer(buffer);
            device.DrawPrimitives(type , 0, primitiveCt);
        }
    }
}
