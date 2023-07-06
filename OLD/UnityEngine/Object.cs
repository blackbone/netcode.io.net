namespace Netcode.io.OLD.UnityEngine
{
    public abstract class Object : IDisposable
    {
        public string name { get; set; }
        private int m_InstanceID;
        
        public int GetInstanceID() => this.m_InstanceID;
        public override int GetHashCode() => this.m_InstanceID;
        public override bool Equals(object other)
        {
            var rhs = other as Object;
            return (rhs != null || other == null) && CompareBaseObjects(this, rhs);
        }
        
        private static bool CompareBaseObjects(Object lhs, Object rhs)
        {
            var flag1 = lhs == null;
            var flag2 = rhs == null;
            if (flag2 & flag1) return true;
            if (!flag2 & !flag1) return lhs.m_InstanceID == rhs.m_InstanceID;
            return false;
        }

        public static implicit operator bool(Object o) => o != null;
        public static bool operator ==(Object a, Object b) => !Equals(a, null) && !Equals(b, null) && a.m_InstanceID == b.m_InstanceID;
        public static bool operator !=(Object a, Object b) => !(a == b);

        public static void Destroy(Object obj, float time) => throw new NotImplementedException();
        
        public static void Destroy(Object obj)
        {
            float t = 0.0f;
            Destroy(obj, t);
        }
        
        public static void DestroyImmediate(Object obj)
        {
            float t = 0.0f;
            Destroy(obj, t);
        }

        public void Dispose() => OnDispose();

        protected virtual void OnDispose()
        {
            
        }
    }
}