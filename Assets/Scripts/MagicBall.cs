using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MagicBall : MonoBehaviourPun
{
    [SerializeField] private float _speed;
    public ServerManager _server;
    public Player _owner;

    public Player Owner => _owner;
    public float lifeSpawn = 5f;
    public float counter = 0;

    public string duenio;

    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= lifeSpawn)
        {
            _server.RPC("DestroyBall", _owner, this.gameObject);
        }

        transform.position += transform.forward * Time.deltaTime * _speed;
    }

    [PunRPC]
    private void SetOwner(Player client)
    {
        if (PhotonNetwork.LocalPlayer == client)
        {
            _owner = client;
            duenio = _owner.NickName;
        }
    }
}
