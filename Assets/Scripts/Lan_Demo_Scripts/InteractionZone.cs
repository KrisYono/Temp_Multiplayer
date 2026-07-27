using Unity.Netcode;
using UnityEngine;

public class InteractionZone : NetworkBehaviour
{
    public QTEManager qteManager;  // 引用 QTEManager

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("An object entered the trigger zone.");

        // 检查是否是玩家进入区域
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone.");

            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                Debug.Log("NetworkObject found on player.");

                // 检查玩家是否是客户端拥有者且非 Host
                if (networkObject.IsOwner && IsClient)
                {
                    Debug.Log("Client entered interaction zone - Ready for QTE interaction.");
                    TriggerClientInteraction();
                }
                else
                {
                    Debug.Log("The player is not the owner or is on the Host.");
                }
            }
            else
            {
                Debug.Log("No NetworkObject component found on the player.");
            }
        }
    }

    //[ClientRpc]
    private void TriggerClientInteraction()
    {
        Debug.Log("TriggerClientInteractionClientRpc isOwener   "+ IsOwner + " ");
        // 仅在客户端拥有者上调用 QTE
        if (IsServer) return;

        Debug.Log("  called on client.");

        if (qteManager != null)
        {
            Debug.Log("Starting QTE through QTEManager.");
            qteManager.StartQTE();
        }
        else
        {
            Debug.LogError("QTE Manager is not assigned in InteractionZone!");
        }
    }
}