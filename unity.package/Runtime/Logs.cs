#if UNITY
using Debug = UnityEngine.Debug;
using Color = UnityEngine.Color;
#else
using Color = System.Drawing.Color;
#endif
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Logs
{
    private static readonly object NullOwner = new();
#if UNITY
    private static readonly Color Gray = Color.grey;
    private static readonly Color Yellow = Color.yellow;
    private static readonly Color Red = Color.red;
    private static readonly Color Nc = Color.white;
#else
    private static readonly Color Gray = Color.Gray;
    private static readonly Color Yellow = Color.Yellow;
    private static readonly Color Red = Color.Red;
    private static readonly Color Nc = Color.DarkGray;
#endif

    [Conditional("DEBUG")]
    [DebuggerHidden]
    public static void LogDebug(this object target, object message)
#if UNITY
        => Debug.Log($"[{ColorizeSelf(target)}] {Wrap(message, Gray)}");
#else
        => Console.WriteLine($"[{DateTimeOffset.Now.TimeOfDay}] [{ColorizeSelf(target)}] {CStart(Gray)}{message}.{CEnd()}");
#endif

    [DebuggerHidden]
    public static void Log(this object target, object message)
#if UNITY
        => Debug.Log($"[{ColorizeSelf(target)}] {message}.");
#else
        => Console.WriteLine($"[{DateTimeOffset.Now.TimeOfDay}] [{ColorizeSelf(target)}] {message}.");
#endif
    [DebuggerHidden]
    public static void LogWarning(this object target, object message)
#if UNITY
        => Debug.LogWarning($"[{ColorizeSelf(target)}] {message}.");
#else
        => Console.WriteLine($"[{DateTimeOffset.Now.TimeOfDay}] [{ColorizeSelf(target)}] {Wrap(message, Yellow)}.");
#endif
    [DebuggerHidden]
    public static void LogError(this object target, object message)
#if UNITY
        => Debug.LogError($"[{ColorizeSelf(target)}] {message}.");
#else
        => Console.WriteLine($"[{DateTimeOffset.Now.TimeOfDay}] [{ColorizeSelf(target)}] {Wrap(message, Red)}.");
#endif
    [DebuggerHidden]
    public static void LogException(this object target, Exception exception)
#if UNITY
        => Debug.LogException(exception); // TODO [Dmitrii Osipov] 
#else
        => Console.WriteLine($"[{DateTimeOffset.Now.TimeOfDay}] [{ColorizeSelf(target)}] Exception occured: {Wrap(exception.Message, Red)}{(string.IsNullOrEmpty(exception.StackTrace) ? "" : $"\n{exception.StackTrace}")}.", exception);
#endif
    [DebuggerHidden]
    public static void LogException(this object target, object message, Exception exception)
#if UNITY
        => Debug.LogException(exception); // TODO [Dmitrii Osipov] 
#else
        => Console.WriteLine($"[{DateTimeOffset.Now.TimeOfDay}] [{ColorizeSelf(target)}] {message}: {Wrap(exception.Message, Red)}{(string.IsNullOrEmpty(exception.StackTrace) ? "" : $"\n{exception.StackTrace}")}", exception);
#endif

    [DebuggerHidden]
    public static void LogAssertion(this object target, object message)
    {
        
    } 

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ColorizeSelf(object owner)
    {
        owner ??= NullOwner;

        var name = owner.GetType().Name;
        return Wrap($"{name}", GetColor(owner.GetHashCode()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Color GetColor(int hashCode)
    {
        var rgb = BitConverter.GetBytes(hashCode);
        var r = (byte)(128 + (rgb[0] + rgb[1]) / 4);
        var g = (byte)(128 + (rgb[1] + rgb[2]) / 4);
        var b = (byte)(128 + (rgb[2] + rgb[3]) / 4);
#if UNITY
        return new UnityEngine.Color32(r, g, b, 255);
#else
        return Color.FromArgb(r, g, b);
#endif
    }

    private static string Wrap(object message, Color color)
    {
        return $"{CStart(color)}{message}{CEnd()}";
    }

    private static string CStart(Color c)
#if UNITY
        => $"<color={UnityEngine.ColorUtility.ToHtmlStringRGBA(c)}>";
#else
        => $"\x1b[38;2;{c.R};{c.G};{c.B}m";
#endif

    private static string CEnd()
#if UNITY
        => "</color>";
#else
        => $"\x1b[38;2;{Nc.R};{Nc.G};{Nc.B}m";
#endif
}