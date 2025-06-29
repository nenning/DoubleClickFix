using DoubleClickFix;
using DoubleClickFix.Tests.Helper;
using System;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix.Tests;


/*
 -- NEXT?
register & unregister mousehook (incl powermode changes)
include x1, x2 buttons
check mouse device switch?
 */
public class MouseHookTests
{
    private const int WM_MOUSEMOVE = 0x0200;

    private static void AssertAllowed(MouseHook hook, IntPtr wmMouseEvent, uint timeMs, int movedPixels = 0)
    {
        AssertMouseEvent(hook, wmMouseEvent, timeMs, true, movedPixels);
    }
    private static void AssertIgnored(MouseHook hook, IntPtr wmMouseEvent, uint timeMs, int movedPixels = 0)
    {
        AssertMouseEvent(hook, wmMouseEvent, timeMs, false, movedPixels);
    }
    private static void AssertMouseEvent(MouseHook hook, IntPtr wmMouseEvent, uint timeMs, bool allowed, int movedPixels = 0)
    {
        using var data = HookStruct.Create(timeMs, movedPixels);
        Assert.Equal(allowed ? 0 : 1, hook.HookCallback(0, wmMouseEvent, data.Pointer));
    }

    [Fact]
    public void TestLeftClickIgnored()
    {
        TestNativeMethods nativeMethods = new();
        MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

        AssertAllowed(hook, WM_LBUTTONDOWN, 100);
        AssertAllowed(hook, WM_LBUTTONUP, 110);
        AssertIgnored(hook, WM_LBUTTONDOWN, 120);
        AssertAllowed(hook, WM_LBUTTONUP, 130);
        Assert.Equal(3, nativeMethods.CallCounter);
    }

    [Fact]
    public void TestLeftClickAllowed()
    {
        TestNativeMethods nativeMethods = new();
        MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

        AssertAllowed(hook, WM_LBUTTONDOWN, 100);
        AssertAllowed(hook, WM_LBUTTONUP, 110);
        AssertAllowed(hook, WM_LBUTTONDOWN, 170);
        AssertAllowed(hook, WM_LBUTTONUP, 180);
        Assert.Equal(4, nativeMethods.CallCounter);
    }

    [Fact]
    public void TestMultipleClicks()
    {
        TestNativeMethods nativeMethods = new();
        MouseHook hook = new(new TestSettings(), new TestLogger(), nativeMethods);

        AssertAllowed(hook, WM_LBUTTONDOWN, 100);
        AssertAllowed(hook, WM_LBUTTONUP, 110);
        AssertAllowed(hook, WM_LBUTTONDOWN, 170);
        AssertAllowed(hook, WM_LBUTTONUP, 180);
        AssertIgnored(hook, WM_LBUTTONDOWN, 190);
        AssertAllowed(hook, WM_LBUTTONUP, 200);
        Assert.Equal(5, nativeMethods.CallCounter);

        AssertAllowed(hook, WM_LBUTTONDOWN, 300);
        AssertAllowed(hook, WM_LBUTTONUP, 310);
        AssertIgnored(hook, WM_LBUTTONDOWN, 320);
        AssertAllowed(hook, WM_LBUTTONUP, 330);
        Assert.Equal(8, nativeMethods.CallCounter);

        AssertAllowed(hook, WM_LBUTTONDOWN, 400);
        AssertAllowed(hook, WM_LBUTTONUP, 410);
        AssertIgnored(hook, WM_LBUTTONDOWN, 420);
        AssertAllowed(hook, WM_LBUTTONUP, 430);
        Assert.Equal(11, nativeMethods.CallCounter);

        AssertAllowed(hook, WM_LBUTTONDOWN, 500);
        AssertAllowed(hook, WM_LBUTTONUP, 510);
        AssertAllowed(hook, WM_LBUTTONDOWN, 570);
        AssertAllowed(hook, WM_LBUTTONUP, 580);
        Assert.Equal(15, nativeMethods.CallCounter);
    }

    [Fact]
    public void TestBasicClicksClickIgnored()
    {
        MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());

        AssertAllowed(hook, WM_LBUTTONUP, 100);
        AssertIgnored(hook, WM_LBUTTONDOWN, 110);

        AssertAllowed(hook, WM_LBUTTONUP, 100);
        AssertAllowed(hook, WM_LBUTTONDOWN, 160);

        AssertAllowed(hook, WM_RBUTTONUP, 200);
        AssertAllowed(hook, WM_RBUTTONDOWN, 210);

