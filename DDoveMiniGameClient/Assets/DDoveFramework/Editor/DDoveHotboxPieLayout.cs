using UnityEngine;

namespace DDoveFramework.Editor
{
    public static class DDoveHotboxPieLayout
    {
        public const float ItemWidth = 124f;
        public const float ItemHeight = 30f;
        public const float Center = 60f;
        public const float RadiusT = 0.36f;
        private const float Pad = 10f;

        public static float Radius(int count)
        {
            if (count <= 1)
            {
                return 140f;
            }

            return Mathf.Max(140f, 52f / Mathf.Sin(Mathf.PI / count));
        }

        public static float PieSize(int count)
        {
            return Radius(count) * 2f + ItemWidth + Pad * 2f;
        }

        public static Vector2 NormalizedOf(DDoveHotboxSlot slot, int index, int count)
        {
            if (slot != null && slot.Placed)
            {
                return new Vector2(Mathf.Clamp01(slot.X), Mathf.Clamp01(slot.Y));
            }

            return CircleNormalized(index, count);
        }

        public static Vector2 CircleNormalized(int index, int count)
        {
            var step = 360f / Mathf.Max(count, 1);
            var angle = (90f - index * step) * Mathf.Deg2Rad;
            return new Vector2(
                0.5f + Mathf.Cos(angle) * RadiusT,
                0.5f - Mathf.Sin(angle) * RadiusT);
        }

        public static Vector2 SlotOrigin(DDoveHotboxSlot slot, int index, int count, float size, float itemWidth, float itemHeight)
        {
            var normalized = NormalizedOf(slot, index, count);
            return ClampOrigin(
                normalized.x * size - itemWidth * 0.5f,
                normalized.y * size - itemHeight * 0.5f,
                size,
                itemWidth,
                itemHeight);
        }

        public static Vector2 SlotOrigin(DDoveHotboxSlot slot, int index, int count, float size)
        {
            return SlotOrigin(slot, index, count, size, ItemWidth, ItemHeight);
        }

        public static Vector2 CenterOrigin(float size, float center)
        {
            return new Vector2((size - center) * 0.5f, (size - center) * 0.5f);
        }

        public static Vector2 CenterOrigin(float size)
        {
            return CenterOrigin(size, Center);
        }

        public static Vector2 ClampOrigin(float left, float top, float size, float itemWidth, float itemHeight)
        {
            return new Vector2(
                Mathf.Clamp(left, 4f, Mathf.Max(4f, size - itemWidth - 4f)),
                Mathf.Clamp(top, 4f, Mathf.Max(4f, size - itemHeight - 4f)));
        }

        public static Vector2 ClampOrigin(float left, float top, float size)
        {
            return ClampOrigin(left, top, size, ItemWidth, ItemHeight);
        }

        public static Vector2 ToNormalized(float left, float top, float size, float itemWidth, float itemHeight)
        {
            var x = size > 1f ? (left + itemWidth * 0.5f) / size : 0.5f;
            var y = size > 1f ? (top + itemHeight * 0.5f) / size : 0.5f;
            return new Vector2(Mathf.Clamp01(x), Mathf.Clamp01(y));
        }

        public static Vector2 ToNormalized(float left, float top, float size)
        {
            return ToNormalized(left, top, size, ItemWidth, ItemHeight);
        }

        public static void Scatter(DDoveHotboxRing ring)
        {
            var count = Mathf.Max(ring.Slots.Count, 1);
            for (var i = 0; i < ring.Slots.Count; i++)
            {
                var normalized = CircleNormalized(i, count);
                ring.Slots[i].Placed = true;
                ring.Slots[i].X = normalized.x;
                ring.Slots[i].Y = normalized.y;
            }
        }
    }
}
