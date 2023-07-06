namespace Netcode.io.OLD
{
    /// <summary>
    /// Defines network update stages being executed by the network update loop.
    /// See for more details on update stages:
    /// https://docs.unity3d.com/ScriptReference/PlayerLoop.Initialization.html
    /// </summary>
    public enum NetworkUpdateStage : byte
    {
        /// <summary>
        /// Invoked before Fixed update
        /// </summary>
        EarlyUpdate = 2,
        /// <summary>
        /// Updated before the Monobehaviour.Update for all components is invoked
        /// </summary>
        PreUpdate = 4,
        /// <summary>
        /// Updated after the Monobehaviour.LateUpdate for all components is invoked
        /// </summary>
        PostLateUpdate = 7
    }
}
