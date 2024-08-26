using System;
using System.Diagnostics;
using System.Windows;

namespace InnoSetupPasswordBypass.InnoSetup
{
    public class InnoSetupPatch
    {
        private static readonly ushort[] PASSWORD_CHECK_PATTERN = new ushort[] 
        { 0xE8, Utils.UNK_BYTE, Utils.UNK_BYTE, Utils.UNK_BYTE, Utils.UNK_BYTE, 0x81, 0xC4, 0xB0, 0x01 };

        private static readonly byte[] PASSWORD_CHECK_BYPASS = new byte[]
        { 0xB8, 0x01, 0x00, 0x00, 0x00 };

        public static bool BypassPassword(int processId)
        {
            Process process = Process.GetProcessById(processId);

            if (process == null)
                return false;

            IntPtr handle = WinAPI.OpenProcess(WinAPI.PROCESS_VM_READ | WinAPI.PROCESS_VM_WRITE | WinAPI.PROCESS_VM_OPERATION, false, process.Id);

            if (handle == IntPtr.Zero)
                return false;

            byte[] moduleMem = new byte[process.MainModule.ModuleMemorySize];

            if (!Utils.ReadProcessMemory(handle, process.MainModule.BaseAddress, ref moduleMem))
                return false;

            uint addr = Utils.FindPattern(moduleMem, PASSWORD_CHECK_PATTERN);

            if (addr == 0)
                return false;

            if (!Utils.WriteProcessMemory(handle, (IntPtr)(process.MainModule.BaseAddress.ToInt64() + addr), PASSWORD_CHECK_BYPASS))
                return false;

            return true;
        }
    }
}
