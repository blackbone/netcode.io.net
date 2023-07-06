// Decompiled with JetBrains decompiler
// Type: UnityEngine.PlayerLoop.PreUpdate
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
  public struct PreUpdate
  {
    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PhysicsUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct Physics2DUpdate
    {
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct PhysicsClothUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CheckTexFieldInput
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct IMGUISendQueuedEvents
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct SendMouseEvents
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct AIUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct WindUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateVideo
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct NewInputUpdate
    {
    }
  }
}
