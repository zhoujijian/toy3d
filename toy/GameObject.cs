using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class GameObject {
        public Transform transform;
	    public Sprite Sprite { get; set; }

        public GameObject(Sprite sprite) {
            this.Sprite = sprite;
        }

        public void Draw(OrthogonalCamera2D camera) {
            Matrix4.CreateTranslation(transform.position, out var t);
            Matrix4.CreateScale(transform.scale, out var s);
            Matrix4.CreateRotationX(transform.rotation.X, out var rx);
            Matrix4.CreateRotationY(transform.rotation.Y, out var ry);
            Matrix4.CreateRotationZ(transform.rotation.Z, out var rz);
            Sprite.Draw(rx * ry * rz * s * t, camera);
        }
    }
}
