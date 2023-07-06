namespace Netcode.io.OLD.Unity.Collections
{
    /// <summary>
    /// An interface for a sequence of UTF-8 encoded text.
    /// </summary>
    public interface IUTF8Bytes
    {
        /// <summary>
        /// Whether this IUTF8Bytes is empty.
        /// </summary>
        /// <value>True if this IUTF8Bytes is empty.</value>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns a pointer to the content of this IUTF8Bytes.
        /// </summary>
        /// <remarks>The pointer may point to stack memory.</remarks>
        /// <returns>A pointer to the content of this IUTF8Bytes.</returns>
        unsafe byte* GetUnsafePtr();
    }
}
