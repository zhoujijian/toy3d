namespace Toy3d.Game {
    public interface IGameLifecycle {
        void OnLoad(IGameWorld world);
        void OnRenderFrame(float elapsed);
        void OnUpdateFrame(float elapsed);
    }
}