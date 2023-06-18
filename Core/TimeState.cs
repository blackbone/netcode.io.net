using System.Runtime.InteropServices;

namespace NetcodeIO.NET.Core
{
    
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct TimeState
    {
        public ulong Time;
        public ulong Delta;
        
        public void Reset()
        {
            Time = 0;
            Delta = 0;
        }
    }
}