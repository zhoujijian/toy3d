using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class PerspectiveCamera {
	public float Yaw { get; set; }
	public float Pitch { get; set; }
	public float Sensitivity { get; set; }
        public Vector3 Position { get; set; }
	public Vector3 Front { get; set; }
	public Vector3 Up { get; set; }
	public float Near { get; set; }
	public float Far { get; set; }
	public float Fov { get; set; }
	public float AspectRatio { get; set; }

        public PerspectiveCamera(Vector3 position, Vector3 front, Vector3 up, float sensitivity, float near, float far, float aspect, float fov) {
            this.Position = position;
            this.Front = front;
            this.Up = up;
            this.Sensitivity = sensitivity;
            this.Near = near;
            this.Far = far;
            this.AspectRatio = aspect;
            this.Fov = fov;
        }

	public Matrix4 ViewMatrix {
            get { return Matrix4.LookAt(Position, Position + Front, Up); }
        }

	public Matrix4 ProjectionMatrix {
            get { return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), AspectRatio, Near, Far); }
        }
    }
}
