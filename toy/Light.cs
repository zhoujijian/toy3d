using OpenTK.Mathematics;

namespace Toy3d.Core {
    public enum LightType {
        Direction,
        Point,
        Spot
    }

    public struct Light {
        public Vector3 direction;
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public float constant;
        public float linear;
        public float quadratic;
        public float radius;
        public Vector3 position;
        public readonly LightType type;

        public Light(LightType type) {
            this.type = type;
        }
    }
}
