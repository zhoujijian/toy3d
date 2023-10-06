using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class GameObject {
        private GameObject parent;
        private readonly List<GameObject> children = new List<GameObject>();

        public Transform transform;
        public ParticleEmitter emitter;

        public GameObject Parent {
            get { return parent; }
            set {
                if (parent != null) {
                    parent.children.Remove(this);
                }
                parent = value;
                parent.children.Add(this);
            }
        }

        public GameObject() {
            transform.scale = Vector3.One;
        }

        public virtual void Draw(Matrix4 projection) { }

        public virtual void Draw(Camera camera) { }

        public ICollection<GameObject> GetChildren() {
            return children;
        }

        public void Update(float elapsed) {
            emitter?.Update(Vector3.Zero, elapsed, true);
        }

        public Matrix4 GetModelMatrix() {
            Matrix4.CreateScale(transform.scale, out var s);
            Matrix4.CreateRotationX(transform.rotation.X, out var rx);
            Matrix4.CreateRotationY(transform.rotation.Y, out var ry);
            Matrix4.CreateRotationZ(transform.rotation.Z, out var rz);
            Matrix4.CreateTranslation(transform.position, out var t);
            
            // 右乘形式，矩阵乘法满足结合律(不满足交换律)
            // V => T * (R * (S * Vertex)) => (T * R * S) * Vertex
            // 因为OpenTK的矩阵是转置形式，故是按照相反的顺序计算的
            return s * (rx * ry * rz) * t;
        }
    }
}
