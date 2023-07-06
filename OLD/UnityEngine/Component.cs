namespace Netcode.io.OLD.UnityEngine
{
    public class Component : Object
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public T GetComponent<T>() where T : Component
            => gameObject.GetComponent<T>();
        
        public bool TryGetComponent<T>(out T component) where T : Component
            => gameObject.TryGetComponent(out component);

        public T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : Component
            => gameObject.GetComponentsInChildren<T>(includeInactive);

        public T GetComponentInParent<T>() where T : Component
            => gameObject.GetComponentInParent<T>();

        public T[] FindObjectsOfType<T>(bool includeInactive = false) where T : Component
            => gameObject.FindObjectsOfType<T>(includeInactive);

        public void DontDestroyOnLoad(GameObject gameObject)
            => gameObject.DontDestroyOnLoad(gameObject);
    }
}