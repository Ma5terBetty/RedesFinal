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

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        { 
            Destroy(this);
        }
    }

    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= lifeSpawn)
        {
            _server.RPC("DestroyBall", _owner, this.gameObject);
        }

        transform.position += transform.forward * Time.deltaTime * _speed;
    }
}
