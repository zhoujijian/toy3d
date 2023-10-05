using Assimp;
using Assimp.Unmanaged;

namespace Toy3d.Core {
    public class Model {
        private Mesh[] meshes;

        public void Draw() {

        }

        private void ProcessNode(Scene scene, Node node) {
            for (var i=0; i<node.MeshCount; ++i) {
                ProcessMesh(scene.Meshes[node.MeshIndices[i]]);
            }
            for (var i=0; i<node.ChildCount; ++i) {
                ProcessNode(scene, node.Children[i]);
            }
        }

        private void ProcessMesh(Assimp.Mesh mesh) {

        }
    }
}