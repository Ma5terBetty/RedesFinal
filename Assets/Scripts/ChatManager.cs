using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    #region PROPERTIES
    public TextMeshProUGUI content;
    public TMP_InputField inputField;
    private ChatClient _chatClient;
    private Dictionary<string, int> _chatDic = new Dictionary<string, int>();
    public CharacterControl characterControl;
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
        inputField.Select();
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
        print("Chat Update");
        inputField.ActivateInputField();
    }

    public void SendMessageToChat()
    {
        if (string.IsNullOrEmpty(inputField.text) || string.IsNullOrWhiteSpace(inputField.text)) return;
        CheckTextForCommand(inputField.text);
        _chatClient.PublishMessage(_channels[_currentChat], inputField.text);
        inputField.text = "";
    }

    public void CheckTextForCommand(string input)
    {
        input.ToLower();

        if (PhotonNetwork.IsMasterClient)
        {
            //Comandos Server
        }
        else
        {
            switch (input)
            {
                case "fireball":
                    //characterControl.Attack();
                    print(PhotonNetwork.LocalPlayer.NickName + "Ataque!");
                    break;

                case "mp":
                    characterControl.MoveUp();
                    break;

                case "md":
                    characterControl.MoveDown();
                    break;

                case "ml":
                    characterControl.MoveLeft();
                    break;

                case "mr":
                    characterControl.MoveRight();
                    break;

                case "rl":
                    characterControl.RotateLeft();
                    break;

                case "rr":
                    characterControl.RotateRight();
                    break;

                case "heal":
                    print(PhotonNetwork.LocalPlayer.NickName + "Curarse");
                    break;
            }
        }
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
        print("Client Connected to Photon");
    }

    public void OnDisconnected()
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            int indexChat = _chatDic[channelName];
            _chats[indexChat] += senders[i] + ":" + messages[i] + "\n";
        }
        UpdateChatUI();
        print("Get Message");
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
        print("Suscribed to Channel");
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {
        print("User Suscribed");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }
    #endregion
}
