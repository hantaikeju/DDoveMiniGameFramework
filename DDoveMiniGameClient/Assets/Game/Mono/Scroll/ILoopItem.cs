using UnityEngine;

namespace Game.Mono
{
    public enum LoopDirection
    {
        Vertical = 0,
        Horizontal = 1
    }

    public enum LoopAlign
    {
        Top = 0,
        Middle = 1,
        Bottom = 2,
        Left = 3,
        Right = 4
    }

    public interface ILoopItem<T>
    {
        void Bind(T data, int index);

        void Recycle();
    }

    public interface IScrollItemTween
    {
        void ApplyPage(Vector3 viewCenterWorld, bool horizontal);

        void StopMotion();
    }
}
