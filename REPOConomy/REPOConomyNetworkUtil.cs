using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace REPOConomyMod
{
    public class REPOConomyNetworkHandler : MonoBehaviour
    {
        private const byte ECONOMY_SYNC_EVENT = 179;

        private void Start()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void OnEvent(EventData evt)
        {
            if (evt.Code != ECONOMY_SYNC_EVENT)
                return;

            try
            {
                var data = (object[])evt.CustomData;
                if (data.Length < 7)
                {
                    REPOConomyPlugin.Logger.LogWarning("[REPOConomy] Invalid economy event data.");
                    return;
                }

                REPOConomyPlugin.CurrentEconomy = (REPOConomyPlugin.EconomyType)(int)data[0];
                REPOConomyPlugin.CurrentEconomyName = (string)data[1];
                REPOConomyPlugin.CurrentEconomyFlavor = (string)data[2];
                REPOConomyPlugin.MinPercent = (float)data[3];
                REPOConomyPlugin.MaxPercent = (float)data[4];
                REPOConomyPlugin.ClientDeltaMin = (float)data[5];
                REPOConomyPlugin.ClientDeltaMax = (float)data[6];
                REPOConomyPlugin.HasEconomy = true;
            }
            catch (Exception ex)
            {
                REPOConomyPlugin.Logger.LogError($"[REPOConomy] Handler.OnEvent: {ex}");
            }
        }
    }
}
