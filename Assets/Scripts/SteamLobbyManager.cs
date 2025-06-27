using UnityEngine;
using Steamworks;
using FishNet.Managing;

public class SteamLobbyManager : MonoBehaviour
{
    private Callback<LobbyCreated_t> _lobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
    private Callback<LobbyEnter_t> _lobbyEntered;

    private const int MaxPlayers = 2;

    private void Start()
    {
        Debug.Log("SteamLobbyManager Start() called");

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }

        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        if (GameManager.Instance != null)
            Debug.Log("IsMultiplayer = " + GameManager.Instance.IsMultiplayer);
        // Auto-host if user is entering GameScene for Steam Friends Play
        if (GameManager.Instance != null && GameManager.Instance.IsMultiplayer)
        {
            Debug.Log("Detected multiplayer mode. Hosting Steam lobby...");

            HostLobby();
        }
    }

    public void HostLobby()
    {
        Debug.Log("Hosting Steam lobby...");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, MaxPlayers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby creation failed.");
            return;
        }

        Debug.Log("Lobby created. Invite a friend via Steam Overlay.");
        FindFirstObjectByType<NetworkManager>().ServerManager.StartConnection();
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t result)
    {
        Debug.Log("Friend invite received. Joining...");
        SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        Debug.Log("Lobby entered. Starting client connection...");
        FindFirstObjectByType<NetworkManager>().ClientManager.StartConnection();

    }
}
