// Decompiled with JetBrains decompiler
// Type: UnityEngine.PlayerLoop.Update
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
    public struct Update
    {
        /// <summary>
        ///   <para>A native engine system that the native player loop updates.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct ScriptRunBehaviourUpdate
        {
        }

        /// <summary>
        ///   <para>A native engine system that the native player loop updates.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct DirectorUpdate
        {
        }

        /// <summary>
        ///   <para>A native engine system that the native player loop updates.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct ScriptRunDelayedDynamicFrameRate
        {
        }

        /// <summary>
        ///   <para>A native engine system that the native player loop updates.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct ScriptRunDelayedTasks
        {
        }
    }
}