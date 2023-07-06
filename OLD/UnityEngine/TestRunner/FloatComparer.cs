// Decompiled with JetBrains decompiler
// Type: UnityEngine.Assertions.Comparers.FloatComparer
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1390FDF6-EA82-4C60-86AD-FB60B8035622
// Assembly location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.xml

namespace Netcode.io.OLD.UnityEngine.TestRunner
{
  /// <summary>
  ///   <para>A float comparer used by Assertions.Assert performing approximate comparison.</para>
  /// </summary>
  public class FloatComparer : IEqualityComparer<float>
  {
    private readonly float m_Error;
    private readonly bool m_Relative;
    /// <summary>
    ///   <para>Default instance of a comparer class with deafult error epsilon and absolute error check.</para>
    /// </summary>
    public static readonly FloatComparer s_ComparerWithDefaultTolerance = new FloatComparer(1E-05f);
    /// <summary>
    ///   <para>Default epsilon used by the comparer.</para>
    /// </summary>
    public const float kEpsilon = 1E-05f;

    /// <summary>
    ///   <para>Creates an instance of the comparer.</para>
    /// </summary>
    /// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
    /// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
    public FloatComparer()
      : this(1E-05f, false)
    {
    }

    /// <summary>
    ///   <para>Creates an instance of the comparer.</para>
    /// </summary>
    /// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
    /// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
    public FloatComparer(bool relative)
      : this(1E-05f, relative)
    {
    }

    /// <summary>
    ///   <para>Creates an instance of the comparer.</para>
    /// </summary>
    /// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
    /// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
    public FloatComparer(float error)
      : this(error, false)
    {
    }

    /// <summary>
    ///   <para>Creates an instance of the comparer.</para>
    /// </summary>
    /// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
    /// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
    public FloatComparer(float error, bool relative)
    {
      this.m_Error = error;
      this.m_Relative = relative;
    }

    public bool Equals(float a, float b) => this.m_Relative ? FloatComparer.AreEqualRelative(a, b, this.m_Error) : FloatComparer.AreEqual(a, b, this.m_Error);

    public int GetHashCode(float obj) => this.GetHashCode();

    /// <summary>
    ///   <para>Performs equality check with absolute error check.</para>
    /// </summary>
    /// <param name="expected">Expected value.</param>
    /// <param name="actual">Actual value.</param>
    /// <param name="error">Comparison error.</param>
    /// <returns>
    ///   <para>Result of the comparison.</para>
    /// </returns>
    public static bool AreEqual(float expected, float actual, float error) => (double) Math.Abs(actual - expected) <= (double) error;

    /// <summary>
    ///   <para>Performs equality check with relative error check.</para>
    /// </summary>
    /// <param name="expected">Expected value.</param>
    /// <param name="actual">Actual value.</param>
    /// <param name="error">Comparison error.</param>
    /// <returns>
    ///   <para>Result of the comparison.</para>
    /// </returns>
    public static bool AreEqualRelative(float expected, float actual, float error)
    {
      if ((double) expected == (double) actual)
        return true;
      float num1 = Math.Abs(expected);
      float num2 = Math.Abs(actual);
      return (double) Math.Abs((float) (((double) actual - (double) expected) / ((double) num1 > (double) num2 ? (double) num1 : (double) num2))) <= (double) error;
    }
  }
}
