using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Voice.Unity;

public class MicManager : MonoBehaviour
{
    public TMP_Dropdown dropDown;
    public Recorder recorder;

    private void Start()
    {
        List<string> list = new List<string>();
        string[] devices = Microphone.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            list.Add(devices[i]);
        }
        dropDown.AddOptions(list);
    }

    public void SetMicrophoneDevice(int i)
    {
        recorder.UnityMicrophoneDevice = Microphone.devices[i];
    }
}
