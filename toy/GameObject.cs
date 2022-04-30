namespace Toy3d.Core {
    public class GameObject {
        public Transform Transform { get; private set; }
	public Sprite Sprite { get; set; }

        public GameObject(Sprite sprite) {
            this.Sprite = sprite;
            this.Transform = new Transform();
        }

        public void Draw(SpriteRenderer renderer, OrthogonalCamera2D camera) {
            renderer.Draw(Sprite, Transform.ModelMatrix, camera);
        }
    }
}
