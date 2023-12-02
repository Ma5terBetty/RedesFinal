using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    #region PROPERTIES
    public TextMeshProUGUI content;
    public TMP_InputField inputField;
    private ChatClient _chatClient;
    private Dictionary<string, int> _chatDic = new Dictionary<string, int>();
    private string[] _channels;
    private string[] _chats;
    private int _currentChat;
    
    #endregion


    #region UNITY_FUNCTIONS
    void Start()
    {
        if (!PhotonNetwork.IsConnected) return;
        _channels = new string[] { "World", PhotonNetwork.CurrentRoom.Name };
        _chats = new string[2];
        _chatDic["World"] = 0;
        _chatDic[PhotonNetwork.CurrentRoom.Name] = 1;
        _chatClient = new ChatClient(this);
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
    }

    void Update()
    {
        _chatClient.Service();
    }
    #endregion

    #region CUSTOM_FUNCTIONS
    void UpdateChatUI()
    {
        content.text = _chats[_currentChat];
    }

    public void SendMessageToChat()
    {
        if (string.IsNullOrEmpty(inputField.text) || string.IsNullOrWhiteSpace(inputField.text)) return;
        _chatClient.PublishMessage(_channels[_currentChat], inputField.text);
    }
    #endregion


    #region PHOTON_CHAT_INTERFACE
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnConnected()
    {
        _chatClient.Subscribe(_channels);
        print("Hola chat");
    }

    public void OnDisconnected()
    {
        print("Chau chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            int indexChat = _chatDic[channelName];
            _chats[indexChat] += senders[i] + ":" + messages[i] + "\n";
        }
        UpdateChatUI();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
        for (int i = 0; i < channels.Length; i++)
        {
            _chats[0] += "<color=blue>" + "Suscribed to channel: " + channels[i] + "</color>" + "\n";
        }
        UpdateChatUI();
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }
    #endregion
}
