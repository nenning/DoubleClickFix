using DoubleClickFix.Tests.Helper;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix.Tests;

public class MouseHookTests {
	private const int WM_MOUSEMOVE = 0x0200;
	private const int WM_MOUSEWHEEL = 0x020A;
	private const uint WheelDown = 0xFF880000; // delta = -120
	private const uint WheelUp = 0x00780000;   // delta = +120

	private static void AssertAllowed(MouseHook hook, nint wmMouseEvent, uint timeMs, int movedPixels = 0, uint mouseData = 0) => AssertMouseEvent(hook, wmMouseEvent, timeMs, true, movedPixels, mouseData);
	private static void AssertIgnored(MouseHook hook, nint wmMouseEvent, uint timeMs, int movedPixels = 0, uint mouseData = 0) => AssertMouseEvent(hook, wmMouseEvent, timeMs, false, movedPixels, mouseData);
	private static void AssertMouseEvent(MouseHook hook, nint wmMouseEvent, uint timeMs, bool allowed, int movedPixels = 0, uint mouseData = 0) {
		using HookStruct data = HookStruct.Create(timeMs, movedPixels, 0, mouseData);
		Assert.Equal(allowed ? 0 : 1, hook.HookCallback(0, wmMouseEvent, data.Pointer));
	}

