using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.SceneManagement;
/// <summary>
/// This class detects the HoloLens 2's HandTracking joints.
/// </summary>
public class Hand1 : MonoBehaviour
{
    public int k = 0;
    public int count = 0;
    int i = 0;
    int max = 6;
    public float emissionFireMax = 30.0f;
    public float emissionzero = 0.0f;
    public ParticleSystem _ParticleSystem;
    public ParticleSystem.EmissionModule _EmissionModule;
    //public ParticleSystem.MinMaxCurve _MinMaxCurve;
    public int effectpower;
    // public ParticleSystem.Min _Min;
    public GameObject fireboostred;
    [SerializeField, HeaderAttribute("DetectTargetHand")]
    HandMode HandDetectMode;
    Handedness handednesstype;
    private bool check1 = false;
    public bool check2 = false;
    private bool onec = true;
    float dis=0;//手首と人差し指の差検出
    public  GameObject endbutton;
    public GameObject Text;


    enum HandMode
    {
        RightHand,
        LeftHand,
        BothHand,
    }

    //ボタンのイベントトリガー(pointerup)につける

    // Start is called before the first frame update
    void Start()
    {
        //_ParticleSystem = effect[i].GetComponent<ParticleSystem>();
        //DetectRighitHandWrist
        if ((int)HandDetectMode == 0)
        {
            handednesstype = Handedness.Right;
        }
        //DetectLeftHandWrist
        if ((int)HandDetectMode == 1)
        {
            handednesstype = Handedness.Left;
        }
        //DetectBothHandWrist
        if ((int)HandDetectMode == 2)
        {
            handednesstype = Handedness.Both;
        }
        Debug.Log(handednesstype);
        //最初にエフェクト消す

    }

    public void end()
    {
        SceneManager.LoadScene("SampleScene");
    }


    // Update is called once per frame
    void Update()
    {
        //手首検知
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, handednesstype, out MixedRealityPose pose1))
        {
            //Debug.Log("Detect");確認用
            // Debug.Log(pose1);
        }
        //人差し指検知
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, handednesstype, out MixedRealityPose pose2))
        {
            // Debug.Log("Detect1");確認用
            //Debug.Log(pose2);
        }
        //x,y,zは距離の差
        float x = pose1.Position.x - pose2.Position.x;
        float y = pose1.Position.y - pose2.Position.y;
        float z = pose1.Position.z - pose2.Position.z;
        dis = x * x + y * y + z * z;//手首と人差し指の差検出
        //Debug.Log(dis);

        //距離の差が狭くなると発生
        if (dis < 0.025)
        {
            // エミッションで可視化
            //定義
            _ParticleSystem = fireboostred.GetComponent<ParticleSystem>();
            _EmissionModule = _ParticleSystem.emission;
            // _MinMaxCurve = _EmissionModule.rateOverTime;
            //_Min = emissionzero;
            //定義終わり
            check1 = true;
            // Debug.Log(effectpower);
            //_MinMaxCurve = emissionFireMax;
             _EmissionModule.rateOverTime = effectpower;
        }
        if (dis > 0.025 || dis == 0)
        {
            //  effect[i].SetActive(false);
            //_MinMaxCurve = emissionzero;
            _EmissionModule.rateOverTime = 0;
            if (check1&&dis>0)
            {
                count = count + 1;
                check1 = false;
                if (count == 0) { effectpower = 5; }
                if (count == 2) { effectpower = 10; }
                if (count == 4) { effectpower = 20; }
                if (count == 6) { effectpower = 30; }
            }
        }
        fireboostred.transform.position = new Vector3(pose1.Position.x, pose1.Position.y, pose1.Position.z);
        fireboostred.transform.rotation = pose1.Rotation;
        if (count == 6) 
        {
            endbutton.SetActive(true);
            Text.SetActive(true);
        }
    }
       
}

