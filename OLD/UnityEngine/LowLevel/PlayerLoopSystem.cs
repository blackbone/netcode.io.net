// Decompiled with JetBrains decompiler
// Type: UnityEngine.LowLevel.PlayerLoopSystem
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.LowLevel
{
  /// <summary>
  ///   <para>The representation of a single system being updated by the player loop in Unity.</para>
  /// </summary>
  public struct PlayerLoopSystem
  {
    /// <summary>
    ///   <para>This property is used to identify which native system this belongs to, or to get the name of the managed system to show in the profiler.</para>
    /// </summary>
    public System.Type type;
    /// <summary>
    ///   <para>A list of sub systems which run as part of this item in the player loop.</para>
    /// </summary>
    public PlayerLoopSystem[] subSystemList;
    /// <summary>
    ///   <para>A managed delegate. You can set this to create a new C# entrypoint in the player loop.</para>
    /// </summary>
    public PlayerLoopSystem.UpdateFunction updateDelegate;
    /// <summary>
    ///   <para>A native engine system. To get a valid value for this, you must copy it from one of the PlayerLoopSystems returned by PlayerLoop.GetDefaultPlayerLoop.</para>
    /// </summary>
    public IntPtr updateFunction;
    /// <summary>
    ///   <para>The loop condition for a native engine system. To get a valid value for this, you must copy it from one of the PlayerLoopSystems returned by PlayerLoop.GetDefaultPlayerLoop.</para>
    /// </summary>
    public IntPtr loopConditionFunction;

    public override string ToString() => this.type.Name;

    public delegate void UpdateFunction();
  }
}
