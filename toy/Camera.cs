using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class Camera: GameObject {
        private Vector3 up;
        private Vector3 front;

        public Vector3 Front => front;
        public Vector3 Up => up;

        public float Yaw         { get; set; }
        public float Pitch       { get; set; }
        public float Sensitivity { get; set; }
        public float Fov         { get; set; }

        public readonly float near;
        public readonly float far;
        public readonly float aspectRatio;

        public Camera(Vector3 front, Vector3 up, float sensitivity, float near, float far, float aspectRatio, float fov) {
            this.front = front;
            this.up = up;
            this.Sensitivity = sensitivity;
            this.near = near;
            this.far = far;
            this.Fov = fov;
            this.aspectRatio = aspectRatio;
        }

        // https://zhuanlan.zhihu.com/p/552252893
        public Matrix4 ViewMatrix {
            // LookAt => 平移矩阵*旋转矩阵
            // TODO: 由父节点计算出世界空间position
            get { return Matrix4.LookAt(transform.position, transform.position + Front, Up); }
        }

        public Matrix4 ProjectionMatrix {
            get { return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), aspectRatio, near, far); }
        }

        public void ResetFront() {
            // 按照pitch俯仰角计算y分量及在xz平面的投影 => 按照yaw偏航角计算xz平面的投影的x/z值
            var radYaw = MathHelper.DegreesToRadians(Yaw);
            var radPitch = MathHelper.DegreesToRadians(Pitch);
            var frontx = MathHelper.Cos(radPitch) * MathHelper.Cos(radYaw);
            var fronty = MathHelper.Sin(radPitch);
            var frontz = MathHelper.Cos(radPitch) * MathHelper.Sin(radYaw);
            front = Vector3.Normalize(new Vector3((float)frontx, (float)fronty, (float)frontz));
        }
    }
}
