using UnityEngine;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;

public class MultiplayerGameStarter : MonoBehaviour
{
    [SerializeField] private NetworkObject boardPrefab;
    private bool boardSpawned = false;

    private void Start()
    {
        NetworkManager manager = FindFirstObjectByType<NetworkManager>();

        manager.ServerManager.OnRemoteConnectionState += (conn, args) =>
        {
            if (args.ConnectionState == RemoteConnectionState.Started && !boardSpawned)
            {
                boardSpawned = true;
                Debug.Log("Second player joined. Spawning board...");
                NetworkObject board = Instantiate(boardPrefab, Vector3.zero, Quaternion.identity);
                manager.ServerManager.Spawn(board);
            }
        };
    }
}
