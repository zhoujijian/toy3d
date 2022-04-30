using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class Transform {
        public Vector3 position = Vector3.Zero;
        public Vector3 scale = Vector3.One;
        public Vector3 rotation = Vector3.Zero;

        public Matrix4 ModelMatrix {
            get {
                var t = Matrix4.CreateTranslation(position);
                var s = Matrix4.CreateScale(scale);
                var r = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z);
                return t * s * r;
            }
        }
    }
}
