using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
#pragma warning disable CS0618

namespace PrimeTween {
    public partial struct Tween {
        #if UNITASK_INSTALLED
        public static implicit operator Cysharp.Threading.Tasks.UniTask(Tween tween) => tween.ToUniTask();
        public async Cysharp.Threading.Tasks.UniTask ToUniTask() => await this;
        #endif

        /// <summary>Makes this animation cancellable by calling 'Stop' internally when <paramref name="cancellationToken"/> is canceled.<br/>
        /// Cancellation via <see cref="CancellationTokenSource.Cancel()"/> is not immediate and happens on the next PrimeTween update. But if the token is ALREADY canceled, the animation is stopped immediately.<br/>
        /// If the token is canceled while the animation is awaited with 'await', the 'await' throws <see cref="OperationCanceledException"/> when it resumes, even if the animation has already completed naturally.<br/>
        /// But awaiting an animation that was canceled and recycled (typically the frame after the cancellation) does NOT throw and results in no-op (same as awaiting an animation that's not alive).<br/></summary>
        public Tween SetCancellationToken(CancellationToken cancellationToken) {
            if (!ValidateCanModify(this) || !cancellationToken.CanBeCanceled) {
                return this;
            }
            if (tween.data.HasCancellationToken) {
                tween.managedData.LogErrorWithStackTrace(nameof(SetCancellationToken) + "() was already called on this animation. Applying more than one CancellationToken to the same animation is not supported and most likely indicates a bug.");
                return this;
            }
            tween.cancellationToken = cancellationToken;
            tween.data.HasCancellationToken = true;
            if (cancellationToken.IsCancellationRequested) {
                StopTweenOrSequence(this);
            }
            return this;
        }

        internal static void StopTweenOrSequence(Tween tweenOrSequenceRoot) {
            Assert.IsTrue(tweenOrSequenceRoot.isAliveInternal);
            Assert.IsTrue(tweenOrSequenceRoot.tween.data.canManipulate()); // HasCancellationToken always means we can manipulate this animation
            if (tweenOrSequenceRoot.tween.data.tweenType == TweenAnimation.TweenType.MainSequence) {
                {
                    new Sequence(tweenOrSequenceRoot.tween).Stop();
                }
            } else {
                tweenOrSequenceRoot.Stop();
            }
            Assert.IsFalse(tweenOrSequenceRoot.isAliveInternal);
        }

        /// <summary>This method is needed for async/await support. Don't use it directly.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TweenAwaiter GetAwaiter() {
            return new TweenAwaiter(this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This struct is needed for async/await support, you should not use it directly.")]
        public readonly struct TweenAwaiter : INotifyCompletion {
            readonly Tween wait;
            readonly CancellationToken cancellationToken;

            internal TweenAwaiter(Tween awaitedTween) {
                PrimeTweenManager.EnsureRunningOnMainThread();

                wait = default;
                cancellationToken = CancellationToken.None;

                if (awaitedTween.isReusableReferenceValid && awaitedTween.tween.data.HasCancellationToken) {
                    // Copy the cancellation token from the awaited animation, even if it's not alive (isReusableReferenceValid)
                    Assert.IsTrue(awaitedTween.tween.cancellationToken.CanBeCanceled);
                    cancellationToken = awaitedTween.tween.cancellationToken;
                }

                if (awaitedTween.isAliveInternal && awaitedTween.TryManipulate()) {
                    if (cancellationToken.IsCancellationRequested) {
                        StopTweenOrSequence(awaitedTween);
                    } else {
                        // Explicitly use UpdateType.Update instead of 'default' so that continuations always happen from PrimeTweenManager.Update.
                        // I could have mirrored awaitedTween.updateType here so that the continuation happen from the corresponding Unity Player loop, but that might be unexpected. Instead, the user can control this explicitly with Awaitable.FixedUpdateAsync, etc.
                        var updateType = UpdateType.Update;
                        var infiniteSettings = new TweenSettings<float>(0, 0, float.MaxValue, Ease.Linear, -1, updateType: updateType);
                        wait = animate(awaitedTween.tween, ref infiniteSettings, TweenAnimation.TweenType.TweenAwaiter);
                        if (!wait.isAliveInternal) {
                            return;
                        }
                        wait.tween.longParam = awaitedTween.id;
                    }
                }
            }

            public bool IsCompleted => !wait.isAliveInternal;

            public void OnCompleted([NotNull] Action continuation) {
                // try-catch is needed here because any exception that is thrown inside the OnCompleted will be silenced
                // probably because this try in UnitySynchronizationContext.cs has no exception handling:
                // https://github.com/Unity-Technologies/UnityCsReference/blob/dd0d959800a675836a77dbe188c7dd55abc7c512/Runtime/Export/Scripting/UnitySynchronizationContext.cs#L157
                try {
                    Assert.IsTrue(wait.isAliveInternal);
                    wait.tween.managedData.OnComplete(continuation, true);
                } catch (Exception e) {
                    Debug.LogException(e);
                    throw;
                }
            }

            internal static void UpdateTweenAwaiter(ref TweenData rt, ref UnmanagedTweenData d) {
                if (d.isAlive) {
                    var target = rt.target as ColdData;
                    Assert.IsNotNull(target);
                    if (rt.cold.longParam == target.id && target.hasData && target.data.isAlive) {
                        ref var targetData = ref target.data;
                        if (targetData.HasCancellationToken && target.cancellationToken.IsCancellationRequested) {
                            // See AwaitedFixedUpdateTweenIsCanceledWhileTimeScaleIsZero
                            StopTweenOrSequence(new Tween(target));
                            rt.ForceComplete(ref d); // invoke continuation
                        }
                    } else {
                        rt.ForceComplete(ref d); // invoke continuation
                    }
                }
            }

            public void GetResult() {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }

    public partial struct Sequence {
        #if UNITASK_INSTALLED
        public static implicit operator Cysharp.Threading.Tasks.UniTask(Sequence sequence) => sequence.ToUniTask();
        public async Cysharp.Threading.Tasks.UniTask ToUniTask() => await this;
        #endif

        /// <inheritdoc cref="Tween.SetCancellationToken"/>
        public Sequence SetCancellationToken(CancellationToken cancellationToken) {
            root.SetCancellationToken(cancellationToken);
            return this;
        }

        /// <summary>This method is needed for async/await support. Don't use it directly.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Tween.TweenAwaiter GetAwaiter() {
            return new Tween.TweenAwaiter(root);
        }
    }
}
