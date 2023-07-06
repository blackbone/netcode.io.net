using Newtonsoft.Json;

namespace Netcode.io.OLD.UnityEngine
{
    [Serializable]
    public sealed class Scene : IEquatable<Scene>
    {
        [JsonProperty] private GameObject[] objects;
        
        public int handle;
        public string name;
        public int buildIndex;
        public bool isLoaded;
        public string path;

        public bool IsValid() => throw new NotImplementedException();
        public bool Equals(Scene other) => handle == other.handle;
        public override bool Equals(object obj) => obj is Scene other && Equals(other);
        public override int GetHashCode() => handle;
        public static bool operator ==(Scene a, Scene b) => a.Equals(b);
        public static bool operator !=(Scene a, Scene b) => !(a == b);
        
        public GameObject Instantiate(GameObject prefab) => throw new NotImplementedException();
        public T Instantiate<T>(T prefab) where T : Component => Instantiate(prefab.gameObject).GetComponent<T>();
        public T[] FindObjectsOfType<T>(bool includeInactive = false) where T : Component => throw new NotImplementedException();
        public void DontDestroyOnLoad(GameObject gameObject) => throw new NotImplementedException();
    }
}