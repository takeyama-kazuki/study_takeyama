using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System.Runtime.InteropServices;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
//using Microsoft.MixedReality.Toolkit.SpatialAwareness;
//using Microsoft.MixedReality.Toolkit;
//using UnityEngine.UIElements;//�X���C�_�[���g����UI��UIE�̊Ԃ��D�D�D�̂Ƃ����ꏑ���Ă���
/// <summary>
/// This class detects the HoloLens 2's HandTracking joints.
/// </summary>
/// 
//hand�̃G�t�F�N�g�͂��ׂēo�^���Ȃ��Ƃ����[�ɂȂ�
//mesheffect�̃I�u�W�F�N�g��o�^���Ȃ��ƃG���[�ɂȂ�
//effect[0]��Effect1�͓����Ӗ�
//_EmissionModule = new ParticleSystem.EmissionModule[] { aaa, bbb };����Ȃ��ƃG���[����������
public class Hand : MonoBehaviour
{

    int i = 0;
    int j = 0;
    int k = 0;
    int y = 0;
    float s = 0;
    int max = 7;//none,eff1,eff2,eff3,eff4��3��
    public List<Material> _Material;
    public List<EmissionModule> _EmissionModule;
    public ParticleSystem.MinMaxCurve[] Emissionkeep;
    public ParticleSystem.MinMaxCurve[] Emissiondiskeep;
    public ParticleSystem[] _ParticleSystems;
    public Renderer[] _Renderer;
    public GameObject Effect;
    public GameObject[] effect;
    public GameObject[] wallpeace;
    public GameObject wall;
    public Vector3[] _initialPosition; // �����ʒu
    private bool appear = true;
    private bool flag= true;
    private MixedRealityPose posekeep;



    [SerializeField, HeaderAttribute("DetectTargetHand")]
    HandMode HandDetectMode;

    Handedness handednesstype;

    enum HandMode
    {
        RightHand,
        LeftHand,
        BothHand,
    }

    //�{�^���̃C�x���g�g���K�[(pointerup)�ɂ���
    public void OnClickUp()
    {
        effect[i].SetActive(false);
        i++;
        if (i > max)
            i = 0;
        effect[i].SetActive(true);
        _EmissionModule = new List<EmissionModule>();
        _Material = new List<Material>();
        _ParticleSystems = effect[i].GetComponentsInChildren<ParticleSystem>();
        _Renderer = effect[i].GetComponentsInChildren<Renderer>();
        j = _ParticleSystems.Length;
        for (k = 0; k < j; k++)
        {
            _EmissionModule.Add(_ParticleSystems[k].emission);
            ParticleSystem.EmissionModule[] emissionModules = _EmissionModule.ToArray();
            Emissionkeep[k] = emissionModules[k].rateOverTime;
            Emissiondiskeep[k]= emissionModules[k].rateOverDistance;
        }
    }
    //�{�^���̃C�x���g�g���K�[(�R���|�[�l���g�ǉ�)(pointerdown)�ɂ���@�V�[����hand(���Ă�I�u�W�F�N�g��)�ɂ���
    public void OnClikDown()
    {
        effect[i].SetActive(false);
        i--;
        if (i < 0)
            i = max;
        effect[i].SetActive(true);
        _EmissionModule = new List<ParticleSystem.EmissionModule>();
        _Material = new List<Material>();
        _ParticleSystems = effect[i].GetComponentsInChildren<ParticleSystem>();
        _Renderer = effect[i].GetComponentsInChildren<Renderer>();
        j = _ParticleSystems.Length;
        for (k = 0; k < j; k++)
        {
            _EmissionModule.Add(_ParticleSystems[k].emission);
            ParticleSystem.EmissionModule[] emissionModules = _EmissionModule.ToArray();
            Emissionkeep[k] = emissionModules[k].rateOverTime;
            Emissiondiskeep[k] = emissionModules[k].rateOverDistance;
        }

    }

    public void Wallreverse()
    {
        for (y = 0; y < 16; y++)
        {
            wallpeace[y].transform.position = _initialPosition[y]; // �ʒu�̏�����
            wallpeace[y].transform.rotation = Quaternion.Euler(0,0,0); // ��]�̏�����
        }
    }

    public void Wallappear()
    {
        if (appear==false)
        { appear = true; }
        else if (appear==true)
        { appear = false; }
        wall.SetActive(appear);
    }

    public void onof()
    {

        if (flag == false)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }
        effect[i].SetActive(flag);
    }

    void Start()
    { 
       
        for (y = 0; y < 16; y++)
        {
            _initialPosition[y] = wallpeace[y].transform.position;
        }

        if ((int)HandDetectMode == 0)
        {
            handednesstype = Handedness.Right;
        }
    
        //�ŏ��ɃG�t�F�N�g����
        foreach (GameObject effects in effect)
        {
            effects.SetActive(false);
        }
        effect[0].SetActive(true);//none��on�ɂ���

    }

    // Update is called once per frame
    void Update()
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //��񌟒m
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, handednesstype, out MixedRealityPose pose1))
        {
            posekeep = pose1;
        }
        //�l�����w���m
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, handednesstype, out MixedRealityPose pose2))
        {

        }
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexDistalJoint, handednesstype, out MixedRealityPose pose3))
        {
           
        }

        //x,y,z�͋����̍�
        float x = pose3.Position.x - pose1.Position.x;
        float y = pose3.Position.y - pose1.Position.y;
        float z = pose3.Position.z - pose1.Position.z;
        float dis = x * x + y * y + z * z;//���Ɛl�����w�̍����o
       

        //�����̍��������Ȃ�Ɣ���
        if (dis < 0.018)
        {
            for (k = 0; k < j; k++)
            {
                ParticleSystem.EmissionModule[] emissionModules = _EmissionModule.ToArray();
                emissionModules[k].rateOverTime = Emissionkeep[k];
                emissionModules[k].rateOverDistance = Emissiondiskeep[k];
            }
        }
        if (dis > 0.018 || dis == 0)
        {
            for (k = 0; k < j; k++)
            {
                _EmissionModule.Add(_ParticleSystems[k].emission);
                ParticleSystem.EmissionModule[] emissionModules = _EmissionModule.ToArray();
                if (posekeep != pose1&&dis!=0)
                {
                    emissionModules[k].rateOverTime = Emissionkeep[k];
                    emissionModules[k].rateOverDistance = Emissiondiskeep[k];
                }
                else
                {
                    emissionModules[k].rateOverTime = 0;
                    emissionModules[k].rateOverDistance = 0;
                }
            }
        }
        Effect.transform.position = posekeep.Position;
        Effect.transform.rotation = posekeep.Rotation;
    }
}

//hand��particle�V�X�e��������effect1����22�܂ł�particle�V�X�e����t���Ȃ��Ɠ����Ȃ�
//hand�ɂ��Ă���particle�����ĂȂ�����particle�̒l���擾�ł��Ȃ��Ƃ����G���[���o��
