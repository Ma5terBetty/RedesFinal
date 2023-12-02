using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button playButton;
    [SerializeField] private InputField playerName;
    [SerializeField] private InputField roomName;
    private void Start()
    {
        playButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        BLogger.Print(MType.P, "Connected To Master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        playButton.interactable = true;
        BLogger.Print(MType.P, "Connected To Lobby");
    }

    ///<summary>
    ///Intenta conectarse a un servidor. Si este no extiste, lo crea.
    ///</summary>
    public void Connect()
    {
        if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrWhiteSpace(roomName.text)) return;
        if (string.IsNullOrEmpty(playerName.text) || string.IsNullOrWhiteSpace(playerName.text)) return;

        PhotonNetwork.NickName = playerName.text;
        RoomOptions options = new RoomOptions();
        options.IsOpen = true;
        options.IsVisible = true;
        options.MaxPlayers = 5;

        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);
        playButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        BLogger.Print(MType.P, "Join Room Success!");
        PhotonNetwork.LoadLevel("Gameplay");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        playButton.interactable = true;
        BLogger.Print(MType.N, "Join Room Failed :(");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        playButton.interactable = true;
    }
}
