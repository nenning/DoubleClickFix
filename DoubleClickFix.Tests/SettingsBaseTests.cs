using DoubleClickFix.Tests.Helper;

namespace DoubleClickFix.Tests;

public class SettingsBaseTests {
	// -------------------------------------------------------------------------
	// DragStartTimeMilliseconds clamping [-1, 2000]
	// -------------------------------------------------------------------------

	[Fact]
	public void TestDragStartTimeClampsToMax() {
		TestableSettings s = new(new TestLogger());
		s.DragStartTimeMilliseconds = 9999;
		Assert.Equal(2000, s.DragStartTimeMilliseconds);
	}

	[Fact]
	public void TestDragStartTimeClampsToMinusOne() {
		TestableSettings s = new(new TestLogger());
		s.DragStartTimeMilliseconds = -99;
		Assert.Equal(-1, s.DragStartTimeMilliseconds);
	}

	[Fact]
	public void TestDragStartTimeAcceptsValidRange() {
		TestableSettings s = new(new TestLogger());
		s.DragStartTimeMilliseconds = 0;
		Assert.Equal(0, s.DragStartTimeMilliseconds);
		s.DragStartTimeMilliseconds = 2000;
		Assert.Equal(2000, s.DragStartTimeMilliseconds);
	}

	// -------------------------------------------------------------------------
	// DragStopTimeMilliseconds clamping [-1, 500]
	// -------------------------------------------------------------------------

	[Fact]
	public void TestDragStopTimeClampsToMax() {
		TestableSettings s = new(new TestLogger());
		s.DragStopTimeMilliseconds = 9999;
		Assert.Equal(500, s.DragStopTimeMilliseconds);
	}

	[Fact]
	public void TestDragStopTimeClampsToMinusOne() {
		TestableSettings s = new(new TestLogger());
		s.DragStopTimeMilliseconds = -99;
		Assert.Equal(-1, s.DragStopTimeMilliseconds);
	}

	[Fact]
	public void TestDragStopTimeAcceptsValidRange() {
		TestableSettings s = new(new TestLogger());
		s.DragStopTimeMilliseconds = 0;
		Assert.Equal(0, s.DragStopTimeMilliseconds);
		s.DragStopTimeMilliseconds = 500;
		Assert.Equal(500, s.DragStopTimeMilliseconds);
	}

	// -------------------------------------------------------------------------
	// IsDragCorrectionEnabled
	// -------------------------------------------------------------------------

	[Theory]
	[InlineData(-1, -1, false)]
	[InlineData(0, -1, false)]
	[InlineData(-1, 0, false)]
	[InlineData(0, 0, true)]
	[InlineData(100, 200, true)]
	public void TestIsDragCorrectionEnabled(int start, int stop, bool expected) {
		TestableSettings s = new(new TestLogger());
		s.DragStartTimeMilliseconds = start;
		s.DragStopTimeMilliseconds = stop;
		Assert.Equal(expected, s.IsDragCorrectionEnabled);
	}

	// -------------------------------------------------------------------------
	// Change notification — only fires when value actually changes
	// -------------------------------------------------------------------------

	[Fact]
	public void TestSettingUnchangedValueDoesNotFireEvent() {
		TestableSettings s = new(new TestLogger());
		int count = 0;
		s.RegisterSettingsChangedListener(() => count++);

		s.LeftThreshold = s.LeftThreshold; // same value — no event
		Assert.Equal(0, count);

		s.LeftThreshold = s.LeftThreshold + 1; // different value — fires
		Assert.Equal(1, count);
	}

	[Fact]
	public void TestEachSettablePropertyFiresEventOnChange() {
		TestableSettings s = new(new TestLogger());
		int count = 0;
		s.RegisterSettingsChangedListener(() => count++);

		s.LeftThreshold = 99;
		s.RightThreshold = 99;
		s.MiddleThreshold = 99;
		s.X1Threshold = 99;
		s.X2Threshold = 99;
		s.WheelThreshold = 99;
		s.MinDelay = 99;
		s.DragStartTimeMilliseconds = 99;
		s.DragStopTimeMilliseconds = 99;
		s.IsRemoteDesktopDetectionEnabled = true;

		Assert.Equal(10, count);
	}

