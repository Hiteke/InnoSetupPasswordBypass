using System;

namespace InnoSetupPasswordBypass
{
    public class Utils
    {
        public const ushort UNK_BYTE = 256;

        public static bool ReadProcessMemory(IntPtr process, IntPtr addr, ref byte[] buffer)
        {
            int bytesRead = 0;
            return WinAPI.ReadProcessMemory(process, addr, buffer, buffer.Length, ref bytesRead) && bytesRead > 0;
        }

        public static bool WriteProcessMemory(IntPtr process, IntPtr addr, byte[] buffer)
        {
            int bytesWrite = 0;
            return WinAPI.WriteProcessMemory(process, addr, buffer, buffer.Length, ref bytesWrite) && bytesWrite == buffer.Length;
        }

        public static uint FindPattern(byte[] buffer, ushort[] pattern)
        {
            uint foundBytes = 0;
            for (uint i = 0; i < buffer.Length; i++)
            {
                ushort patternByte = pattern[foundBytes];
                if (buffer[i] == (byte)patternByte
                    || patternByte == UNK_BYTE)
                {
                    foundBytes++;

                    if (foundBytes == pattern.Length)
                        return i - (uint)pattern.Length + 1;
                }
                else
                {
                    i -= foundBytes;
                    foundBytes = 0;
                }
            }

            return 0;
        }
    }
}
