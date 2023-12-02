using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallControl : MonoBehaviourPun
{
    BallMovement _ballMovement;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(this);
            return;
        }
        _ballMovement = GetComponent<BallMovement>();
    }

    private void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        _ballMovement.Move(dir.normalized);
    }
}
