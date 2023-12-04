using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Material[] _wizardColors;
    [SerializeField] private SkinnedMeshRenderer _skinedMeshRenderer;

    int _materialIndex = 0;

    private void Awake()
    {
        _skinedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
}
