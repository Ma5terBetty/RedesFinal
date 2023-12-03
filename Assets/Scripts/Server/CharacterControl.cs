using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private ServerManager _server;
    private Player _localClient;

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void Attack()
    {
        _server.RPC("RequestAttack", _localClient);
    }

    public void MoveUp()
    {
        _server.RPC("RequestMove", _localClient, new Vector3(0, 0, 0.5f));
    }

    public void MoveDown()
    {
        _server.RPC("RequestMove", _localClient, new Vector3(0, 0, -0.5f));
    }

    public void MoveLeft()
    {
        _server.RPC("RequestMove", _localClient, new Vector3(-0.5f, 0, 0));
    }

    public void MoveRight()
    {
        _server.RPC("RequestMove", _localClient, new Vector3(0.5f, 0, 0));
    }

    public void RotateLeft()
    {
        _server.RPC("RequestRotation", _localClient, -45f);
    }

    public void RotateRight()
    {
        _server.RPC("RequestRotation", _localClient, 45f);
    }

    public void Heal()
    {
        _server.RPC("RequestHealing", _localClient);
    }

    public void Damage()
    {
        _server.RPC("RequestDamage", _localClient);
    }
}
