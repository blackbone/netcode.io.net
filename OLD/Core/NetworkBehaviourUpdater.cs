namespace Netcode.io.OLD
{
    /// <summary>
    /// An helper class that helps NetworkManager update NetworkBehaviours and replicate them down to connected clients.
    /// </summary>
    public class NetworkBehaviourUpdater
    {
        private HashSet<NetworkObject> m_DirtyNetworkObjects = new HashSet<NetworkObject>();

        internal void AddForUpdate(NetworkObject networkObject)
        {
            m_DirtyNetworkObjects.Add(networkObject);
        }

        internal void NetworkBehaviourUpdate(NetworkManager networkManager)
        {
            try
            {
                // NetworkObject references can become null, when hidden or despawned. Once NUll, there is no point
                // trying to process them, even if they were previously marked as dirty.
                m_DirtyNetworkObjects.RemoveWhere((sobj) => sobj == null);

                if (true)
                {
                    foreach (var dirtyObj in m_DirtyNetworkObjects)
                    {
                        for (int k = 0; k < dirtyObj.ChildNetworkBehaviours.Count; k++)
                        {
                            dirtyObj.ChildNetworkBehaviours[k].PreVariableUpdate();
                        }

                        for (int i = 0; i < networkManager.ConnectedClientsList.Count; i++)
                        {
                            var client = networkManager.ConnectedClientsList[i];

                            if (dirtyObj.IsNetworkVisibleTo(client.ClientId))
                            {
                                // Sync just the variables for just the objects this client sees
                                for (int k = 0; k < dirtyObj.ChildNetworkBehaviours.Count; k++)
                                {
                                    dirtyObj.ChildNetworkBehaviours[k].VariableUpdate(client.ClientId);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // when client updates the server, it tells it about all its objects
                    foreach (var sobj in m_DirtyNetworkObjects)
                    {
                        if (sobj.IsOwner)
                        {
                            for (int k = 0; k < sobj.ChildNetworkBehaviours.Count; k++)
                            {
                                sobj.ChildNetworkBehaviours[k].PreVariableUpdate();
                            }
                            for (int k = 0; k < sobj.ChildNetworkBehaviours.Count; k++)
                            {
                                sobj.ChildNetworkBehaviours[k].VariableUpdate(NetworkManager.ServerClientId);
                            }
                        }
                    }
                }

                foreach (var dirtyObj in m_DirtyNetworkObjects)
                {
                    for (int k = 0; k < dirtyObj.ChildNetworkBehaviours.Count; k++)
                    {
                        var behaviour = dirtyObj.ChildNetworkBehaviours[k];
                        for (int i = 0; i < behaviour.NetworkVariableFields.Count; i++)
                        {
                            if (behaviour.NetworkVariableFields[i].IsDirty() &&
                                !behaviour.NetworkVariableIndexesToResetSet.Contains(i))
                            {
                                behaviour.NetworkVariableIndexesToResetSet.Add(i);
                                behaviour.NetworkVariableIndexesToReset.Add(i);
                            }
                        }
                    }
                }
                // Now, reset all the no-longer-dirty variables
                foreach (var dirtyobj in m_DirtyNetworkObjects)
                {
                    dirtyobj.PostNetworkVariableWrite();
                }
                m_DirtyNetworkObjects.Clear();
            }
            finally
            {
            }
        }
    }
}
