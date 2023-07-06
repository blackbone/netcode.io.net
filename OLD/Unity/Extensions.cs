namespace Netcode.io.OLD.Unity;

public static class Extensions
{
    public static void InsertRangeWithBeginEnd<T>(this List<T> list, int begin, int end)
    {
        int items = end - begin;
        if(items < 1) return;
        list.InsertRange(begin, Enumerable.Repeat(default(T), end - begin));
    }

    public static unsafe void* GetUnsafePtr<T>(this ref Span<T> span) where T : unmanaged
    {
        fixed (void* p = span) return p;
    }
    
    public static unsafe void* GetUnsafePtr<T>(this T[] array) where T : unmanaged
    {
        fixed (void* p = array) return p;
    }
}