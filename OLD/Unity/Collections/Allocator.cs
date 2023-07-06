// Decompiled with JetBrains decompiler
// Type: Unity.Collections.Allocator
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.Unity.Collections
{
    /// <summary>
    ///   <para>Used to specify allocation type for NativeArray.</para>
    /// </summary>
    public enum Allocator
    {
        /// <summary>
        ///   <para>Invalid allocation.</para>
        /// </summary>
        Invalid = 0,
        /// <summary>
        ///   <para>No allocation.</para>
        /// </summary>
        None = 1,
        /// <summary>
        ///   <para>Temporary allocation.</para>
        /// </summary>
        Temp = 2,
        /// <summary>
        ///   <para>Temporary job allocation.</para>
        /// </summary>
        TempJob = 3,
        /// <summary>
        ///   <para>Persistent allocation.</para>
        /// </summary>
        Persistent = 4,
        /// <summary>
        ///   <para>Allocation associated with a DSPGraph audio kernel.</para>
        /// </summary>
        AudioKernel = 5,
        /// <summary>
        ///   <para>First user defined allocator index.</para>
        /// </summary>
        FirstUserIndex = 64, // 0x00000040
    }
}