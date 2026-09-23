using UnityEngine;

namespace Game.Mono
{
    public sealed class DefaultTween_V : ScrollItemMotion
    {
        const float EnterYaw = -50f;
        const float LeaveYaw = 45f;
        const float EnterShift = 100f / 420f;
        const float LeaveShift = 120f / 420f;
        const float EnterDepth = 100f / 420f;
        const float LeaveDepth = 80f / 420f;

        protected override void ApplyEnter(MotionTarget target, float t)
        {
            Write(target, Mathf.Lerp(EnterYaw, 0f, t), Mathf.Lerp(target.Cross * EnterShift, 0f, t), Mathf.Lerp(-target.Cross * EnterDepth, 0f, t), t);
        }

        protected override void ApplyLeave(MotionTarget target, float t)
        {
            Write(target, Mathf.Lerp(0f, LeaveYaw, t), Mathf.Lerp(0f, -target.Cross * LeaveShift, t), Mathf.Lerp(0f, -target.Cross * LeaveDepth, t), 1f - t);
        }

        protected override void ApplyShown(MotionTarget target)
        {
            Write(target, 0f, 0f, 0f, 1f);
        }

        static void Write(MotionTarget target, float rot, float shift, float depth, float alpha)
        {
            var anim = target.Anim;
            anim.localRotation = Quaternion.Euler(0f, rot, 0f);
            anim.anchoredPosition = new Vector2(shift, 0f);
            Vector3 local = anim.localPosition;
            local.z = depth;
            anim.localPosition = local;
            target.Group.alpha = alpha;
        }
    }
}
