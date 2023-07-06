// Decompiled with JetBrains decompiler
// Type: UnityEngine.Assertions.AssertionMessageUtil
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.Assertions
{
    internal class AssertionMessageUtil
    {
        private const string k_Expected = "Expected:";
        private const string k_AssertionFailed = "Assertion failure.";

        public static string GetMessage(string failureMessage) => $"{(object)"Assertion failure."} {(object)failureMessage}";

        public static string GetMessage(string failureMessage, string expected) => AssertionMessageUtil.GetMessage($"{(object)failureMessage}{(object)Environment.NewLine}{(object)"Expected:"} {(object)expected}");

        public static string GetEqualityMessage(object actual, object expected, bool expectEqual) => AssertionMessageUtil.GetMessage($"Values are {(expectEqual ? (object)"not " : (object)"")}equal.", string.Format("{0} {2} {1}", actual, expected, expectEqual ? (object) "==" : (object) "!="));

        public static string NullFailureMessage(object value, bool expectNull) => AssertionMessageUtil.GetMessage($"Value was {(expectNull ? (object)"not " : (object)"")}Null", $"Value was {(expectNull ? (object)"" : (object)"not ")}Null");

        public static string BooleanFailureMessage(bool expected) => AssertionMessageUtil.GetMessage("Value was " + (!expected).ToString(), expected.ToString());
    }
}