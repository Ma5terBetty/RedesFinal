using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviourPun //DEPRECADO
{
    [SerializeField] Transform _markedPoint;
    NavMeshAgent selfAgent;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(this);
            return;
        }

        selfAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                selfAgent.destination = hit.point;
            }
        }
    }

    void MoveToMarkedPoint()
    { 
    
    }
}
