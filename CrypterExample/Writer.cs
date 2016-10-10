using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrypterExample
{

    class Writer
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr BeginUpdateResource(string pFileName,
        [MarshalAs(UnmanagedType.Bool)]bool bDeleteExistingResources);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool UpdateResource(IntPtr hUpdate, string lpType, string lpName, ushort wLanguage, IntPtr lpData, uint cbData);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        public enum ICResult
        {
            Success,
            FailBegin,
            FailUpdate,
            FailEnd
        }

        public static ICResult WriteResource(string FileName, byte[] FileBytes)
        {
            IntPtr hUpdate = BeginUpdateResource(FileName, false);
            GCHandle Handle = GCHandle.Alloc(FileBytes, GCHandleType.Pinned);
           
            if (!UpdateResource(hUpdate, "RT_RCDATA", "Encrypted", 1066, Handle.AddrOfPinnedObject(), Convert.ToUInt32(FileBytes.Length)))
                return ICResult.FailUpdate;
            if (!EndUpdateResource(hUpdate, false))
                return ICResult.FailEnd;

            return ICResult.Success;


        }
    }
}
