using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public Material defaultMaterial;
    public Material triggeredMaterial;
    [Range(0,100)] public float velocity;//设置范围，在Inspector中显示滑动条，默认为80
    public bool isTriggered;//被按下的状态

    private AudioSource my_AudioSource;
    private AudioClip my_AudioClip;
    private GameObject sustainPedal;
    private SustainPedalController sustainPedalController;
    private float outputVolume;//由AudioSource.volume和velocity相乘得出最终发出的声音

    // Start is called before the first frame update
    void Start()
    {
        velocity = 80.0f;
        my_AudioSource = GetComponent<AudioSource>();
        my_AudioClip = my_AudioSource.clip;
        sustainPedal = GameObject.Find("SustainPedal");//提前获取延音踏板，防止运行时调用Find函数导致运行速度降低
        sustainPedalController = sustainPedal.GetComponent<SustainPedalController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finger")
        {
            keyTriggered();
        }   
    }
    public void OnTriggerStay(Collider other)
    {
        
    }
    public void OnTriggerExit(Collider other)
    {
        keyReleased();
    }

    public void keyTriggered()
    {
        isTriggered = true;
        GetComponent<Renderer>().material = triggeredMaterial;
        my_AudioSource.volume = 1.0f;//按下琴键时重新设置音量
        outputVolume = my_AudioSource.volume * velocity / 100.0f;
        my_AudioSource.PlayOneShot(my_AudioClip, outputVolume);//速度从0到100
    }

    public void keyReleased()
    {
        isTriggered = false;
        GetComponent<Renderer>().material = defaultMaterial;
        if (sustainPedalController.onPedalDown != true)
        // 如果没有按下延音踏板，某个琴键松开时就停止发声，否则一直播放音频直到结束
        {
            my_AudioSource.volume = 0.0f;//离开时将音量设置为0，停止发声
            // my_AudioSource.Stop();//不使用Stop()函数的原因：Stop()函数会产生音频停止时的
        }
    }
}
