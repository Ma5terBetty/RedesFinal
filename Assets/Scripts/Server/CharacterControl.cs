using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private ServerManager _server;
    private Player _localClient;
    private bool WKey => Input.GetKey(KeyCode.W);
    private bool AKey => Input.GetKey(KeyCode.A);
    private bool SKey => Input.GetKey(KeyCode.S);
    private bool DKey => Input.GetKey(KeyCode.D);
    private bool LeftKey => Input.GetKey(KeyCode.LeftArrow);
    private bool RightKey => Input.GetKey(KeyCode.RightArrow);

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Destroy(this);
        }
        else
        {
            _localClient = PhotonNetwork.LocalPlayer;
        }
    }

    private void Start()
    {
        _server.photonView.RPC("RequestConnect", _server.GetServer, PhotonNetwork.LocalPlayer); //??
    }
    private void Update()
    {
        if (!_server.IsGameStarted) return;

        if (Input.GetKeyDown(KeyCode.Space)) _server.RPC("RequestAttack", _localClient);

        if (LeftKey && RightKey) return;

        if (LeftKey) _server.RPC("RequestRotation", _localClient, -1f);
        if (RightKey) _server.RPC("RequestRotation", _localClient, 1f);
    }
    private void FixedUpdate()
    {
        if (!_server.IsGameStarted) return;

        Vector3 direction = new Vector3();

        if (AKey) direction += new Vector3(-1, 0, 0);
        if (DKey) direction += new Vector3(1, 0, 0);
        if (WKey) direction += new Vector3(0, 0, 1);
        if (SKey) direction += new Vector3(0, 0, -1);

        _server.RPC("RequestMove", _localClient, direction);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
