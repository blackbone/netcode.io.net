using System.Numerics;

namespace Netcode.io.OLD.UnityEngine
{
    public class Transform : Component
    {
        public Vector3 position;
        public Vector3 localPosition;
        public Vector3 lossyScale;
        public Vector3 localScale;
        public Quaternion rotation;
        public Quaternion localRotation;
        public Vector3 eulerAngles;
        public Vector3 localEulerAngles;

        public Transform parent { get; set; }
        public Transform root { get; }

        public void SetParent(Transform parent, bool worldPositionStays = true) => throw new NotImplementedException();
    }
}