// Decompiled with JetBrains decompiler
// Type: UnityEngine.AsyncOperation
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

using System.Runtime.InteropServices;

namespace Netcode.io.OLD.UnityEngine
{
  /// <summary>
  ///   <para>Asynchronous operation coroutine.</para>
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public class AsyncOperation : YieldInstruction
  {
    internal IntPtr m_Ptr;
    private Action<AsyncOperation> m_completeCallback;


    /// <summary>
    ///   <para>Has the operation finished? (Read Only)</para>
    /// </summary>
    public bool isDone => throw new NotImplementedException();

    /// <summary>
    ///   <para>What's the operation's progress. (Read Only)</para>
    /// </summary>
    public float progress => throw new NotImplementedException();

    /// <summary>
    ///   <para>Priority lets you tweak in which order async operation calls will be performed.</para>
    /// </summary>
    public int priority
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    ///   <para>Allow Scenes to be activated as soon as it is ready.</para>
    /// </summary>
    public bool allowSceneActivation 
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
    
    internal void InvokeCompletionEvent()
    {
      if (this.m_completeCallback == null)
        return;
      this.m_completeCallback(this);
      this.m_completeCallback = (Action<AsyncOperation>) null;
    }

    public event Action<AsyncOperation> completed
    {
      add
      {
        if (this.isDone)
          value(this);
        else
          this.m_completeCallback += value;
      }
      remove => this.m_completeCallback -= value;
    }
  }
}
