using System.Collections.Generic;
using OpenTK.Mathematics;
using Toy3d.Core;

namespace Toy3d.Game {
    public interface IGameWorld {
        Camera Camera { get; set; }
        Light DirectionLight { get; set; }
        Skybox Skybox { get; set; }
        Renderer Renderer { get; }

        void AddGameObject(GameObject obj);
        void Draw2D(float elapsed);
        void DrawWindow(float elapsed);
        void Update(float elapsed);
        void AddPointLight(Light pointLight);
        IEnumerable<Light> GetPointLights();
        IEnumerable<GameObject> GetGameObjects();
    }

    public class GameWorld : IGameWorld {
        public readonly int width;
        public readonly int height;

        private Renderer renderer;
        private List<Light> pointLights = new List<Light>();
        private List<GameObject> gameObjects = new List<GameObject>();
        
        public Skybox Skybox { get; set; }
        public Camera Camera { get; set; }
        public Light DirectionLight { get; set; }
        public Renderer Renderer => renderer;

        public GameWorld(int width, int height) {
            this.width = width;
            this.height = height;
            renderer = new Renderer(width, height);
        }

        public void AddPointLight(Light pointLight) {
            pointLights.Add(pointLight);
        }

        public IEnumerable<Light> GetPointLights() {
            return pointLights;
        }

        public IEnumerable<GameObject> GetGameObjects() {
            return gameObjects;
        }

        public void AddGameObject(GameObject obj) {
            gameObjects.Add(obj);
        }

        public void Draw2D(float elapsed) {
            // https://zhuanlan.zhihu.com/p/474879818
            var projection = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
            foreach (var obj in gameObjects) {
                obj.Draw(projection);
            }
        }

        public void DrawWindow(float elapsed) {
            renderer.Draw(this, elapsed);
        }

        public void Update(float elapsed) {
            foreach (var obj in gameObjects) {
                obj.Update(elapsed);
            }
        }
    }
}