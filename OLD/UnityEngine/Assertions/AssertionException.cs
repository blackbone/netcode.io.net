// Decompiled with JetBrains decompiler
// Type: UnityEngine.Assertions.AssertionException
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.Assertions
{
    /// <summary>
    ///   <para>An exception that is thrown when an assertion fails.</para>
    /// </summary>
    public class AssertionException : Exception
    {
        private string m_UserMessage;

        public AssertionException(string message, string userMessage)
            : base(message)
        {
            this.m_UserMessage = userMessage;
        }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (this.m_UserMessage != null)
                    message = this.m_UserMessage + "\n" + message;
                return message;
            }
        }
    }
}