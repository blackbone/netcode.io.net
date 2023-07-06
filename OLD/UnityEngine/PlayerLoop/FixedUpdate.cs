// Decompiled with JetBrains decompiler
// Type: UnityEngine.PlayerLoop.FixedUpdate
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

using System.Runtime.InteropServices;

namespace Netcode.io.OLD.UnityEngine.PlayerLoop
{
  /// <summary>
  ///   <para>Update phase in the native player loop.</para>
  /// </summary>
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct FixedUpdate
  {
    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ClearLines
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct DirectorFixedSampleTime
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct AudioFixedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ScriptRunBehaviourFixedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct DirectorFixedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct LegacyFixedAnimationUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct XRFixedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PhysicsFixedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct Physics2DFixedUpdate
    {
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct PhysicsClothFixedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct DirectorFixedUpdatePostPhysics
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ScriptRunDelayedFixedFrameRate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct NewInputFixedUpdate
    {
    }
  }
}
