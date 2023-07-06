namespace Netcode.io.OLD.UnityEngine
{
    public class ScriptableObject : Object
    {
        public static T CreateInstance<T>() where T : ScriptableObject
            => Activator.CreateInstance<T>();
    }
}