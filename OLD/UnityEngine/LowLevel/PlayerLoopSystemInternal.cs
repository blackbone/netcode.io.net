// Decompiled with JetBrains decompiler
// Type: UnityEngine.LowLevel.PlayerLoopSystemInternal
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.LowLevel
{
    internal struct PlayerLoopSystemInternal
    {
        public System.Type type;
        public PlayerLoopSystem.UpdateFunction updateDelegate;
        public IntPtr updateFunction;
        public IntPtr loopConditionFunction;
        public int numSubSystems;
    }
}