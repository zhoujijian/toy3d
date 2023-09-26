using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class GameObject {
        public Transform transform;
	    public Sprite sprite;

        public GameObject(Sprite sprite) {
            this.sprite = sprite;
            transform.scale = Vector3.One;
        }

        public void Draw(Matrix4 projection) {
            Matrix4.CreateTranslation(transform.position, out var t);
            Matrix4.CreateScale(transform.scale, out var s);
            Matrix4.CreateRotationX(transform.rotation.X, out var rx);
            Matrix4.CreateRotationY(transform.rotation.Y, out var ry);
            Matrix4.CreateRotationZ(transform.rotation.Z, out var rz);
            // 右乘，矩阵乘法满足结合律(不满足交换律)
            // V => T * (R * (S * Vertex)) => (T * R * S) * Vertex
            sprite.Draw(s * (rx * ry * rz) * t, projection);
        }
    }
}
