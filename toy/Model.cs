using Toy3d.Game;

namespace Toy3d.Core {
    public class Model: GameObject {
        private Mesh[] meshes;

        public Model(Mesh[] meshes) {
            this.meshes = meshes;
        }

        public override void Draw(IGameWorld world) {
            foreach (var mesh in meshes) {
                mesh.Draw(world);
            }
        }
    }
}