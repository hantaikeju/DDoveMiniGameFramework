using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mono
{
    public abstract class ScrollItemMotion : MonoBehaviour, IScrollItemTween
    {
        const float EnterVisible = 0.2f;
        const string AnimNode = "Anim";

        protected struct MotionTarget
        {
            public RectTransform Anim;
            public CanvasGroup Group;
            public RectTransform Root;
            public float Cross;
            public float Along;
            public bool Horizontal;
        }

        enum Phase
        {
            None,
            Hidden,
            Entering,
            Shown
        }

        Phase _phase;
        bool _tracked;
        float _along;
        int _version;
        Tween _tween;

        protected virtual float Duration => 0.7f;

        protected abstract void ApplyEnter(MotionTarget target, float t);

        protected abstract void ApplyLeave(MotionTarget target, float t);

        protected abstract void ApplyShown(MotionTarget target);

        public void ApplyPage(Vector3 viewCenterWorld, bool horizontal)
        {
            if (!TryTarget(horizontal, out var target))
            {
                return;
            }

            float stride = Stride(target.Root, horizontal);
            float along = horizontal ? target.Root.anchoredPosition.x : target.Root.anchoredPosition.y;
            if (_tracked && Mathf.Abs(along - _along) > stride * 1.5f)
            {
                StopTween();
                _phase = Phase.None;
            }

            _along = along;
            _tracked = true;

            if (_phase == Phase.None)
            {
                ApplyEnter(target, 0f);
                HoldRoot(target);
                _phase = Phase.Hidden;
            }

            float visible = VisibleFraction(target.Root, horizontal);
            if (visible >= EnterVisible)
            {
                if (_phase == Phase.Shown || _phase == Phase.Entering)
                {
                    return;
                }

                PlayEnter();
                return;
            }

            if (visible > 0f || _phase == Phase.Hidden)
            {
                return;
            }

            StopTween();
            ApplyEnter(target, 0f);
            HoldRoot(target);
            _phase = Phase.Hidden;
        }

        public void StopMotion()
        {
            StopTween();
            _phase = Phase.None;
            _tracked = false;
            transform.localScale = Vector3.one;
            if (!TryTarget(IsHorizontal(transform as RectTransform), out var target))
            {
                return;
            }

            ApplyShown(target);
            HoldRoot(target);
        }

        void PlayEnter()
        {
            StopTween();
            int version = _version;
            _phase = Phase.Entering;
            if (TryTarget(IsHorizontal(transform as RectTransform), out var first))
            {
                ApplyEnter(first, 0f);
                HoldRoot(first);
            }

            _tween = Tween.Custom(this, 0f, 1f, Duration, (self, t) =>
            {
                if (self._version != version)
                {
                    return;
                }

                if (!self.TryTarget(IsHorizontal(self.transform as RectTransform), out var target))
                {
                    return;
                }

                self.ApplyEnter(target, t);
                HoldRoot(target);
            }, Ease.OutCubic);
            _tween.OnComplete(this, self =>
            {
                if (self._version != version)
                {
                    return;
                }

                if (self.TryTarget(IsHorizontal(self.transform as RectTransform), out var target))
                {
                    self.ApplyEnter(target, 1f);
                    HoldRoot(target);
                }

                self._phase = Phase.Shown;
            });
        }

        void StopTween()
        {
            _version++;
            if (_tween.isAlive)
            {
                _tween.Stop();
            }
        }

        bool TryTarget(bool horizontal, out MotionTarget target)
        {
            target = default;
            var root = transform as RectTransform;
            if (root == null)
            {
                return false;
            }

            var anim = transform.Find(AnimNode) as RectTransform;
            if (anim == null)
            {
                return false;
            }

            var group = anim.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = anim.gameObject.AddComponent<CanvasGroup>();
            }

            var scroll = root.GetComponentInParent<ScrollRect>(true);
            var view = scroll != null ? scroll.viewport : null;
            if (view == null)
            {
                return false;
            }

            target = new MotionTarget
            {
                Anim = anim,
                Group = group,
                Root = root,
                Cross = Mathf.Max(1f, horizontal ? view.rect.height : view.rect.width),
                Along = Mathf.Max(1f, horizontal ? view.rect.width : view.rect.height),
                Horizontal = horizontal
            };
            return true;
        }

        static void HoldRoot(MotionTarget target)
        {
            if (target.Root != null)
            {
                target.Root.localScale = Vector3.one;
            }
        }

        static float Stride(RectTransform root, bool horizontal)
        {
            var list = root.GetComponentInParent<LoopList>(true);
            if (list != null)
            {
                return list.MotionStride;
            }

            return Mathf.Max(1f, horizontal ? root.rect.width : root.rect.height);
        }

        static float VisibleFraction(RectTransform root, bool horizontal)
        {
            var scroll = root.GetComponentInParent<ScrollRect>(true);
            var view = scroll != null ? scroll.viewport : null;
            if (view == null)
            {
                return 0f;
            }

            Vector3 min = view.InverseTransformPoint(root.TransformPoint(new Vector3(root.rect.xMin, root.rect.yMin, 0f)));
            Vector3 max = view.InverseTransformPoint(root.TransformPoint(new Vector3(root.rect.xMax, root.rect.yMax, 0f)));
            float itemMin = horizontal ? Mathf.Min(min.x, max.x) : Mathf.Min(min.y, max.y);
            float itemMax = horizontal ? Mathf.Max(min.x, max.x) : Mathf.Max(min.y, max.y);
            float viewMin = horizontal ? view.rect.xMin : view.rect.yMin;
            float viewMax = horizontal ? view.rect.xMax : view.rect.yMax;
            float size = Mathf.Max(1f, itemMax - itemMin);
            float overlap = Mathf.Min(itemMax, viewMax) - Mathf.Max(itemMin, viewMin);
            return Mathf.Clamp01(overlap / size);
        }

        static bool IsHorizontal(RectTransform root)
        {
            if (root == null)
            {
                return false;
            }

            var list = root.GetComponentInParent<LoopList>(true);
            return list != null && list.HorizontalMotion;
        }
    }
}
