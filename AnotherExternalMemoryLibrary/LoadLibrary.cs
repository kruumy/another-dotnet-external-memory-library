﻿using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class LoadLibrary
    {
        public static void LoadLibraryA(IntPtrEx pHandle, string dllPath)
        {
            byte[] dllPathBytes = dllPath.ToByteArray(true);
            IntPtrEx loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtrEx allocMemAddress = VirtualAllocEx(pHandle, 0, dllPathBytes.Length, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            WriteProcessMemory(pHandle, allocMemAddress, dllPathBytes, dllPathBytes.Length, out IntPtrEx _);
            CreateRemoteThread(pHandle, 0, 0, loadLibraryAddr, allocMemAddress, 0, out IntPtrEx _);
        }
    }
}
