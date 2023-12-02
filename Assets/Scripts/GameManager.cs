using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    Dictionary<int,PlayerController> playersDic = new Dictionary<int,PlayerController>();
}
