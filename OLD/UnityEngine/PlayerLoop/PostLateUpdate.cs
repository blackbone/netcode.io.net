// Decompiled with JetBrains decompiler
// Type: UnityEngine.PlayerLoop.PostLateUpdate
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
  public struct PostLateUpdate
  {
    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PlayerSendFrameStarted
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateRectTransform
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateCanvasRectTransform
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PlayerUpdateCanvases
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateAudio
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
    public struct DirectorLateUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ScriptRunDelayedDynamicFrameRate
    {
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct VFXUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ParticleSystemEndUpdateAll
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct EndGraphicsJobsAfterScriptLateUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateSubstance
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateCustomRenderTextures
    {
    }

    /// <summary>
    ///   <para>Native engine system updated by the Player loop.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct XRPostLateUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateAllRenderers
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateLightProbeProxyVolumes
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct EnlightenRuntimeUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateAllSkinnedMeshes
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ProcessWebSendMessages
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct SortingGroupsUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateVideoTextures
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct DirectorRenderImage
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PlayerEmitCanvasGeometry
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct FinishFrameRendering
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct BatchModeUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PlayerSendFrameComplete
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateCaptureScreenshot
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PresentAfterDraw
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ClearImmediateRenderers
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct XRPostPresent
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct UpdateResolution
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct InputEndFrame
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct GUIClearEvents
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ShaderHandleErrors
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ResetInputAxis
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ThreadedLoadingDebug
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ProfilerSynchronizeStats
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MemoryFrameMaintenance
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ExecuteGameCenterCallbacks
    {
    }

    /// <summary>
    ///   <para>Native engine system updated by the Player loop.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct XRPreEndFrame
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ProfilerEndFrame
    {
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct GraphicsWarmupPreloadedShaders
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PlayerSendFramePostPresent
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PhysicsSkinnedClothBeginUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PhysicsSkinnedClothFinishUpdate
    {
    }

    /// <summary>
    ///   <para>A native engine system that the native player loop updates.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct TriggerEndOfFrameCallbacks
    {
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ObjectDispatcherPostLateUpdate
    {
    }
  }
}
