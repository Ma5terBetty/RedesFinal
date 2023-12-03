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

        if (devices.Length != 0)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                list.Add(devices[i]);
            }
            dropDown.AddOptions(list);
            SetMicrophoneDevice(0);
        }
    }

    public void SetMicrophoneDevice(int i)
    {
        string[] devices = Microphone.devices;
        if (Microphone.devices.Length > i)
        {
            recorder.UnityMicrophoneDevice = devices[i];
        }
    }
}
