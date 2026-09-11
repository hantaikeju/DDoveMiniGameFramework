#if TEST_FRAMEWORK_INSTALLED
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;
using PrimeTween;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = NUnit.Framework.Assert;

public partial class Tests {
    const float longDuration = 100f;

    Tween CreateCancellableTween(CancellationToken cancellationToken) {
        var result = Tween.PositionX(transform, 10f, longDuration).SetCancellationToken(cancellationToken);
        return result;
    }

    static CancellationToken CreateCanceledToken() {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        return cts.Token;
    }

    static CancellationToken CreateToken() => new CancellationTokenSource().Token;

    static void ExpectModificationIsNotAllowedError() => LogAssert.Expect(LogType.Error, new Regex(Constants.modificationIsNotAllowedError));
    static void ExpectTokenAlreadyAppliedError() => LogAssert.Expect(LogType.Error, new Regex("was already called on this animation"));
    static void ExpectTokenInSequenceError() => LogAssert.Expect(LogType.Error, new Regex("Adding an animation with a CancellationToken"));

    static async void AwaitAndCatch(Tween tween, [NotNull] Action<Exception> onCompleted) {
        try {
            await tween;
        } catch (Exception e) {
            onCompleted(e);
            return;
        }
        onCompleted(null);
    }

    static async void AwaitAndCatch(Sequence sequence, [NotNull] Action<Exception> onCompleted) {
        try {
            await sequence;
        } catch (Exception e) {
            onCompleted(e);
            return;
        }
        onCompleted(null);
    }

