﻿using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix.Tests.Helper;

class HookStruct : IDisposable
{
    private readonly nint ptrHookStruct = 0;
    private readonly MSLLHOOKSTRUCT hookStruct;

    private HookStruct(MSLLHOOKSTRUCT hookStruct)
    {
        this.hookStruct = hookStruct;

        // Allocate memory for the structure
        ptrHookStruct = Marshal.AllocHGlobal(Marshal.SizeOf(this.hookStruct));

        // Copy the structure to the allocated memory
        Marshal.StructureToPtr(this.hookStruct, ptrHookStruct, false);

    }
    public static HookStruct Create(uint timeMs)
    {
        MSLLHOOKSTRUCT hookStruct = new()
        {
            time = timeMs,
        };
        return new HookStruct(hookStruct);
    }
    public nint Pointer { get { return ptrHookStruct; } }
    public void Dispose()
    {
        if (ptrHookStruct != nint.Zero)
        {
            Marshal.FreeHGlobal(ptrHookStruct);
        }
    }
}
