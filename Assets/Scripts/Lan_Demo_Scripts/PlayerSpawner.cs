using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public Transform hostSpawnPoint;       // 主机生成点
    public Transform clientSpawnPoint;     // 客户端生成点
    public GameObject playerPrefab;        // 玩家预设

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (IsHost)
            {
                // 主机直接在主机生成点生成角色
                SpawnPlayerAtPosition(hostSpawnPoint.position);
            }
            else
            {
                // 客户端请求服务器生成角色
                RequestSpawnPlayerServerRpc();
            }
        }
    }

    [ServerRpc]
    private void RequestSpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        // 确保服务器在客户端生成点生成角色
        GameObject playerInstance = Instantiate(playerPrefab, clientSpawnPoint.position, Quaternion.identity);

        // 将所有权赋予请求的客户端
        playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }

    private void SpawnPlayerAtPosition(Vector3 position)
    {
        // 本地生成玩家对象
        GameObject playerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.LocalClientId);
    }
}