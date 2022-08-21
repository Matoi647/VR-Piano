using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoEmulator : MonoBehaviour
{
    private string[] keyNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    private GameObject[] totalKeys;//所有的按键
    private GameObject[] availableKeys;//电脑键盘上可用的按键 aws drftg hujikol ;
    private KeyController[] availableKeyControllers;//可用按键的控制脚本
    private int availableKeyMinGlobalIndex = 36;//可用按键中最左边按键在所有按键中的索引，默认为36
    private int availableKeyMaxGlobalIndex = 51;//可用按键中最左边按键在所有按键中的索引，默认为51
    private GameObject piano;
    private GameObject blackKeys;
    private GameObject whiteKeys;
    private KeyCode[] myKeyCode =
    {
        KeyCode.A, KeyCode.W, KeyCode.S,
        KeyCode.D, KeyCode.R, KeyCode.F, KeyCode.T, KeyCode.G,
        KeyCode.H, KeyCode.U, KeyCode.J, KeyCode.I, KeyCode.K, KeyCode.O, KeyCode.L,
        KeyCode.Semicolon
    };
    private GameObject sustainPedal;
    private SustainPedalController sustainPedalController;
    // private GameObject virtualFingerAsTrigger;//用于电脑按键模拟的手指
    // private BoxCollider virtualFingerCollider;//虚拟手指的碰撞体

    // Start is called before the first frame update
    void Start()
    {
        totalKeys = new GameObject[88];
        availableKeys = new GameObject[16];
        availableKeyControllers = new KeyController[16];

        piano = GameObject.Find("Piano");
        blackKeys = piano.transform.Find("BlackKeys").gameObject;
        whiteKeys = piano.transform.Find("WhiteKeys").gameObject;
        // virtualFingerAsTrigger = GameObject.Find("VirtualFingerAsTrigger");
        // virtualFingerCollider = virtualFingerAsTrigger.GetComponent<BoxCollider>();
        //获取按键
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

        for (int i = 0; i < 16; i++)//初始化可用按键
        {
            availableKeys[i] = totalKeys[i + availableKeyMinGlobalIndex];//默认为A3到B4
            availableKeyControllers[i] = availableKeys[i].GetComponent<KeyController>();
        }

        sustainPedal = GameObject.Find("SustainPedal");
        sustainPedalController = sustainPedal.GetComponent<SustainPedalController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Z,X键移动八度
        if(Input.GetKeyDown(KeyCode.Z))//Z键降低一个八度
        {
            if(availableKeyMinGlobalIndex > 0)//如果可用按键最左边的索引大于0则可以降低八度
            {
                availableKeyMinGlobalIndex -= 12;
                availableKeyMaxGlobalIndex -= 12;
                // Debug.Log("Octave Down");
                for (int i = 0;i<16;i++)
                {
                    availableKeys[i] = totalKeys[i + availableKeyMinGlobalIndex];
                    availableKeyControllers[i] = availableKeys[i].GetComponent<KeyController>();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))//X键升高一个八度
        {
            if (availableKeyMaxGlobalIndex < 87)//如果可用按键最右边的索引小于87则可以升高八度
            {
                availableKeyMinGlobalIndex += 12;
                availableKeyMaxGlobalIndex += 12;
                // Debug.Log("Octave Up");
                for (int i = 0; i < 16; i++)
                {
                    availableKeys[i] = totalKeys[i + availableKeyMinGlobalIndex];
                    availableKeyControllers[i] = availableKeys[i].GetComponent<KeyController>();
                }
            }
        }

        // // 电脑按键模拟手指碰撞琴键
        // 直接调用keyTriggered函数，而不是通过虚拟的手指碰撞
        for (int i = 0; i < 16; i++)
        {
            if (Input.GetKeyDown(myKeyCode[i]))
            {
                // availableKeys[i].GetComponent<KeyController>().OnTriggerEnter(virtualFingerCollider);
                // Debug.Log(myKeyCode[i] + " down");
                availableKeyControllers[i].keyTriggered();
            }
            if (Input.GetKeyUp(myKeyCode[i]))
            {
                // availableKeys[i].GetComponent<KeyController>().OnTriggerExit(virtualFingerCollider);
                availableKeyControllers[i].keyReleased();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Space))//通过电脑键盘模拟延音踏板，按下P键会切换踏板状态
        {
            sustainPedalController.sustainPedalSwitched();
            // Debug.Log("Pedal Switched");
        }
    }
}
