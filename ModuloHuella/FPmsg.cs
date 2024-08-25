using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ModuloHuella

{
    public class FPMsg
    {
        /* Message type definition */
        public enum FP_MSG_TYPE_T
        {
            FP_MSG_PRESS_FINGER,                // Enter fingerprint. Prompt to press finger.
            FP_MSG_RISE_FINGER,                 // Enter fingerprint and prompt to raise finger
            FP_MSG_ENROLL_TIME,                 // Fingerprint entry times prompt
            FP_MSG_CAPTURED_IMAGE,              // Enter fingerprint image feedback            
        };

        //* Message processing function definition*/
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FpMessageHandler(FP_MSG_TYPE_T enMsgType, IntPtr pMsgData);


    }
}
