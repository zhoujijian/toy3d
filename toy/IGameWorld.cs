using System.Collections.Generic;
using OpenTK.Mathematics;
using Toy3d.Core;

namespace Toy3d.Game {
    public interface IGameWorld {
        void AddGameObject(GameObject obj);
        void Draw(float elapsed);
        void Draw2D(float elapsed);
        void Update(float elapsed);
        Light GetDirectionLight();
        void SetDirectionLight(Light directionLight);
        void AddPointLight(Light pointLight);
        IEnumerable<Light> GetPointLights();
        Camera GetCamera();
        void SetCamera(Camera camera);
    }

    public class GameWorld : IGameWorld {
        public readonly int width;
        public readonly int height;

        private Camera camera;
        private Light directionLight;
        private List<Light> pointLights = new List<Light>();
        private List<GameObject> gameObjects = new List<GameObject>();

        public GameWorld(int width, int height) {
            this.width = width;
            this.height = height;
        }

        public Camera GetCamera() {
            return camera;
        }

        public void SetCamera(Camera camera) {
            this.camera = camera;
        }

        public Light GetDirectionLight() {
            return directionLight;
        }

        public void SetDirectionLight(Light directionLight) {
            this.directionLight = directionLight;
        }

        public void AddPointLight(Light pointLight) {
            pointLights.Add(pointLight);
        }

        public IEnumerable<Light> GetPointLights() {
            return pointLights;
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

        public void Draw(float elapsed) {
            foreach (var obj in gameObjects) {
                obj.Draw(this);
            }
        }

        public void Update(float elapsed) {
            foreach (var obj in gameObjects) {
                obj.Update(elapsed);
            }
        }
    }
}