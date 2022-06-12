using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class OrthogonalCamera2D {
        private float width;
        private float height;
        private Vector3 position;

        public float Width { get { return width; } }
        public float Height { get { return height; } }

        public OrthogonalCamera2D(float width, float height, Vector3 position) {
            this.width = width;
            this.height = height;
            this.position = position;
        }

        public Vector3 Position {
            get { return position; }
            set { position = value; }
        }

        public Matrix4 ProjectionMatrix {
            get {
                return Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
            }
        }
    }
}