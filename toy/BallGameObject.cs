using Toy3d.Core;
using OpenTK.Mathematics;

namespace Toy3d.Samples {
    public class BallGameObject : GameObject {
        private Vector2 velocity;

        public BallGameObject(Sprite sprite, Vector2 velocity) : base(sprite) {
            this.velocity = velocity;
        }

        public void Move(float dt, float windowWidth, float windowHeight) {
            Transform.position.X += dt * velocity.X;
            Transform.position.Y += dt * velocity.Y;

            if (Transform.position.X < 0 || Transform.position.X > windowWidth) {
                velocity.X *= -1;
                if (Transform.position.X < 0) { Transform.position.X = 0; }
                if (Transform.position.X > windowWidth) { Transform.position.X = windowWidth; }
            }
            if (Transform.position.Y < 0 || Transform.position.Y > windowHeight) {
                velocity.Y *= -1;
                if (Transform.position.Y < 0) { Transform.position.Y = 0; }
                if (Transform.position.Y > windowHeight) { Transform.position.Y = windowHeight; }
            }
        }
    }
}
