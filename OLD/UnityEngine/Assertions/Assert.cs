// Decompiled with JetBrains decompiler
// Type: UnityEngine.Assertions.Assert
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

using System.ComponentModel;
using System.Diagnostics;
using Netcode.io.OLD.UnityEngine.TestRunner;

namespace Netcode.io.OLD.UnityEngine.Assertions
{
  /// <summary>
  ///   <para>The Assert class contains assertion methods for setting invariants in the code.</para>
  /// </summary>
  [DebuggerStepThrough]
  public static class Assert
  {
    internal const string UNITY_ASSERTIONS = "UNITY_ASSERTIONS";
    /// <summary>
    ///   <para>Obsolete. Do not use.</para>
    /// </summary>
    [Obsolete("Future versions of Unity are expected to always throw exceptions and not have this field.")]
    public static bool raiseExceptions = true;

    private static void Fail(string message, string userMessage)
    {
      if (Assert.raiseExceptions)
        throw new AssertionException(message, userMessage);
      if (message == null)
        message = "Assertion has failed\n";
      if (userMessage != null)
        message = userMessage + "\n" + message;
      Logs.LogAssertion(null, (object) message);
    }

    [Obsolete("Assert.Equals should not be used for Assertions", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static bool Equals(object obj1, object obj2) => throw new InvalidOperationException("Assert.Equals should not be used for Assertions");

    [Obsolete("Assert.ReferenceEquals should not be used for Assertions", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static bool ReferenceEquals(object obj1, object obj2) => throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");

    /// <summary>
    ///   <para>Asserts that the condition is true.</para>
    /// </summary>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="condition">true or false.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void IsTrue(bool condition)
    {
      if (condition)
        return;
      Assert.IsTrue(condition, (string) null);
    }

    /// <summary>
    ///   <para>Asserts that the condition is true.</para>
    /// </summary>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="condition">true or false.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void IsTrue(bool condition, string message)
    {
      if (condition)
        return;
      Assert.Fail(AssertionMessageUtil.BooleanFailureMessage(true), message);
    }

    /// <summary>
    ///   <para>Return true when the condition is false.  Otherwise return false.</para>
    /// </summary>
    /// <param name="condition">true or false.</param>
    /// <param name="message">The string used to describe the result of the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void IsFalse(bool condition)
    {
      if (!condition)
        return;
      Assert.IsFalse(condition, (string) null);
    }

    /// <summary>
    ///   <para>Return true when the condition is false.  Otherwise return false.</para>
    /// </summary>
    /// <param name="condition">true or false.</param>
    /// <param name="message">The string used to describe the result of the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void IsFalse(bool condition, string message)
    {
      if (!condition)
        return;
      Assert.Fail(AssertionMessageUtil.BooleanFailureMessage(false), message);
    }

    /// <summary>
    ///   <para>Assert the values are approximately equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreApproximatelyEqual(float expected, float actual) => Assert.AreEqual<float>(expected, actual, (string) null, (IEqualityComparer<float>) FloatComparer.s_ComparerWithDefaultTolerance);

    /// <summary>
    ///   <para>Assert the values are approximately equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreApproximatelyEqual(float expected, float actual, string message) => Assert.AreEqual<float>(expected, actual, message, (IEqualityComparer<float>) FloatComparer.s_ComparerWithDefaultTolerance);

    /// <summary>
    ///   <para>Assert the values are approximately equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreApproximatelyEqual(float expected, float actual, float tolerance) => Assert.AreApproximatelyEqual(expected, actual, tolerance, (string) null);

    /// <summary>
    ///   <para>Assert the values are approximately equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    public static void AreApproximatelyEqual(
      float expected,
      float actual,
      float tolerance,
      string message)
    {
      Assert.AreEqual<float>(expected, actual, message, (IEqualityComparer<float>) new FloatComparer(tolerance));
    }

    /// <summary>
    ///   <para>Asserts that the values are approximately not equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotApproximatelyEqual(float expected, float actual) => Assert.AreNotEqual<float>(expected, actual, (string) null, (IEqualityComparer<float>) FloatComparer.s_ComparerWithDefaultTolerance);

    /// <summary>
    ///   <para>Asserts that the values are approximately not equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotApproximatelyEqual(float expected, float actual, string message) => Assert.AreNotEqual<float>(expected, actual, message, (IEqualityComparer<float>) FloatComparer.s_ComparerWithDefaultTolerance);

    /// <summary>
    ///   <para>Asserts that the values are approximately not equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance) => Assert.AreNotApproximatelyEqual(expected, actual, tolerance, (string) null);

    /// <summary>
    ///   <para>Asserts that the values are approximately not equal.</para>
    /// </summary>
    /// <param name="tolerance">Tolerance of approximation.</param>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotApproximatelyEqual(
      float expected,
      float actual,
      float tolerance,
      string message)
    {
      Assert.AreNotEqual<float>(expected, actual, message, (IEqualityComparer<float>) new FloatComparer(tolerance));
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual<T>(T expected, T actual) => Assert.AreEqual<T>(expected, actual, (string) null);

    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual<T>(T expected, T actual, string message) => Assert.AreEqual<T>(expected, actual, message, (IEqualityComparer<T>) EqualityComparer<T>.Default);

    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual<T>(
      T expected,
      T actual,
      string message,
      IEqualityComparer<T> comparer)
    {
      if (typeof (UnityEngine.Object).IsAssignableFrom(typeof (T)))
      {
        Assert.AreEqual((object) expected as UnityEngine.Object, (object) actual as UnityEngine.Object, message);
      }
      else
      {
        if (comparer.Equals(actual, expected))
          return;
        Assert.Fail(AssertionMessageUtil.GetEqualityMessage((object) actual, (object) expected, true), message);
      }
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(UnityEngine.Object expected, UnityEngine.Object actual, string message)
    {
      if (!(actual != expected))
        return;
      Assert.Fail(AssertionMessageUtil.GetEqualityMessage((object) actual, (object) expected, true), message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual<T>(T expected, T actual) => Assert.AreNotEqual<T>(expected, actual, (string) null);

    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual<T>(T expected, T actual, string message) => Assert.AreNotEqual<T>(expected, actual, message, (IEqualityComparer<T>) EqualityComparer<T>.Default);

    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual<T>(
      T expected,
      T actual,
      string message,
      IEqualityComparer<T> comparer)
    {
      if (typeof (UnityEngine.Object).IsAssignableFrom(typeof (T)))
      {
        Assert.AreNotEqual((object) expected as UnityEngine.Object, (object) actual as UnityEngine.Object, message);
      }
      else
      {
        if (!comparer.Equals(actual, expected))
          return;
        Assert.Fail(AssertionMessageUtil.GetEqualityMessage((object) actual, (object) expected, false), message);
      }
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(UnityEngine.Object expected, UnityEngine.Object actual, string message)
    {
      if (!(actual == expected))
        return;
      Assert.Fail(AssertionMessageUtil.GetEqualityMessage((object) actual, (object) expected, false), message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNull<T>(T value) where T : class => Assert.IsNull<T>(value, (string) null);

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNull<T>(T value, string message) where T : class
    {
      if (typeof (UnityEngine.Object).IsAssignableFrom(typeof (T)))
      {
        Assert.IsNull((object) value as UnityEngine.Object, message);
      }
      else
      {
        if ((object) value == null)
          return;
        Assert.Fail(AssertionMessageUtil.NullFailureMessage((object) value, true), message);
      }
    }

    /// <summary>
    ///   <para>Assert the value is null.</para>
    /// </summary>
    /// <param name="value">The Object or type being checked for.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNull(UnityEngine.Object value, string message)
    {
      if (!(value != (UnityEngine.Object) null))
        return;
      Assert.Fail(AssertionMessageUtil.NullFailureMessage((object) value, true), message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNotNull<T>(T value) where T : class => Assert.IsNotNull<T>(value, (string) null);

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNotNull<T>(T value, string message) where T : class
    {
      if (typeof (UnityEngine.Object).IsAssignableFrom(typeof (T)))
      {
        Assert.IsNotNull((object) value as UnityEngine.Object, message);
      }
      else
      {
        if ((object) value != null)
          return;
        Assert.Fail(AssertionMessageUtil.NullFailureMessage((object) value, false), message);
      }
    }

    /// <summary>
    ///   <para>Assert that the value is not null.</para>
    /// </summary>
    /// <param name="value">The Object or type being checked for.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNotNull(UnityEngine.Object value, string message)
    {
      if (!(value == (UnityEngine.Object) null))
        return;
      Assert.Fail(AssertionMessageUtil.NullFailureMessage((object) value, false), message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(sbyte expected, sbyte actual)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<sbyte>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(sbyte expected, sbyte actual, string message)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<sbyte>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(sbyte expected, sbyte actual)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<sbyte>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(sbyte expected, sbyte actual, string message)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<sbyte>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(byte expected, byte actual)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<byte>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(byte expected, byte actual, string message)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<byte>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(byte expected, byte actual)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<byte>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(byte expected, byte actual, string message)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<byte>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(char expected, char actual)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<char>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(char expected, char actual, string message)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<char>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(char expected, char actual)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<char>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(char expected, char actual, string message)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<char>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(short expected, short actual)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<short>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(short expected, short actual, string message)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<short>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(short expected, short actual)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<short>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(short expected, short actual, string message)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<short>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(ushort expected, ushort actual)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<ushort>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(ushort expected, ushort actual, string message)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<ushort>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(ushort expected, ushort actual)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<ushort>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(ushort expected, ushort actual, string message)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<ushort>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(int expected, int actual)
    {
      if (expected == actual)
        return;
      Assert.AreEqual<int>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(int expected, int actual, string message)
    {
      if (expected == actual)
        return;
      Assert.AreEqual<int>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(int expected, int actual)
    {
      if (expected != actual)
        return;
      Assert.AreNotEqual<int>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(int expected, int actual, string message)
    {
      if (expected != actual)
        return;
      Assert.AreNotEqual<int>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(uint expected, uint actual)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<uint>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(uint expected, uint actual, string message)
    {
      if ((int) expected == (int) actual)
        return;
      Assert.AreEqual<uint>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(uint expected, uint actual)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<uint>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(uint expected, uint actual, string message)
    {
      if ((int) expected != (int) actual)
        return;
      Assert.AreNotEqual<uint>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(long expected, long actual)
    {
      if (expected == actual)
        return;
      Assert.AreEqual<long>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(long expected, long actual, string message)
    {
      if (expected == actual)
        return;
      Assert.AreEqual<long>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(long expected, long actual)
    {
      if (expected != actual)
        return;
      Assert.AreNotEqual<long>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(long expected, long actual, string message)
    {
      if (expected != actual)
        return;
      Assert.AreNotEqual<long>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(ulong expected, ulong actual)
    {
      if ((long) expected == (long) actual)
        return;
      Assert.AreEqual<ulong>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreEqual(ulong expected, ulong actual, string message)
    {
      if ((long) expected == (long) actual)
        return;
      Assert.AreEqual<ulong>(expected, actual, message);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(ulong expected, ulong actual)
    {
      if ((long) expected != (long) actual)
        return;
      Assert.AreNotEqual<ulong>(expected, actual, (string) null);
    }

    /// <summary>
    ///   <para>Assert that the values are not equal.</para>
    /// </summary>
    /// <param name="expected">The assumed Assert value.</param>
    /// <param name="actual">The exact Assert value.</param>
    /// <param name="message">The string used to describe the Assert.</param>
    /// <param name="comparer">Method to compare expected and actual arguments have the same value.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void AreNotEqual(ulong expected, ulong actual, string message)
    {
      if ((long) expected != (long) actual)
        return;
      Assert.AreNotEqual<ulong>(expected, actual, message);
    }
  }
}
