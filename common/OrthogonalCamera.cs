using OpenTK.Mathematics;

namespace Toy3d.Common {
    public class OrthogonalCamera {
        private float width;
        private float height;
        private Vector3 position;

        public OrthogonalCamera(float width, float height, Vector3 position) {
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
                return Matrix4.CreateOrthographic(width, height, -1, 1);
            }
        }
    }
}
