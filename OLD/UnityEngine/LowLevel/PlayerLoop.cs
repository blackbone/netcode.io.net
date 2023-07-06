// Decompiled with JetBrains decompiler
// Type: UnityEngine.LowLevel.PlayerLoop
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.LowLevel
{
  /// <summary>
  ///   <para>The class representing the player loop in Unity.</para>
  /// </summary>
  public class PlayerLoop
  {
    /// <summary>
    ///   <para>Returns the default update order of all engine systems in Unity.</para>
    /// </summary>
    public static PlayerLoopSystem GetDefaultPlayerLoop()
    {
      PlayerLoopSystemInternal[] playerLoopInternal = PlayerLoop.GetDefaultPlayerLoopInternal();
      int offset = 0;
      return PlayerLoop.InternalToPlayerLoopSystem(playerLoopInternal, ref offset);
    }

    /// <summary>
    ///   <para>Returns the current update order of all engine systems in Unity.</para>
    /// </summary>
    public static PlayerLoopSystem GetCurrentPlayerLoop()
    {
      PlayerLoopSystemInternal[] playerLoopInternal = PlayerLoop.GetCurrentPlayerLoopInternal();
      int offset = 0;
      return PlayerLoop.InternalToPlayerLoopSystem(playerLoopInternal, ref offset);
    }

    /// <summary>
    ///   <para>Set a new custom update order of all engine systems in Unity.</para>
    /// </summary>
    /// <param name="loop"></param>
    public static void SetPlayerLoop(PlayerLoopSystem loop)
    {
      List<PlayerLoopSystemInternal> internalSys = new List<PlayerLoopSystemInternal>();
      PlayerLoop.PlayerLoopSystemToInternal(loop, ref internalSys);
      PlayerLoop.SetPlayerLoopInternal(internalSys.ToArray());
    }

    private static int PlayerLoopSystemToInternal(
      PlayerLoopSystem sys,
      ref List<PlayerLoopSystemInternal> internalSys)
    {
      int count = internalSys.Count;
      PlayerLoopSystemInternal loopSystemInternal = new PlayerLoopSystemInternal()
      {
        type = sys.type,
        updateDelegate = sys.updateDelegate,
        updateFunction = sys.updateFunction,
        loopConditionFunction = sys.loopConditionFunction,
        numSubSystems = 0
      };
      internalSys.Add(loopSystemInternal);
      if (sys.subSystemList != null)
      {
        for (int index = 0; index < sys.subSystemList.Length; ++index)
          loopSystemInternal.numSubSystems += PlayerLoop.PlayerLoopSystemToInternal(sys.subSystemList[index], ref internalSys);
      }
      internalSys[count] = loopSystemInternal;
      return loopSystemInternal.numSubSystems + 1;
    }

    private static PlayerLoopSystem InternalToPlayerLoopSystem(
      PlayerLoopSystemInternal[] internalSys,
      ref int offset)
    {
      PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem()
      {
        type = internalSys[offset].type,
        updateDelegate = internalSys[offset].updateDelegate,
        updateFunction = internalSys[offset].updateFunction,
        loopConditionFunction = internalSys[offset].loopConditionFunction,
        subSystemList = (PlayerLoopSystem[]) null
      };
      int index = offset++;
      if (internalSys[index].numSubSystems > 0)
      {
        List<PlayerLoopSystem> playerLoopSystemList = new List<PlayerLoopSystem>();
        while (offset <= index + internalSys[index].numSubSystems)
          playerLoopSystemList.Add(PlayerLoop.InternalToPlayerLoopSystem(internalSys, ref offset));
        playerLoopSystem.subSystemList = playerLoopSystemList.ToArray();
      }
      return playerLoopSystem;
    }

    private static PlayerLoopSystemInternal[] GetDefaultPlayerLoopInternal() => throw new NotImplementedException();

    private static PlayerLoopSystemInternal[] GetCurrentPlayerLoopInternal() => throw new NotImplementedException();

    private static void SetPlayerLoopInternal(PlayerLoopSystemInternal[] loop) => throw new NotImplementedException();
  }
}
