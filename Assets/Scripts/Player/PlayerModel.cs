using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] int playerHealth;
    [SerializeField] string playerName;

    public int PlayerHealth => playerHealth;
    public string PlayerName => playerName;

}
