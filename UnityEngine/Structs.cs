namespace UnityEngine
{
    public struct Vector2
    {
        public float x;
        public float y;
    }

    public struct Vector3
    {
        public int x;
        public int y;
        public int z;
    }

    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    
    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    
    public struct Vector2Int
    {
        public int x;
        public int y;
    }
    
    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;
    }
    
    public struct Color32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }
    
    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
    
    public struct Ray2D
    {
        public Vector2 origin;
        public Vector2 direction;

        public Ray2D(Vector2 origin, Vector2 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
}