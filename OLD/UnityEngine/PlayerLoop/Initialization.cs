// Decompiled with JetBrains decompiler
// Type: UnityEngine.PlayerLoop.Initialization
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
  public struct Initialization
  {
    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ProfilerStartFrame
    {
    }

    [Obsolete("PlayerUpdateTime player loop component has been moved to its own category called TimeUpdate. (UnityUpgradable) -> UnityEngine.PlayerLoop.TimeUpdate/WaitForLastPresentationAndUpdateTime", true)]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PlayerUpdateTime
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateCameraMotionVectors
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct DirectorSampleTime
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct AsyncUploadTimeSlicedUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct SynchronizeState
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct SynchronizeInputs
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct XREarlyUpdate
    {
    }
  }
}
