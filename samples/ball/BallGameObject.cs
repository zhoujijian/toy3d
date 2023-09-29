using Toy3d.Core;
using OpenTK.Mathematics;

public class BallGameObject : GameObject {
    private Vector2 velocity;

    public BallGameObject(Sprite sprite, Vector2 velocity) : base() {
        this.sprite = sprite;
        this.velocity = velocity;
    }

    public void Move(float dt, float windowWidth, float windowHeight) {
        transform.position.X += dt * velocity.X;
        transform.position.Y += dt * velocity.Y;

        if (transform.position.X < 0 || transform.position.X > windowWidth) {
            velocity.X *= -1;
            if (transform.position.X < 0) { transform.position.X = 0; }
            if (transform.position.X > windowWidth) { transform.position.X = windowWidth; }
        }
        if (transform.position.Y < 0 || transform.position.Y > windowHeight) {
            velocity.Y *= -1;
            if (transform.position.Y < 0) { transform.position.Y = 0; }
            if (transform.position.Y > windowHeight) { transform.position.Y = windowHeight; }
        }
    }
}