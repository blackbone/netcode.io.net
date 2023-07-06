using Netcode.io.OLD.NetworkVariable;

namespace Netcode.io
{
    public static class NetCode
    {
        public static void Initialize()
        {
            NetworkVariableSerializationTypes.InitializeIntegerSerialization();
        }
    }
}