    [UnityTest]
    public IEnumerator CancellationStopsTweenOnNextUpdate() {
        var cts = new CancellationTokenSource();
        bool isOnCompleteCalled = false;
        var t = Tween.PositionX(transform, 10f, longDuration)
            .OnComplete(() => isOnCompleteCalled = true)
            .SetCancellationToken(cts.Token);
        Assert.IsTrue(t.tween.data.HasCancellationToken);
        yield return null;
        Assert.IsTrue(t.isAlive);

        cts.Cancel();
        Assert.IsTrue(t.isAlive, "Cancel() doesn't stop the animation immediately");
        yield return null;
        Assert.IsFalse(t.isAlive, "the animation is stopped on the next update");
        Assert.IsFalse(isOnCompleteCalled, "cancellation stops the animation as if Stop() was called, so OnComplete() is not called");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator CancellationStopsPausedTween() {
        var cts = new CancellationTokenSource();
        var t = CreateCancellableTween(cts.Token);
        t.isPaused = true;
        yield return null;
        Assert.IsTrue(t.isAlive);

        cts.Cancel();
        yield return null;
        Assert.IsFalse(t.isAlive, "paused animations are canceled too");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator CancellationStopsPausedSequence() {
        var cts = new CancellationTokenSource();
        var child = Tween.PositionX(transform, 10f, longDuration);
        var s = Sequence.Create(child).SetCancellationToken(cts.Token);
        s.isPaused = true;
        yield return null;
        Assert.IsTrue(s.isAlive);

        cts.Cancel();
        yield return null;
        Assert.IsFalse(s.isAlive, "paused Sequences are canceled too");
        Assert.IsFalse(child.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void CancellationWithAlreadyCanceledTokenStopsTweenImmediately() {
        var t = CreateCancellableTween(CreateCanceledToken());
        Assert.IsFalse(t.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void CancellationWithAlreadyCanceledTokenStopsSequenceImmediately() {
        var child = Tween.PositionX(transform, 10f, longDuration);
        var s = Sequence.Create(child).SetCancellationToken(CreateCanceledToken());
        Assert.IsFalse(s.isAlive);
        Assert.IsFalse(child.isAlive, "the whole Sequence is stopped, not only its root");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator SequenceCancellationStopsAllChildren() {
        var cts = new CancellationTokenSource();
        var t1 = Tween.PositionX(transform, 10f, longDuration);
        var t2 = Tween.PositionY(transform, 10f, longDuration);
        var s = Sequence.Create(t1).Chain(t2).SetCancellationToken(cts.Token);
        yield return null;
        Assert.IsTrue(s.isAlive);

        cts.Cancel();
        Assert.IsTrue(s.isAlive, "Cancel() doesn't stop the Sequence immediately");
        yield return null;
        Assert.IsFalse(s.isAlive);
        Assert.IsFalse(t1.isAlive);
        Assert.IsFalse(t2.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void CancellationOnDeadAnimationLogsError() {
        var t = Tween.PositionX(transform, 10f, longDuration);
        t.Stop();
        expectIsDeadError();
        t.SetCancellationToken(CreateToken());
        Assert.IsFalse(t.tween.data.HasCancellationToken);
    }

    [UnityTest]
    public IEnumerator CancellationOnStartedAnimationLogsError() {
        var t = Tween.PositionX(transform, 10f, longDuration);
        yield return null;
        Assert.IsTrue(t.tween.data.ModificationIsNotAllowed, "the animation received its first update");

        ExpectModificationIsNotAllowedError();
        t.SetCancellationToken(CreateToken());
        Assert.IsFalse(t.tween.data.HasCancellationToken);

        var s = Sequence.Create(Tween.PositionY(transform, 10f, longDuration));
        yield return null;
        ExpectModificationIsNotAllowedError();
        s.SetCancellationToken(CreateToken());
        Assert.IsFalse(s.root.tween.data.HasCancellationToken);

        Tween.StopAll();
    }

    [Test]
    public void CancellationTwiceLogsError() {
        var t = CreateCancellableTween(CreateToken());
        ExpectTokenAlreadyAppliedError();
        t.SetCancellationToken(CreateToken());
        Assert.IsTrue(t.isAlive);
        t.Stop();
    }

    [Test]
    public void CancellationWithNonCancellableTokenIsIgnored() {
        var t = CreateCancellableTween(CancellationToken.None);
        Assert.IsTrue(t.isAlive);
        Assert.IsFalse(t.tween.data.HasCancellationToken, "a token that can't be canceled is not applied");

        // the non-cancellable token was ignored, so applying a real token is still allowed and doesn't log the 'already applied' error
        t.SetCancellationToken(CreateToken());
        Assert.IsTrue(t.tween.data.HasCancellationToken);
        t.Stop();
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void CancellationOnTweenInsideSequenceLogsError() {
        var t = Tween.PositionX(transform, 10f, longDuration);
        var s = Sequence.Create(t);
        expectCantManipulateTweenInsideSequence();
        t.SetCancellationToken(CreateToken());
        Assert.IsFalse(t.tween.data.HasCancellationToken);
        s.Stop();
    }

    [Test]
    public void CancellationOnNestedSequenceLogsError() {
        var nested = Sequence.Create(Tween.PositionX(transform, 10f, longDuration));
        var s = Sequence.Create(Tween.PositionY(transform, 10f, longDuration)).Chain(nested);
        expectCantManipulateTweenInsideSequence();
        nested.SetCancellationToken(CreateToken());
        Assert.IsFalse(nested.root.tween.data.HasCancellationToken);
        s.Stop();
    }

    [Test]
    public void AddingTweenCancellationTokenToSequenceIsNotAllowed() {
        var t = CreateCancellableTween(CreateToken());
        ExpectTokenInSequenceError();
        var s = Sequence.Create(t);
        Assert.IsTrue(t.isAlive);
        Assert.IsFalse(t.tween.data.isInSequence, "the tween was not added to the Sequence");
        s.Stop();
        Assert.IsTrue(t.isAlive, "the tween outlives the Sequence it was not added to");
        t.Stop();
    }

    [Test]
    public void AddingSequenceCancellationTokenToSequenceIsNotAllowed() {
        var inner = Sequence.Create(Tween.PositionX(transform, 10f, longDuration)).SetCancellationToken(CreateToken());
        var outer = Sequence.Create(Tween.PositionY(transform, 10f, longDuration));
        ExpectTokenInSequenceError();
        outer.Chain(inner);
        Assert.IsTrue(inner.root.tween.data.IsMainSequenceRoot(), "the Sequence was not nested");
        outer.Stop();
        Assert.IsTrue(inner.isAlive, "the inner Sequence outlives the Sequence it was not added to");
        inner.Stop();
    }

    [Test]
    public async Task AwaitCanceledTweenThrowsImmediately() {
        var t = CreateCancellableTween(CreateCanceledToken());
        Assert.IsFalse(t.isAlive);
        var frameStart = Time.frameCount;
        Exception caught = null;
        try {
            await t;
        } catch (Exception e) {
            caught = e;
        }
        Assert.IsInstanceOf<OperationCanceledException>(caught);
        Assert.AreEqual(frameStart, Time.frameCount, "the 'await' throws without postponing to the next frame");
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public async Task AwaitCanceledSequenceThrowsImmediately() {
        var child = Tween.PositionX(transform, 10f, longDuration);
        var s = Sequence.Create(child).SetCancellationToken(CreateCanceledToken());
        Assert.IsFalse(s.isAlive);
        Exception caught = null;
        try {
            await s;
        } catch (Exception e) {
            caught = e;
        }
        Assert.IsInstanceOf<OperationCanceledException>(caught);
        Assert.IsFalse(child.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator AwaitThrowsWhenTweenIsCanceledWhileAwaiting() {
        var cts = new CancellationTokenSource();
        var t = CreateCancellableTween(cts.Token);
        bool isCompleted = false;
        Exception caught = null;
        AwaitAndCatch(t, e => {
            isCompleted = true;
            caught = e;
        });
        yield return null;
        Assert.IsFalse(isCompleted);

        cts.Cancel();
        yield return null;
        Assert.IsTrue(isCompleted);
        Assert.IsInstanceOf<OperationCanceledException>(caught);
        Assert.IsFalse(t.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator AwaitThrowsWhenSequenceIsCanceledWhileAwaiting() {
        var cts = new CancellationTokenSource();
        var child = Tween.PositionX(transform, 0f, 10f, longDuration);
        var s = Sequence.Create(child).SetCancellationToken(cts.Token);
        bool isCompleted = false;
        Exception caught = null;
        AwaitAndCatch(s, e => {
            isCompleted = true;
            caught = e;
        });
        yield return null;
        Assert.IsFalse(isCompleted);

        cts.Cancel();
        yield return null;
        Assert.IsTrue(isCompleted);
        Assert.IsInstanceOf<OperationCanceledException>(caught);
        Assert.IsFalse(s.isAlive);
        Assert.IsFalse(child.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator AwaitThrowsOnlyAfterTheAnimationIsStopped() {
        var cts = new CancellationTokenSource();
        Tween t = default;
        // OnUpdate() is called after the animation's cancellation check, so the animation can't observe the canceled token in the current update.
        // The awaiter must not throw before the awaited animation is stopped.
        t = Tween.PositionX(transform, 0f, 10f, longDuration)
            .OnUpdate(this, delegate { cts.Cancel(); })
            .SetCancellationToken(cts.Token);
        bool isCompleted = false;
        Exception caught = null;
        bool isAliveWhenThrown = true;
        AwaitAndCatch(t, e => {
            isCompleted = true;
            caught = e;
            isAliveWhenThrown = t.isAlive;
        });
        yield return null; // the token is canceled from OnUpdate()
        yield return null;
        Assert.IsTrue(isCompleted);
        Assert.IsInstanceOf<OperationCanceledException>(caught);
        Assert.IsFalse(isAliveWhenThrown, "the 'await' throws only after the awaited animation is stopped");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator AwaitFromOwnOnUpdateWithCanceledTokenDoesntThrow() {
        var cts = new CancellationTokenSource();
        Tween t = default;
        bool isAwaited = false;
        bool isCompletedSynchronously = false;
        bool isAliveWhenAwaitCompleted = false;
        Exception caught = null;
        t = Tween.PositionX(transform, 0f, 10f, longDuration)
            .OnUpdate(this, delegate {
                if (isAwaited) {
                    return;
                }
                isAwaited = true;
                cts.Cancel(); // the token is canceled, but the animation is still alive because cancellation is applied on the next update
                expectRecursiveCallError(); // TweenAwaiter's constructor fails to manipulate the animation from its own OnUpdate()
                bool isCompleted = false;
                AwaitAndCatch(t, e => {
                    isCompleted = true;
                    caught = e;
                });
                isCompletedSynchronously = isCompleted;
                isAliveWhenAwaitCompleted = t.isAlive;
            })
            .SetCancellationToken(cts.Token);
        yield return null; // the animation is awaited from OnUpdate()
        Assert.IsTrue(isAwaited);
        Assert.IsTrue(isCompletedSynchronously, "the 'await' completes synchronously because the awaiter failed to create the 'wait' animation");
        Assert.IsNotNull(caught as OperationCanceledException, $"LIMITATION: the 'await' doesn't throw OperationCanceledException because the awaiter didn't copy the canceled token: {caught}");
        Assert.IsTrue(isAliveWhenAwaitCompleted, "the 'await' completed even before the awaited animation was stopped");

        yield return null;
        Assert.IsFalse(t.isAlive, "the canceled animation is stopped on the next PrimeTween update");
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public async Task AwaitDoesntThrowIfTokenIsNotCanceled() {
        var t = Tween.Delay(getDt() * 2f).SetCancellationToken(CreateToken());
        await t;
        Assert.IsFalse(t.isAlive);
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public async Task AwaitingTweenInsideSequenceCompletesImmediately() {
        var t = Tween.PositionX(transform, 10f, longDuration);
        var s = Sequence.Create(t);
        var frameStart = Time.frameCount;
        expectCantManipulateTweenInsideSequence();
        await t;
        Assert.AreEqual(frameStart, Time.frameCount);
        s.Stop();
    }

    [Test]
    public async Task AwaitingDeadAnimationDoesntCreateAwaiterAnimation() {
        var t = Tween.PositionX(transform, 10f, longDuration);
        t.Stop();
        var count = tweensCount;
        await t;
        Assert.AreEqual(count, tweensCount, "the awaiter animation is not created for an already completed animation");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator AwaiterUpdateType() {
        var t = Tween.PositionX(transform, new TweenSettings<float>(0f, 10f, new TweenSettings(longDuration, updateType: UpdateType.FixedUpdate)));
        Assert.AreEqual(_UpdateType.FixedUpdate, t.tween.data.updateType);
        bool isCompleted = false;
        AwaitAndCatch(t, _ => isCompleted = true);
        Assert.AreEqual(_UpdateType.Update, GetAwaiterUpdateType(t), "the awaiter animation always uses _UpdateType.Update");

        t.Stop();
        yield return new WaitForFixedUpdate();
        yield return null;
        Assert.IsTrue(isCompleted);
        LogAssert.NoUnexpectedReceived();
    }

    /// FixedUpdate doesn't tick while Time.timeScale == 0, so the awaited animation can't observe the canceled token in its own update phase.
    /// The awaiter animation (which always updates from PrimeTweenManager.Update) should stop the awaited animation and resume the 'await'.
    [UnityTest]
    public IEnumerator AwaitedFixedUpdateTweenIsCanceledWhileTimeScaleIsZero() {
        var cts = new CancellationTokenSource();
        var t = Tween.PositionX(transform, new TweenSettings<float>(0f, 10f, new TweenSettings(longDuration, updateType: UpdateType.FixedUpdate)))
            .SetCancellationToken(cts.Token);
        bool isCompleted = false;
        Exception caught = null;
        AwaitAndCatch(t, e => {
            isCompleted = true;
            caught = e;
        });
        Time.timeScale = 0f;
        try {
            yield return null;
            Assert.IsTrue(t.isAlive);
            Assert.IsFalse(isCompleted);

            cts.Cancel();
            yield return null;
            Assert.IsFalse(t.isAlive, "the awaiter stops the awaited animation from the Update phase even though FixedUpdate doesn't tick");
            Assert.IsTrue(isCompleted);
            Assert.IsInstanceOf<OperationCanceledException>(caught);
        } finally {
            Time.timeScale = 1f;
        }
        LogAssert.NoUnexpectedReceived();
    }

    /// An un-awaited FixedUpdate animation should have its cancellation processed even while Time.timeScale == 0, when its own FixedUpdate phase is stalled.
    /// phase to stop it, so the canceled token is only observed from the animation's own FixedUpdate phase and the cancellation is delayed until FixedUpdate resumes.
    [UnityTest]
    public IEnumerator UnawaitedFixedUpdateTweenCancellationIsProcessedWhileTimeScaleIsZero() {
        var cts = new CancellationTokenSource();
        var t = Tween.PositionX(transform, new TweenSettings<float>(0f, 10f, new TweenSettings(longDuration, updateType: UpdateType.FixedUpdate)))
            .SetCancellationToken(cts.Token);
        try {
            Time.timeScale = 0f;
            yield return null;
            Assert.IsTrue(t.isAlive);

            cts.Cancel();
            // Update still ticks every frame at timeScale 0 (WaitForFixedUpdate would hang while FixedUpdate is stalled, so it's deliberately not used).
            // The cancellation should be observed without waiting for FixedUpdate to resume.
            yield return null;
            yield return null;
            Assert.IsFalse(t.isAlive, "cancellation of an un-awaited FixedUpdate animation should be processed while Time.timeScale == 0, without waiting for FixedUpdate to resume");
        } finally {
            Time.timeScale = 1f;
            if (t.isAlive) {
                t.Stop();
            }
        }
        LogAssert.NoUnexpectedReceived();
    }

    static _UpdateType GetAwaiterUpdateType(Tween awaitedTween) {
        foreach (var tweens in PrimeTweenManager.Instance.allTweenArrays) {
            foreach (var el in tweens) {
                var cold = el.tween.cold;
                if (cold != null && el.data.tweenType == TweenAnimation.TweenType.TweenAwaiter && cold.longParam == awaitedTween.id) {
                    return el.data.updateType;
                }
            }
        }
        Assert.Fail("awaiter animation is not found");
        return default;
    }

    [UnityTest]
    public IEnumerator ReleasedAnimationDoesntHoldCancellationToken() {
        var cts = new CancellationTokenSource();
        var t = CreateCancellableTween(cts.Token);
        var cold = t.tween;
        Assert.IsTrue(cold.cancellationToken.CanBeCanceled);

        cts.Cancel();
        yield return null;
        Assert.IsFalse(t.isAlive);
        yield return null;
        Assert.IsTrue(PrimeTweenManager.Instance.pool.Contains(cold), "the canceled animation is released to the pool");
        Assert.AreEqual(CancellationToken.None, cold.cancellationToken, "Reset() clears the token so the pooled animation doesn't hold a reference to the CancellationTokenSource");

        var t2 = Tween.PositionX(transform, 10f, longDuration);
        Assert.IsFalse(t2.tween.data.HasCancellationToken, "the animation reusing the pooled data doesn't inherit the canceled token");
        yield return null;
        Assert.IsTrue(t2.isAlive);
        t2.Stop();
        LogAssert.NoUnexpectedReceived();
    }

    /// Documented limitation of <see cref="Tween.SetCancellationToken"/>: the token lives on the animation and is cleared when the animation is recycled,
    /// so an animation that was canceled in one of the previous frames can no longer report the cancellation to the 'await'.
    [UnityTest]
    public IEnumerator AwaitingRecycledCanceledAnimationDoesntThrow() {
        var cts = new CancellationTokenSource();
        var t = CreateCancellableTween(cts.Token);
        var cold = t.tween;

        cts.Cancel();
        yield return null;
        Assert.IsFalse(t.isAlive);
        yield return null;
        Assert.IsTrue(PrimeTweenManager.Instance.pool.Contains(cold), "the canceled animation is recycled, so the token is no longer reachable from the Tween struct");

        bool isCompleted = false;
        Exception caught = null;
        AwaitAndCatch(t, e => {
            isCompleted = true;
            caught = e;
        });
        Assert.IsTrue(isCompleted, "the 'await' completes synchronously because the awaited animation is dead");
        Assert.IsNull(caught, "the recycled animation doesn't know about the token anymore, so the 'await' completes instead of throwing OperationCanceledException");
        LogAssert.NoUnexpectedReceived();
    }

    /// Documented limitation of <see cref="Tween.SetCancellationToken"/>: the 'await' throws if the token is canceled before the 'await' resumes,
    /// even if the animation itself completed successfully. Reachable only by completing/stopping the animation manually and canceling the token before the next PrimeTween update.
    [UnityTest]
    public IEnumerator AwaitThrowsIfTokenIsCanceledAfterAnimationCompletedManually() {
        var cts = new CancellationTokenSource();
        bool isOnCompleteCalled = false;
        var t = Tween.PositionX(transform, 10f, longDuration)
            .OnComplete(() => isOnCompleteCalled = true)
            .SetCancellationToken(cts.Token);
        bool isCompleted = false;
        Exception caught = null;
        AwaitAndCatch(t, e => {
            isCompleted = true;
            caught = e;
        });
        yield return null;
        Assert.IsFalse(isCompleted);

        t.Complete();
        Assert.IsTrue(isOnCompleteCalled, "the animation completed successfully, it was not canceled");
        Assert.IsFalse(t.isAlive);
        Assert.IsFalse(isCompleted, "the awaiter observes the completion of the awaited animation only on the next PrimeTween update");

        cts.Cancel(); // canceled after the animation has already completed, but before the 'await' resumed
        yield return null;
        Assert.IsTrue(isCompleted);
        Assert.IsInstanceOf<OperationCanceledException>(caught, "OperationCanceledException means 'the token was canceled by the time the await resumed', not 'the animation was interrupted mid-way'");
        LogAssert.NoUnexpectedReceived();
    }

    /// Covers the <see cref="Tween.TweenAwaiter"/> constructor path where the awaited animation is already dead (but not recycled yet) and the token is canceled:
    /// the token is still copied from the dead animation, so the 'await' throws OperationCanceledException immediately.
    [Test]
    public async Task AwaitAfterManualCompleteAndCancelThrowsImmediately() {
        var cts = new CancellationTokenSource();
        bool isOnCompleteCalled = false;
        var t = Tween.PositionX(transform, 10f, longDuration)
            .OnComplete(() => isOnCompleteCalled = true)
            .SetCancellationToken(cts.Token);
        t.Complete();
        Assert.IsTrue(isOnCompleteCalled, "the animation completed successfully, it was not canceled");
        Assert.IsFalse(t.isAlive);

        cts.Cancel(); // canceled after the animation has already completed, in the same frame, before the dead animation is recycled
        var frameStart = Time.frameCount;
        var count = tweensCount;
        Exception caught = null;
        try {
            await t;
        } catch (Exception e) {
            caught = e;
        }
        Assert.IsInstanceOf<OperationCanceledException>(caught, "the dead animation still holds the token until it's recycled, so the 'await' observes the cancellation");
        Assert.AreEqual(frameStart, Time.frameCount, "the 'await' throws without postponing to the next frame");
        Assert.AreEqual(count, tweensCount, "the awaiter animation is not created for a dead animation");
        LogAssert.NoUnexpectedReceived();
    }

    /// Same as <see cref="AwaitAfterManualCompleteAndCancelThrowsImmediately"/>, but the animation is stopped manually instead of completed.
    [Test]
    public async Task AwaitAfterManualStopAndCancelThrowsImmediately() {
        var cts = new CancellationTokenSource();
        var t = CreateCancellableTween(cts.Token);
        t.Stop();
        Assert.IsFalse(t.isAlive);

        cts.Cancel();
        Exception caught = null;
        try {
            await t;
        } catch (Exception e) {
            caught = e;
        }
        Assert.IsInstanceOf<OperationCanceledException>(caught);
        LogAssert.NoUnexpectedReceived();
    }

    /// Repro for a crash in <see cref="Tween.TweenAwaiter.UpdateTweenAwaiter"/>: it takes 'ref target.data' BEFORE checking 'rt.cold.longParam == target.id'.
    /// When the awaited animation's pooled ColdData is fetched for an animation whose creation FAILS (destroyed target), AddTween() calls TweenArray.RemoveLast(),
    /// which detaches the ColdData from its TweenArray ('_tweenArray = null'). On the next update, the awaiter dereferences 'target.data' of the detached ColdData
    /// and fails the 'hasData' assertion (NullReferenceException in release builds) instead of resuming the 'await'.
    [UnityTest]
    public IEnumerator AwaitResumesWhenColdDataOfCanceledAnimationIsDetachedByFailedAnimationCreation() {
        var cts = new CancellationTokenSource();
        var destroyedTarget = new GameObject().transform;
        UnityEngine.Object.DestroyImmediate(destroyedTarget.gameObject);

        // The awaited animation is created FIRST, so its cancellation is processed (and its ColdData is released to the pool) before 'helper' updates.
        var t = CreateCancellableTween(cts.Token);
        var cold = t.tween;
        // 'helper' updates after 't' and before the awaiter animation (which is created last, in GetAwaiter()).
        // On the frame the cancellation is processed, it immediately re-fetches 't's freshly pooled ColdData for an animation that fails to be created.
        bool fetchPooledColdData = false;
        var helper = Tween.PositionY(transform, 0f, 10f, longDuration)
            .OnUpdate(this, delegate {
                if (fetchPooledColdData) {
                    fetchPooledColdData = false;
                    expectTargetIsNull();
                    Tween.PositionX(destroyedTarget, 10f, longDuration); // fetches 't's ColdData from the pool (LIFO), fails, and detaches the ColdData from its TweenArray
                }
            });
        bool isCompleted = false;
        Exception caught = null;
        AwaitAndCatch(t, e => {
            isCompleted = true;
            caught = e;
        });
        yield return null;
        Assert.IsTrue(t.isAlive);
        Assert.IsFalse(isCompleted);

        cts.Cancel();
        fetchPooledColdData = true;
        // Update order within the next frame:
        // 1. 't' observes the canceled token, is stopped, and is released to the pool.
        // 2. 'helper's OnUpdate() re-fetches 't's ColdData for the failed animation, detaching the ColdData from its TweenArray.
        // 3. The awaiter animation updates: it must resume the 'await' instead of dereferencing the detached ColdData's 'data'.
        yield return null;
        Assert.IsFalse(t.isAlive);
        Assert.IsFalse(fetchPooledColdData, "the failed animation was created from 'helper's OnUpdate()");
        Assert.IsFalse(cold.hasData, "'t's ColdData was detached from its TweenArray by the failed animation creation");
        Assert.IsTrue(isCompleted, "the 'await' resumes even though the awaited animation's ColdData is detached");
        Assert.IsInstanceOf<OperationCanceledException>(caught);

        Tween.StopAll();
        LogAssert.NoUnexpectedReceived();
    }

}
#endif