        AssertAllowed(hook, WM_MBUTTONUP, 300);
        AssertAllowed(hook, WM_MBUTTONDOWN, 310);
    }


    [Fact]
    public void TestInterleavedClicks()
    {
        MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());

        AssertAllowed(hook, WM_LBUTTONUP, 100);
        AssertAllowed(hook, WM_RBUTTONUP, 101);
        AssertIgnored(hook, WM_LBUTTONDOWN, 120);
        AssertAllowed(hook, WM_RBUTTONDOWN, 121);

        AssertAllowed(hook, WM_LBUTTONUP, 200);
        AssertAllowed(hook, WM_RBUTTONUP, 201);
        AssertAllowed(hook, WM_LBUTTONDOWN, 260);
        AssertAllowed(hook, WM_RBUTTONDOWN, 261);
    }

    [Fact]
    public void TestSettingsChanged()
    {
        TestSettings settings = new();
        MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());
        settings.LeftThreshold = -1;
        settings.RightThreshold = 10;
        settings.FireSettingsChanged();

        AssertAllowed(hook, WM_LBUTTONUP, 100);
        AssertAllowed(hook, WM_LBUTTONDOWN, 101);

        AssertAllowed(hook, WM_RBUTTONUP, 102);
        AssertIgnored(hook, WM_RBUTTONDOWN, 110);

        AssertAllowed(hook, WM_RBUTTONUP, 111);
        AssertAllowed(hook, WM_RBUTTONDOWN, 130);

    }

    [Theory]
    [InlineData(-1, -1, -1, -1, -1, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, true)] // everything disabled
    [InlineData(-1, -1, -1, -1, -1, -1, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 30, true)]
    [InlineData(50, 50, 50, 50, 50, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, false)] // everything enabled
    [InlineData(50, 50, 50, 50, 50, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 50, true)] 
    [InlineData(20, -1, -1, -1, -1, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, false)] // delay = 0 -> ignored
    [InlineData(-1, 20, -1, -1, -1, -1, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 0, false)]
    [InlineData(-1, -1, 20, -1, -1, -1, 0, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 0, false)]
    [InlineData(20, -1, -1, -1, -1, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 19, false)] // delay < timeout -> ignored
    [InlineData(-1, 20, -1, -1, -1, -1, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 19, false)]
    [InlineData(-1, -1, 20, -1, -1, -1, 0, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 19, false)]
    [InlineData(20, -1, -1, -1, -1, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 20, true)] // delay == timeout -> allowed
    [InlineData(-1, 20, -1, -1, -1, -1, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 20, true)]
    [InlineData(-1, -1, 20, -1, -1, -1, 0, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 20, true)]
    [InlineData(20, -1, -1, -1, -1, -1, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 30, true)] // delay >= timeout -> not allowed
    [InlineData(-1, 20, -1, -1, -1, -1, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 30, true)]
    [InlineData(-1, -1, 20, -1, -1, -1, 0, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 30, true)]
    [InlineData(50, 50, 50, 50, 50, 0, 0, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, true)] // minDelay set to 0
    [InlineData(50, 50, 50, 50, 50, 0, 0, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 0, true)]
    [InlineData(50, 50, 50, 50, 50, 0, 0, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 0, true)]
    [InlineData(50, 50, 50, 50, 50, -1, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, true)] // ignored device set to -1 (the default of mouse hook)
    [InlineData(50, 50, 50, 50, 50, -1, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 0, true)]
    [InlineData(50, 50, 50, 50, 50, -1, -1, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 0, true)]
    [InlineData(50, 50, 50, 50, 50, 0, -1, (int)WM_LBUTTONUP, (int)WM_LBUTTONDOWN, 0, true)] // ignoredDevice=-1 and mindelay=0
    [InlineData(50, 50, 50, 50, 50, 0, -1, (int)WM_RBUTTONUP, (int)WM_RBUTTONDOWN, 1, true)]
    [InlineData(50, 50, 50, 50, 50, 0, -1, (int)WM_MBUTTONUP, (int)WM_MBUTTONDOWN, 2, true)]

    public void TestSettingsCombinations(int lTimeout, int rTimeout, int mTimeout, int x1Timeout, int x2Timeout, int minDelay, int device, int up, int down, uint ms, bool allowed) {
        TestSettings settings = new();
        MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());
        settings.LeftThreshold = lTimeout;
        settings.RightThreshold = rTimeout;
        settings.MiddleThreshold = mTimeout;
        settings.X1Threshold = x1Timeout;
        settings.X2Threshold = x2Timeout;
        settings.MinDelay = minDelay;
        settings.IgnoredDevice = device;
        settings.FireSettingsChanged();

        AssertAllowed(hook, up, 100);
        AssertMouseEvent(hook, down, 100 + ms, allowed);
    }

    [Fact]
    public void TestSwitchDevice()
    {
        /*
        RAWINPUT input = new()
        {
            Header = new RAWINPUTHEADER
            {
                Device = 999,
                Type = RIM_TYPEMOUSE,
            },
            Mouse = new()
        };

        MouseHook hook = new(new TestSettings(), new TestLogger(), new TestNativeMethods());
        //TODO hook.ProcessRawInput(*input);
        */
    }

    [Fact]
    public void TestDragLockEnabled()
    {
        TestSettings settings = new()
        {
            DragStartTimeMilliseconds = 100,
            DragStopTimeMilliseconds = 200
        };
        MouseHook hook = new(settings, new TestLogger(), new TestNativeMethods());

        // Simulate a drag event
        AssertAllowed(hook, WM_LBUTTONDOWN, 200); // Initial press for drag-lock
        AssertAllowed(hook, WM_MOUSEMOVE, 450, 20);  // Movement starts drag-lock
        AssertIgnored(hook, WM_LBUTTONUP, 550);  // Drag-lock active, suppress release
        AssertIgnored(hook, WM_LBUTTONDOWN, 551);  // Drag-lock active, suppress press
        AssertAllowed(hook, WM_LBUTTONUP, 800); // Drag-lock ends, allow release
    }

}