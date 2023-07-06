namespace Netcode.io.OLD.UnityEngine
{
    public class GameObject : Object
    {
        public int? m_InstanceId;
        public Scene scene { get; }
        public bool activeInHierarchy { get; }
        public Transform transform { get; }

        public T GetComponent<T>() where T : Component => throw new NotImplementedException();
        
        public bool TryGetComponent<T>(out T component) where T : Component => throw new NotImplementedException();
        
        public T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : Component => throw new NotImplementedException();
        
        public T GetComponentInParent<T>() where T : Component => throw new NotImplementedException();

        public void SetActive(bool active) => throw new NotImplementedException();
        
        public virtual T[] FindObjectsOfType<T>(bool includeInactive = false) where T : Component => scene.FindObjectsOfType<T>(includeInactive);

        public virtual GameObject Instantiate(GameObject prefab) => scene.Instantiate(prefab);
        public virtual T Instantiate<T>(T prefab) where T : Component => scene.Instantiate(prefab);

        public void DontDestroyOnLoad(GameObject gameObject) => scene.DontDestroyOnLoad(gameObject);

    }
}