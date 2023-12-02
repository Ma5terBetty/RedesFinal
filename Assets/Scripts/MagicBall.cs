using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MagicBall : MonoBehaviourPun
{
    [SerializeField] private float _speed;
    public ServerManager _server;
    public Player _owner;

    public Player Owner => _owner;

    private void Start()
    {
        
    }

    void SetMagicBall()
    { 
    
    }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherClient = other.GetComponent<Character>();

        if (otherClient == null) return;

        if (otherClient != _server.PlayersDic[_owner])
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