	[Fact]
	public void TestLeftClickIgnored() {
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		AssertAllowed(hook, WM_LBUTTONDOWN, 100);
		AssertAllowed(hook, WM_LBUTTONUP, 110);
		AssertIgnored(hook, WM_LBUTTONDOWN, 120);
		AssertIgnored(hook, WM_LBUTTONUP, 130);
		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestX1ClickIgnored() {
		TestNativeMethods nativeMethods = new();
		TestSettings settings = new() {
			X1Threshold = 20
		};
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		AssertAllowed(hook, WM_XBUTTONDOWN, 100, mouseData: 0x00010000);
		AssertAllowed(hook, WM_XBUTTONUP, 110, mouseData: 0x00010000);
		AssertIgnored(hook, WM_XBUTTONDOWN, 120, mouseData: 0x00010000);
		AssertIgnored(hook, WM_XBUTTONUP, 130, mouseData: 0x00010000);
		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestLeftClickAllowed() {
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		AssertAllowed(hook, WM_LBUTTONDOWN, 100);
		AssertAllowed(hook, WM_LBUTTONUP, 110);
		AssertAllowed(hook, WM_LBUTTONDOWN, 170);
		AssertAllowed(hook, WM_LBUTTONUP, 180);
		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestMultipleClicks() {
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		AssertAllowed(hook, WM_LBUTTONDOWN, 100);
		AssertAllowed(hook, WM_LBUTTONUP, 110);
		AssertAllowed(hook, WM_LBUTTONDOWN, 170);
		AssertAllowed(hook, WM_LBUTTONUP, 180);
		AssertIgnored(hook, WM_LBUTTONDOWN, 190);
		AssertIgnored(hook, WM_LBUTTONUP, 200);
		Assert.Equal(4, nativeMethods.CallNextHookCounter);

		AssertAllowed(hook, WM_LBUTTONDOWN, 300);
		AssertAllowed(hook, WM_LBUTTONUP, 310);
		AssertIgnored(hook, WM_LBUTTONDOWN, 320);
		AssertIgnored(hook, WM_LBUTTONUP, 330);
		Assert.Equal(6, nativeMethods.CallNextHookCounter);

		AssertAllowed(hook, WM_LBUTTONDOWN, 400);
		AssertAllowed(hook, WM_LBUTTONUP, 410);
		AssertIgnored(hook, WM_LBUTTONDOWN, 420);
		AssertIgnored(hook, WM_LBUTTONUP, 430);
		Assert.Equal(8, nativeMethods.CallNextHookCounter);

		AssertAllowed(hook, WM_LBUTTONDOWN, 500);
		AssertAllowed(hook, WM_LBUTTONUP, 510);
		AssertAllowed(hook, WM_LBUTTONDOWN, 570);
		AssertAllowed(hook, WM_LBUTTONUP, 580);
		Assert.Equal(12, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestBasicClicksClickIgnored() {
		MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_LBUTTONUP, 100);
		AssertIgnored(hook, WM_LBUTTONDOWN, 110);

		AssertIgnored(hook, WM_LBUTTONUP, 100);
		AssertAllowed(hook, WM_LBUTTONDOWN, 160);

		AssertAllowed(hook, WM_RBUTTONUP, 200);
		AssertAllowed(hook, WM_RBUTTONDOWN, 210);

		AssertAllowed(hook, WM_MBUTTONUP, 300);
		AssertAllowed(hook, WM_MBUTTONDOWN, 310);
	}


	[Fact]
	public void TestInterleavedClicks() {
		MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_LBUTTONUP, 100);
		AssertAllowed(hook, WM_RBUTTONUP, 101);
		AssertIgnored(hook, WM_LBUTTONDOWN, 120);
		AssertAllowed(hook, WM_RBUTTONDOWN, 121);

		AssertIgnored(hook, WM_LBUTTONUP, 200);
		AssertAllowed(hook, WM_RBUTTONUP, 201);
		AssertAllowed(hook, WM_LBUTTONDOWN, 260);
		AssertAllowed(hook, WM_RBUTTONDOWN, 261);
	}

	[Fact]
	public void TestSettingsChanged() {
		TestSettings settings = new();
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());
		settings.LeftThreshold = -1;
		settings.RightThreshold = 10;
		settings.FireSettingsChanged();

		AssertAllowed(hook, WM_LBUTTONUP, 100);
		AssertAllowed(hook, WM_LBUTTONDOWN, 101);

		AssertAllowed(hook, WM_RBUTTONUP, 102);
		AssertIgnored(hook, WM_RBUTTONDOWN, 110);

		AssertIgnored(hook, WM_RBUTTONUP, 111);
		AssertAllowed(hook, WM_RBUTTONDOWN, 130);

	}

	[Theory]
	[InlineData(-1, -1, -1, -1, -1, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, true)] // everything disabled
	[InlineData(-1, -1, -1, -1, -1, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 30, true)]
	[InlineData(50, 50, 50, 50, 50, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, false)] // everything enabled
	[InlineData(50, 50, 50, 50, 50, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 50, true)]
	[InlineData(20, -1, -1, -1, -1, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, false)] // delay = 0 -> ignored
	[InlineData(-1, 20, -1, -1, -1, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 0, false)]
	[InlineData(-1, -1, 20, -1, -1, -1, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 0, false)]
	[InlineData(20, -1, -1, -1, -1, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 19, false)] // delay < timeout -> ignored
	[InlineData(-1, 20, -1, -1, -1, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 19, false)]
	[InlineData(-1, -1, 20, -1, -1, -1, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 19, false)]
	[InlineData(20, -1, -1, -1, -1, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 20, true)] // delay == timeout -> allowed
	[InlineData(-1, 20, -1, -1, -1, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 20, true)]
	[InlineData(-1, -1, 20, -1, -1, -1, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 20, true)]
	[InlineData(20, -1, -1, -1, -1, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 30, true)] // delay >= timeout -> not allowed
	[InlineData(-1, 20, -1, -1, -1, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 30, true)]
	[InlineData(-1, -1, 20, -1, -1, -1, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 30, true)]
	[InlineData(50, 50, 50, 50, 50, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, true)] // minDelay set to 0
	[InlineData(50, 50, 50, 50, 50, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 0, true)]
	[InlineData(50, 50, 50, 50, 50, 0, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 0, true)]

	public void TestSettingsCombinations(int lTimeout, int rTimeout, int mTimeout, int x1Timeout, int x2Timeout, int minDelay, int up, int down, uint ms, bool allowed) {
		TestSettings settings = new();
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());
		settings.LeftThreshold = lTimeout;
		settings.RightThreshold = rTimeout;
		settings.MiddleThreshold = mTimeout;
		settings.X1Threshold = x1Timeout;
		settings.X2Threshold = x2Timeout;
		settings.MinDelay = minDelay;
		settings.FireSettingsChanged();

		AssertAllowed(hook, up, 100);
		AssertMouseEvent(hook, down, 100 + ms, allowed);
	}

	[Fact]
	public void TestSwitchDevice() {
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);
		hook.ProcessRawInput(123);
		Assert.Equal(1, nativeMethods.ProcessRawInputCounter);
	}

