#if UNITY_EDITOR && TEST_FRAMEWORK_INSTALLED
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable PartialTypeWithSinglePart
using System;
using PrimeTween;
using UnityEngine;
using Assert = NUnit.Framework.Assert;

[ExecuteInEditMode]
public partial class EditModeTest : MonoBehaviour {
    [SerializeField] TweenSettings _settings = CreateSettings();
    static TweenSettings CreateSettings() {
        TestExpectExceptions();
        return default;
    }

    Tween tween = TestExpectExceptions();

    static Tween TestExpectExceptions() {
        // print("TestExpectExceptions()");
        ExpectException(() => Tween.GetTweensCount());
        ExpectException(() => Sequence.Create());
        ExpectException(() => new Sequence().ChainCallback(delegate {}));
        ExpectException(() => new Sequence().InsertCallback(0f, delegate {}));
        ExpectException(() => new Sequence().Group(new Tween()));
        ExpectException(() => new Sequence().Group(new Sequence()));
        ExpectException(() => new Sequence().Chain(new Tween()));
        ExpectException(() => new Sequence().Chain(new Sequence()));
        ExpectException(() => new Sequence().Insert(0f, new Tween()));
        ExpectException(() => new Sequence().Insert(0f, new Sequence()));

        ExpectException(() => Tween.StopAll());
        ExpectException(() => PrimeTweenConfig.SetTweensCapacity(PrimeTweenManager.Instance.currentPoolCapacity + 1));
        ExpectException(() => Tween.Delay(new object(), 1f, () => {}));
        ExpectException(() => Tween.Delay(new object(), 1f, _ => {}));
        ExpectException(() => Tween.Delay(1f, () => { }));
        ExpectException(() => Tween.Custom(0, 1, 1, delegate {}));
        return default;
    }

    static void ExpectException(Action action) {
        try {
            action();
        } catch (Exception e) {
            string message = e.Message;
            Assert.IsTrue(message.Contains("is not allowed to be called from a MonoBehaviour constructor") || message.Contains(Constants.nonMainThreadError), message);
            return;
        }
        throw new Exception("Expected exception, but was none.");
    }

    static Tween TestMaybeExceptions() {
        // print("TestMaybeExceptions()");
        MaybeCtorExceptions(() => PrimeTweenConfig.SetTweensCapacity(PrimeTweenManager.Instance.currentPoolCapacity + 1));
        MaybeCtorExceptions(() => PrimeTweenConfig.warnZeroDuration = false);

        MaybeCtorExceptions(() => PrimeTweenConfig.warnEndValueEqualsCurrent = false);
        MaybeCtorExceptions(() => PrimeTweenConfig.warnEndValueEqualsCurrent = true);

        MaybeCtorExceptions(() => Sequence.Create());

        MaybeCtorExceptions(() => Tween.Delay(new object(), 1f, () => {}));
        MaybeCtorExceptions(() => Tween.Delay(new object(), 1f, _ => {}));
        MaybeCtorExceptions(() => Tween.Delay(1f, () => { }));
        MaybeCtorExceptions(() => Tween.Custom(0, 1, 1, delegate {}));
        return default;
    }

    static void MaybeCtorExceptions(Action action) {
        try {
            action();
        } catch (Exception e) {
            string message = e.Message;
            Assert.IsTrue(message.Contains("is not allowed to be called from a MonoBehaviour constructor"), message);
        }
    }

    static Tween StartTween() => Tween.Custom(0f, 1f, 1f, delegate { });

    void Awake() => TestDependingOnMainThread(nameof(Awake));
    void OnValidate() => TestDependingOnMainThread(nameof(OnValidate));
    void Reset() => TestMaybeExceptions();
    void OnEnable() => TestDependingOnMainThread(nameof(OnEnable));
    void OnDisable() => TestDependingOnMainThread(nameof(OnDisable));
    void OnDestroy() => TestDependingOnMainThread(nameof(OnDestroy));

    static void TestDependingOnMainThread(string name) {
        bool isMainThread;
        try {
            PrimeTweenManager.EnsureRunningOnMainThread();
            isMainThread = true;
        } catch (Exception e) {
            Assert.AreEqual(e.Message, Constants.nonMainThreadError);
            isMainThread = false;
        }

        // print($"{name} isMainThread: {isMainThread}");
        if (isMainThread) {
            TestMaybeExceptions();
        } else {
            TestExpectExceptions();
        }
    }
}

/*[UnityEditor.InitializeOnLoad]
public partial class EditModeTest {
    static EditModeTest() => TestWithPossibleException();
    EditModeTest() => TestWithPossibleException();

    [RuntimeInitializeOnLoadMethod]
    static void runtimeInitOnLoad() => Test();
}*/
#endif