	// -------------------------------------------------------------------------
	// AddIgnoredDevice / RemoveIgnoredDevice — dedup and notification
	// -------------------------------------------------------------------------

	[Fact]
	public void TestAddIgnoredDeviceFiresOnlyForNewPath() {
		TestableSettings s = new(new TestLogger());
		int count = 0;
		s.RegisterSettingsChangedListener(() => count++);

		s.AddIgnoredDevice("/dev/mouse0");
		s.AddIgnoredDevice("/dev/mouse0"); // duplicate — no second event
		Assert.Equal(1, count);
		Assert.Single(s.IgnoredDevicePaths);
	}

	[Fact]
	public void TestRemoveIgnoredDeviceFiresOnlyWhenPathWasPresent() {
		TestableSettings s = new(new TestLogger());
		int count = 0;
		s.RegisterSettingsChangedListener(() => count++);

		s.RemoveIgnoredDevice("/dev/nonexistent"); // not present — no event
		Assert.Equal(0, count);

		s.AddIgnoredDevice("/dev/mouse0");   // count → 1
		s.RemoveIgnoredDevice("/dev/mouse0"); // count → 2
		Assert.Equal(2, count);
		Assert.Empty(s.IgnoredDevicePaths);
	}

	[Fact]
	public void TestMultipleIgnoredDevicesStoredIndependently() {
		TestableSettings s = new(new TestLogger());
		s.AddIgnoredDevice("/dev/mouse0");
		s.AddIgnoredDevice("/dev/mouse1");
		s.RemoveIgnoredDevice("/dev/mouse0");

		Assert.Single(s.IgnoredDevicePaths);
		Assert.Contains("/dev/mouse1", s.IgnoredDevicePaths);
	}

	// -------------------------------------------------------------------------
	// Reset()
	// -------------------------------------------------------------------------

	[Fact]
	public void TestResetRestoresDefaults() {
		TestableSettings s = new(new TestLogger());
		s.LeftThreshold = 999;
		s.RightThreshold = 999;
		s.MiddleThreshold = 999;
		s.X1Threshold = 999;
		s.X2Threshold = 999;
		s.WheelThreshold = 999;
		s.MinDelay = 999;
		s.DragStartTimeMilliseconds = 500;
		s.DragStopTimeMilliseconds = 200;
		s.IsRemoteDesktopDetectionEnabled = true;
		s.AddIgnoredDevice("/dev/mouse0");

		s.Reset();

		Assert.Equal(50, s.LeftThreshold);
		Assert.Equal(-1, s.RightThreshold);
		Assert.Equal(-1, s.MiddleThreshold);
		Assert.Equal(-1, s.X1Threshold);
		Assert.Equal(-1, s.X2Threshold);
		Assert.Equal(-1, s.WheelThreshold);
		Assert.Equal(-1, s.MinDelay);
		Assert.Equal(-1, s.DragStartTimeMilliseconds);
		Assert.Equal(-1, s.DragStopTimeMilliseconds);
		Assert.False(s.IsRemoteDesktopDetectionEnabled);
		Assert.Empty(s.IgnoredDevicePaths);
	}

	[Fact]
	public void TestResetFiresEventForChangedValues() {
		TestableSettings s = new(new TestLogger());
		s.RightThreshold = 99; // changed from default -1
							   // LeftThreshold is already 50 (default), so Reset() setting it to 50 should not fire

		int count = 0;
		s.RegisterSettingsChangedListener(() => count++);

		s.Reset();

		// RightThreshold changed (99 → -1): fires. LeftThreshold unchanged (50 → 50): no fire.
		Assert.True(count > 0);
		Assert.Equal(-1, s.RightThreshold);
		Assert.Equal(50, s.LeftThreshold);
	}
}
