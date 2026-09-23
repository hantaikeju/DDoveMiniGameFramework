using UnityEngine;

namespace Game.Mono
{
    public sealed class DefaultTween_H : ScrollItemMotion
    {
        const float EnterDrop = 0.55f;
        const float LeaveDrop = 0.4f;

        protected override void ApplyEnter(MotionTarget target, float t)
        {
            float span = Span(target);
            Write(target, Mathf.Lerp(span * EnterDrop, 0f, t), t);
        }

        protected override void ApplyLeave(MotionTarget target, float t)
        {
            float span = Span(target);
            Write(target, Mathf.Lerp(0f, -span * LeaveDrop, t), 1f - t);
        }

        protected override void ApplyShown(MotionTarget target)
        {
            Write(target, 0f, 1f);
        }

        static float Span(MotionTarget target)
        {
            var root = target.Root;
            if (root == null)
            {
                return target.Cross;
            }

            return Mathf.Max(1f, root.rect.height);
        }

        static void Write(MotionTarget target, float shift, float alpha)
        {
            var anim = target.Anim;
            anim.localRotation = Quaternion.identity;
            anim.anchoredPosition = new Vector2(0f, shift);
            Vector3 local = anim.localPosition;
            local.z = 0f;
            anim.localPosition = local;
            target.Group.alpha = alpha;
        }
    }
}
