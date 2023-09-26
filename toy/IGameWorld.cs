using System.Collections.Generic;
using OpenTK.Mathematics;
using Toy3d.Core;

namespace Toy3d.Game {
    public interface IGameWorld {
        void AddGameObject(GameObject obj);
        void Draw();
    }

    public class GameWorld2D : IGameWorld {
        public readonly int width;
        public readonly int height;

        private List<GameObject> gameObjects = new List<GameObject>();

        public GameWorld2D(int width, int height) {
            this.width = width;
            this.height = height;
        }

        public void AddGameObject(GameObject obj) {
            gameObjects.Add(obj);
        }

        public void Draw() {
            // https://zhuanlan.zhihu.com/p/474879818
            var projection = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
            foreach (var obj in gameObjects) {
                obj.Draw(projection);
            }
        }
    }
}