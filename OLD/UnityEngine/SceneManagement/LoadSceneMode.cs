// Decompiled with JetBrains decompiler
// Type: UnityEngine.SceneManagement.LoadSceneMode
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.SceneManagement
{
    /// <summary>
    ///   <para>Used when loading a Scene in a player.</para>
    /// </summary>
    public enum LoadSceneMode
    {
        /// <summary>
        ///   <para>Closes all current loaded Scenes
        ///           and loads a Scene.</para>
        /// </summary>
        Single,
        /// <summary>
        ///   <para>Adds the Scene to the current loaded Scenes.</para>
        /// </summary>
        Additive,
    }
}