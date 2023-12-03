using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class NetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TextMeshProUGUI statusText;
    private void Start()
    {
        playButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "<color=orange>Conecting to server...</color>";
    }
    public override void OnConnectedToMaster()
    {
        statusText.text = "<color=green>Conected to Photon</color>";
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        statusText.text = "<color=green>Conected to Server</color>";
        playButton.interactable = true;
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
        statusText.text = $"<color=green>Joined to {PhotonNetwork.CurrentRoom.Name} room</color>";
        PhotonNetwork.LoadLevel("Gameplay");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = $"<color=red>Unable to join on {PhotonNetwork.CurrentRoom.Name} room</color>";
        playButton.interactable = true;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = $"<color=red>Unable to create {PhotonNetwork.CurrentRoom.Name} room</color>";
        playButton.interactable = true;
    }
}
