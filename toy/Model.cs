using Toy3d.Game;

namespace Toy3d.Core {
    public class Model: GameObject {
        private Mesh[] meshes;

        public Model(Mesh[] meshes) {
            this.meshes = meshes;
        }

        public override void Draw(IGameWorld world, RenderContext context) {
            context.parent *= GetModelMatrix();
            foreach (var mesh in meshes) {
                world.Renderer.DrawMesh(world, mesh, context);
            }
        }
    }
}