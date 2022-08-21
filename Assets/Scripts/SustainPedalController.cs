using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SustainPedalController : MonoBehaviour
{
    public bool onPedalDown;
    public Material defaultMaterial;
    public Material triggeredMaterial;

    private string[] keyNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
    private GameObject piano;
    private GameObject blackKeys;
    private GameObject whiteKeys;
    private GameObject[] totalKeys = new GameObject[88];//所有的按键
    private AudioSource[] keysAudioSource = new AudioSource[88];//所有按键的AudiioSource组件
    private KeyController[] keyControllers = new KeyController[88];//所有按键的KeyController脚本组件
    // Start is called before the first frame update
    void Start()
    {
        onPedalDown = false;
        piano = GameObject.Find("Piano");
        blackKeys = piano.transform.Find("BlackKeys").gameObject;
        whiteKeys = piano.transform.Find("WhiteKeys").gameObject;
        totalKeys[0] = whiteKeys.transform.Find("A0").gameObject;
        totalKeys[1] = blackKeys.transform.Find("A#0").gameObject;
        totalKeys[2] = whiteKeys.transform.Find("B0").gameObject;

        for (int i = 3; i < 88; i++)
        {
            int octave = (i - 3) / 12 + 1;//八度数量
            int index = (i - 3) % 12;//键在一个八度内的序号（0到11）
            string curKeyName = System.String.Concat(keyNames[index], octave.ToString());
            if (index == 1 || index == 3 || index == 6 || index == 8 || index == 10)//如果是黑键
            {
                totalKeys[i] = blackKeys.transform.Find(curKeyName).gameObject;
            }
            else//白键
            {
                totalKeys[i] = whiteKeys.transform.Find(curKeyName).gameObject;
            }
        }
        for(int i = 0;i<88;i++)
        {
            keysAudioSource[i] = totalKeys[i].GetComponent<AudioSource>();
            keyControllers[i] = totalKeys[i].GetComponent<KeyController>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sustainPedalSwitched()
    {
        onPedalDown = !onPedalDown;
        GetComponent<Renderer>().material = onPedalDown == true ? triggeredMaterial : defaultMaterial;
        if (onPedalDown == false)//踏板放下时停止所有声音
        {
            for (int i = 0; i < 88; i++)
            {
                if (keyControllers[i].isTriggered == false && keysAudioSource[i].isPlaying == true)
                // 如果某个琴键没有被按下却仍然在发声，则松开踏板时将停止发声
                {
                    keysAudioSource[i].volume = 0.0f;
                    // keysAudioSource[i].Stop(); //不使用Stop()函数的原因：Stop()函数会产生音频停止时的杂音
                }
            }
        }
    }
}
