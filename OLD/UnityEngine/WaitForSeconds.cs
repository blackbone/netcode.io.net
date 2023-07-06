// Decompiled with JetBrains decompiler
// Type: UnityEngine.WaitForSeconds
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

using System.Runtime.InteropServices;

namespace Netcode.io.OLD.UnityEngine
{
    /// <summary>
    ///   <para>Suspends the coroutine execution for the given amount of seconds using scaled time.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class WaitForSeconds : YieldInstruction
    {
        internal float m_Seconds;

        /// <summary>
        ///   <para>Suspends the coroutine execution for the given amount of seconds using scaled time.</para>
        /// </summary>
        /// <param name="seconds">Delay execution by the amount of time in seconds.</param>
        public WaitForSeconds(float seconds) => this.m_Seconds = seconds;
    }
}