	[Fact]
	public void TestSwitchDeviceCachesDeviceType() {
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);
		hook.ProcessRawInput(123);
		hook.ProcessRawInput(123); // same device: TryProcessRawInput should not be called again
		Assert.Equal(1, nativeMethods.ProcessRawInputCounter);
	}

	[Fact]
	public void TestIgnoredDeviceByPathPassesThrough() {
		const string devicePath = "/dev/test_mouse";
		TestNativeMethods nativeMethods = new TestNativeMethods {
			TryGetDevicePathFunc = _ => devicePath
		};
		TestSettings settings = new();
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		// Learn the device, then mark it as ignored
		hook.ProcessRawInput(123);
		settings.AddIgnoredDevice(devicePath);

		// Rapid clicks from the ignored device should all pass through unfiltered
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestNonIgnoredDeviceStillFiltered() {
		const string ignoredPath = "/dev/ignored_mouse";
		const string activePath = "/dev/active_mouse";
		TestNativeMethods nativeMethods = new TestNativeMethods {
			TryGetDevicePathFunc = handle => handle == 123 ? ignoredPath : activePath
		};
		TestSettings settings = new();
		settings.AddIgnoredDevice(ignoredPath);
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		// Learn device 456 (not ignored)
		hook.ProcessRawInput(456);

		// Rapid clicks from non-ignored device should still be filtered normally
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer)); // filtered
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));   // orphaned UP filtered
		}

		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestCurrentDevicePathSetOnFirstDevice() {
		const string devicePath = "/dev/test_mouse";
		TestNativeMethods nativeMethods = new TestNativeMethods { TryGetDevicePathFunc = _ => devicePath };
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		hook.ProcessRawInput(123);

		Assert.Equal(devicePath, hook.CurrentDevicePath);
	}

	[Fact]
	public void TestCurrentDevicePathRetainedWhenNewDeviceHasNoPath() {
		const string devicePath = "/dev/test_mouse";
		TestNativeMethods nativeMethods = new TestNativeMethods {
			TryGetDevicePathFunc = handle => handle == 123 ? devicePath : null
		};
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		hook.ProcessRawInput(123); // device with valid path
		hook.ProcessRawInput(456); // device with no path — should keep previous path

		Assert.Equal(devicePath, hook.CurrentDevicePath);
	}

	[Fact]
	public void TestCurrentDevicePathUpdatedWhenNewDeviceHasPath() {
		const string pathA = "/dev/mouse_a";
		const string pathB = "/dev/mouse_b";
		TestNativeMethods nativeMethods = new TestNativeMethods {
			TryGetDevicePathFunc = handle => handle == 123 ? pathA : pathB
		};
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		hook.ProcessRawInput(123);
		hook.ProcessRawInput(456);

		Assert.Equal(pathB, hook.CurrentDevicePath);
	}

	[Fact]
	public void TestDragLockEnabled() {
		TestSettings settings = new() {
			DragStartTimeMilliseconds = 100,
			DragStopTimeMilliseconds = 200
		};
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		// Simulate a drag event
		AssertAllowed(hook, WM_LBUTTONDOWN, 200); // Initial press for drag-lock
		AssertAllowed(hook, WM_MOUSEMOVE, 450, movedPixels: 20);  // Movement starts drag-lock
		AssertIgnored(hook, WM_LBUTTONUP, 550);  // Drag-lock active, suppress release
		AssertIgnored(hook, WM_LBUTTONDOWN, 551);  // Drag-lock active, suppress press
		AssertAllowed(hook, WM_LBUTTONUP, 800); // Drag-lock ends, allow release
	}

	[Fact]
	public void TestDragLockNotInitiated() {
		TestSettings settings = new() {
			DragStartTimeMilliseconds = 100,
			DragStopTimeMilliseconds = 200
		};
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		// Simulate a drag event
		AssertAllowed(hook, WM_LBUTTONDOWN, 200); // Initial press
		AssertAllowed(hook, WM_MOUSEMOVE, 450, movedPixels: 1);  // Not enough movement, drag-lock not initiated
		AssertAllowed(hook, WM_LBUTTONUP, 550);  // Genuine release
		AssertIgnored(hook, WM_LBUTTONDOWN, 551);  // Bounce: too soon after UP, suppressed
		AssertIgnored(hook, WM_LBUTTONUP, 800); // Orphaned UP for suppressed DOWN, also suppressed
	}

	[Fact]
	public void TestWheelBounceSingleIgnored() {
		TestSettings settings = new() { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_MOUSEWHEEL, 1, mouseData: WheelDown);
		AssertIgnored(hook, WM_MOUSEWHEEL, 50, mouseData: WheelUp); // bounce within threshold
	}

	[Fact]
	public void TestWheelBounceMultipleIgnored() {
		TestSettings settings = new() { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_MOUSEWHEEL, 1, mouseData: WheelDown);
		AssertIgnored(hook, WM_MOUSEWHEEL, 50, mouseData: WheelUp);  // bounce #1
		AssertIgnored(hook, WM_MOUSEWHEEL, 100, mouseData: WheelUp); // bounce #2 — previously passed through
		AssertAllowed(hook, WM_MOUSEWHEEL, 301, mouseData: WheelDown); // genuine scroll after threshold
	}

	[Fact]
	public void TestWheelBounceLegitimateReversal() {
		TestSettings settings = new() { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_MOUSEWHEEL, 1, mouseData: WheelDown);
		AssertIgnored(hook, WM_MOUSEWHEEL, 50, mouseData: WheelUp);  // bounce within threshold
		AssertAllowed(hook, WM_MOUSEWHEEL, 400, mouseData: WheelUp); // legitimate reversal after threshold
	}

	[Fact]
	public void TestRdpInjectedClickPassesThroughWhenDetectionEnabled() {
		const uint LLMHF_INJECTED = 0x1;
		TestNativeMethods nativeMethods = new TestNativeMethods { IsRemoteSessionResult = true };
		TestSettings settings = new TestSettings { IsRemoteDesktopDetectionEnabled = true };
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		// Two rapid clicks (10 ms apart, within the 50 ms threshold) with injected flag:
		// both should pass through without filtering.
		using (HookStruct data = HookStruct.Create(100, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestRdpInjectedClickStillFilteredWhenDetectionDisabled() {
		const uint LLMHF_INJECTED = 0x1;
		TestNativeMethods nativeMethods = new TestNativeMethods { IsRemoteSessionResult = true };
		// Detection is disabled by default — filtering should still apply.
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		using (HookStruct data = HookStruct.Create(100, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120, flags: LLMHF_INJECTED)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer)); // filtered
		}

		using (HookStruct data = HookStruct.Create(130, flags: LLMHF_INJECTED)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));   // orphaned UP filtered
		}

		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestTouchpadInjectedClickPassesThroughLocalSession() {
		const uint LLMHF_INJECTED = 0x1;
		TestNativeMethods nativeMethods = new TestNativeMethods { IsRemoteSessionResult = false };
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		// Rapid clicks (10 ms apart, within the 50 ms threshold) with injected flag on local machine:
		// should all pass through — these are touchpad tap-to-click events.
		using (HookStruct data = HookStruct.Create(100, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130, flags: LLMHF_INJECTED)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestTouchpadInjectedWheelPassesThroughLocalSession() {
		const uint LLMHF_INJECTED = 0x1;
		TestNativeMethods nativeMethods = new TestNativeMethods { IsRemoteSessionResult = false };
		TestSettings settings = new TestSettings { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		// Two-finger scroll: rapid direction reversal (normally filtered by wheel bounce logic)
		// but injected on a local machine — should pass through.
		using (HookStruct data = HookStruct.Create(1, flags: LLMHF_INJECTED, mouseData: WheelDown)) {
			Assert.Equal(0, hook.HookCallback(0, WM_MOUSEWHEEL, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(50, flags: LLMHF_INJECTED, mouseData: WheelUp)) {
			Assert.Equal(0, hook.HookCallback(0, WM_MOUSEWHEEL, data.Pointer));
		}

		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestNonInjectedClickStillFilteredInLocalSession() {
		TestNativeMethods nativeMethods = new TestNativeMethods { IsRemoteSessionResult = false };
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		// flags = 0 (not injected, physical mouse): normal filtering still applies.
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer)); // filtered
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));   // orphaned UP filtered
		}

		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestNonInjectedClickStillFilteredInRemoteSession() {
		TestNativeMethods nativeMethods = new TestNativeMethods { IsRemoteSessionResult = true };
		TestSettings settings = new TestSettings { IsRemoteDesktopDetectionEnabled = true };
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		// flags = 0 (not injected): bypass does not apply even in a remote session.
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer)); // filtered
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));   // orphaned UP filtered
		}

		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestX2ClickIgnored() {
		TestNativeMethods nativeMethods = new();
		TestSettings settings = new TestSettings { X2Threshold = 20 };
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		AssertAllowed(hook, WM_XBUTTONDOWN, 100, mouseData: 0x00020000);
		AssertAllowed(hook, WM_XBUTTONUP, 110, mouseData: 0x00020000);
		AssertIgnored(hook, WM_XBUTTONDOWN, 120, mouseData: 0x00020000);
		AssertIgnored(hook, WM_XBUTTONUP, 130, mouseData: 0x00020000);
		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestTouchDevicePassesThroughUnfiltered() {
		TestNativeMethods nativeMethods = new TestNativeMethods {
			ProcessRawInputFunc = _ => (true, 0, true) // DeviceType.TouchPad
		};
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);
		hook.ProcessRawInput(123); // classify device as touch

		// Rapid clicks within 50 ms threshold — all should pass through unfiltered
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestHorizontalWheelBounceSingleIgnored() {
		TestSettings settings = new TestSettings { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_MOUSEHWHEEL, 1, mouseData: WheelDown); // scroll right
		AssertIgnored(hook, WM_MOUSEHWHEEL, 50, mouseData: WheelUp);   // bounce left within threshold
	}

	[Fact]
	public void TestHorizontalWheelBounceLegitimateReversal() {
		TestSettings settings = new TestSettings { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_MOUSEHWHEEL, 1, mouseData: WheelDown);
		AssertIgnored(hook, WM_MOUSEHWHEEL, 50, mouseData: WheelUp);  // bounce within threshold
		AssertAllowed(hook, WM_MOUSEHWHEEL, 400, mouseData: WheelUp);  // legitimate reversal after threshold
	}

	[Fact]
	public void TestTimerWrapAroundFilteredAfterWrap() {
		MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());

		// UP near uint.MaxValue, then DOWN shortly after rollover — elapsed ~11 ms, within 50 ms threshold
		AssertAllowed(hook, WM_LBUTTONDOWN, uint.MaxValue - 20);
		AssertAllowed(hook, WM_LBUTTONUP, uint.MaxValue - 5);
		AssertIgnored(hook, WM_LBUTTONDOWN, 5);   // 11 ms after UP (via wrap) — filtered
		AssertIgnored(hook, WM_LBUTTONUP, 15);  // orphaned UP — suppressed
	}

	[Fact]
	public void TestTimerWrapAroundAllowedAfterWrap() {
		MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());

		// UP near uint.MaxValue, then DOWN well after rollover — elapsed ~66 ms, beyond 50 ms threshold
		AssertAllowed(hook, WM_LBUTTONDOWN, uint.MaxValue - 20);
		AssertAllowed(hook, WM_LBUTTONUP, uint.MaxValue - 5);
		AssertAllowed(hook, WM_LBUTTONDOWN, 60);  // 66 ms after UP (via wrap) — allowed
		AssertAllowed(hook, WM_LBUTTONUP, 70);
	}

	[Fact]
	public void TestNegativeNCodeAlwaysForwards() {
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		// Rapid clicks that would normally be filtered — but nCode = -1 bypasses all processing
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(-1, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(-1, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(0, hook.HookCallback(-1, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(0, hook.HookCallback(-1, WM_LBUTTONUP, data.Pointer));
		}

		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestRemovingIgnoredDeviceRestoresFiltering() {
		const string devicePath = "/dev/test_mouse";
		TestNativeMethods nativeMethods = new TestNativeMethods { TryGetDevicePathFunc = _ => devicePath };
		TestSettings settings = new();
		MouseHook hook = new(settings, new TestLogger(), nativeMethods);

		hook.ProcessRawInput(123);
		settings.AddIgnoredDevice(devicePath);

		// Passes through while ignored
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		settings.RemoveIgnoredDevice(devicePath); // un-ignore

		// Rapid clicks after un-ignore should be filtered again
		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(140)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer)); // filtered
		}

		using (HookStruct data = HookStruct.Create(150)) {
			Assert.Equal(1, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer)); // orphaned UP
		}
	}

	[Fact]
	public void TestDisablingDragCorrectionExitsDragLock() {
		TestSettings settings = new TestSettings {
			DragStartTimeMilliseconds = 100,
			DragStopTimeMilliseconds = 200
		};
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		// Enter drag-lock
		AssertAllowed(hook, WM_LBUTTONDOWN, 200);
		AssertAllowed(hook, WM_MOUSEMOVE, 450, movedPixels: 20); // triggers drag-lock
		AssertIgnored(hook, WM_LBUTTONUP, 550);                  // suppressed inside drag-lock

		// Disable drag correction mid-drag
		settings.DragStartTimeMilliseconds = -1;
		settings.FireSettingsChanged();

		// Release should now be forwarded (drag state was reset)
		AssertAllowed(hook, WM_LBUTTONUP, 560);
	}

	[Fact]
	public void TestWheelSameDirectionAlwaysAllowed() {
		TestSettings settings = new TestSettings { WheelThreshold = 200 };
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		// Rapid same-direction scrolling should never be filtered
		AssertAllowed(hook, WM_MOUSEWHEEL, 1, mouseData: WheelDown);
		AssertAllowed(hook, WM_MOUSEWHEEL, 10, mouseData: WheelDown);
		AssertAllowed(hook, WM_MOUSEWHEEL, 20, mouseData: WheelDown);
		AssertAllowed(hook, WM_MOUSEWHEEL, 30, mouseData: WheelDown);
	}

	[Fact]
	public void TestDragLockRequiresFivePixels() {
		TestSettings settings = new TestSettings {
			DragStartTimeMilliseconds = 100,
			DragStopTimeMilliseconds = 200
		};
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_LBUTTONDOWN, 200);
		AssertAllowed(hook, WM_MOUSEMOVE, 450, movedPixels: 3); // (3,3) → distSq=18 < 25 (5²threshold), no drag-lock
		AssertAllowed(hook, WM_LBUTTONUP, 550);                 // genuine release, drag-lock was not entered
	}

	[Fact]
	public void TestDragLockRequiresDragStartTime() {
		TestSettings settings = new TestSettings {
			DragStartTimeMilliseconds = 300,
			DragStopTimeMilliseconds = 200
		};
		MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

		AssertAllowed(hook, WM_LBUTTONDOWN, 200);
		AssertAllowed(hook, WM_MOUSEMOVE, 350, movedPixels: 20); // 150 ms since down — below 300 ms DragStartTime
		AssertAllowed(hook, WM_LBUTTONUP, 400);                  // released before drag-lock could engage
	}

	[Fact]
	public void TestSustainedChatteringAllSuppressed() {
		// Multiple rapid chatter DOWN/UP pairs after a genuine click should all be suppressed,
		// not just the first one. Regression test for bug where previousUpTime was reset to 0
		// on a suppressed DOWN, allowing subsequent chatter through.
		TestNativeMethods nativeMethods = new();
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		// Genuine click
		AssertAllowed(hook, WM_LBUTTONDOWN, 100);
		AssertAllowed(hook, WM_LBUTTONUP, 110);

		// First chatter pair (1ms after genuine UP — well within 50ms threshold)
		AssertIgnored(hook, WM_LBUTTONDOWN, 111);
		AssertIgnored(hook, WM_LBUTTONUP, 112);

		// Second chatter pair (3ms after genuine UP — still within threshold)
		AssertIgnored(hook, WM_LBUTTONDOWN, 113);
		AssertIgnored(hook, WM_LBUTTONUP, 114);

		// Third chatter pair (5ms after genuine UP — still within threshold)
		AssertIgnored(hook, WM_LBUTTONDOWN, 115);
		AssertIgnored(hook, WM_LBUTTONUP, 116);

		// Only the 2 genuine events should have passed through
		Assert.Equal(2, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void HookCallback_LoggerThrows_DoesNotCrash() {
		TestNativeMethods nativeMethods = new();
		// ThrowingLogger simulates a disposed logger throwing ObjectDisposedException
		MouseHook hook = new(new TestSettings(), new ThrowingLogger(), nativeMethods);

		// First click: allowed
		AssertAllowed(hook, WM_LBUTTONDOWN, 100);
		AssertAllowed(hook, WM_LBUTTONUP, 110);

		// Second click within threshold triggers suppression → logger.Log() → throws.
		// The hardened catch block swallows the exception and falls through to CallNextHook
		// (returning 0 = allowed) instead of crashing. The key assertion is no exception escapes.
		using HookStruct data = HookStruct.Create(120);
		var result = hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer);
		// Result is 0 (CallNextHook) because the catch block handled the exception gracefully
		Assert.Equal(0, result);
	}

	[Fact]
	public void TestDeviceZeroAlwaysIgnored() {
		TestNativeMethods nativeMethods = new TestNativeMethods {
			TryGetDevicePathFunc = _ => null
		};
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		// Device handle 0 (unidentifiable source) should pass through unfiltered
		hook.ProcessRawInput(0);

		// Rapid clicks that would normally be suppressed should all pass through
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		Assert.Equal(4, nativeMethods.CallNextHookCounter);
	}

	[Fact]
	public void TestDeviceZeroIgnoredThenRealDeviceFiltered() {
		const string devicePath = "/dev/real_mouse";
		TestNativeMethods nativeMethods = new TestNativeMethods {
			TryGetDevicePathFunc = handle => handle == 42 ? devicePath : null
		};
		MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

		// Device 0: passes through
		hook.ProcessRawInput(0);
		using (HookStruct data = HookStruct.Create(100)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(110)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(120)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONDOWN, data.Pointer));
		}

		using (HookStruct data = HookStruct.Create(130)) {
			Assert.Equal(0, hook.HookCallback(0, WM_LBUTTONUP, data.Pointer));
		}

		// Switch to real device: rapid second click should be filtered
		hook.ProcessRawInput(42);
		AssertAllowed(hook, WM_LBUTTONDOWN, 200);
		AssertAllowed(hook, WM_LBUTTONUP, 210);
		AssertIgnored(hook, WM_LBUTTONDOWN, 220);
	}
}
