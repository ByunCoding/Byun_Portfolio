using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Stage_3 : Stage
{
    // 스테이지 3: 생성된 무기들을 지정된 곳에 가져다 놓기
    // 위치: 랜덤으로 생성, 지금 1인 원만큼만 빛 주기
    // 드랍존에 들어가면 무기 파괴

    public static Stage_3 Instance;

    public Transform _dropZone;
    private Vector3 _dropZonePos;

    private int _totalWeapons;
    private int _indropZoneWeaponCount = 0;

    private float _dropZoneRadius = 2.5f;

    private SphereCollider _dropZoneCollider;
    private Light _dropZoneLight;

    private GameObject[] _swords;
    private GameObject[] _rifles;

    public bool _isDropZone = false;
    private Collider _currentSword = null;

    protected override void Start()
    {
        base.Start();
        Instance = this;

        _weaponsSpawner.MakeSwords();
        _weaponsSpawner.MakeRifles();
        _enemiesSpawner.MixMakeMonsters();

        _swords = GameObject.FindGameObjectsWithTag("Sword");
        _rifles = GameObject.FindGameObjectsWithTag("Rifle");
        _totalWeapons = _swords.Length + _rifles.Length;
        _indropZoneWeaponCount = 0;
    }

    protected override void StartStage()
    {
        base.StartStage();

        Vector3 dropPos = MapManager.Instance.GetRandomPositionOnNavMesh();
        _dropZonePos = dropPos;

        if (_dropZone == null)
        {
            GameObject dropzoneObj = new GameObject("DropZone");
            _dropZone = dropzoneObj.transform;
            _dropZone.position = _dropZonePos;
        }

        _dropZoneCollider = _dropZone.GetComponent<SphereCollider>();
        if (_dropZoneCollider == null)
        {
            _dropZoneCollider = _dropZone.gameObject.AddComponent<SphereCollider>();
        }
        _dropZoneCollider.isTrigger = true;
        _dropZoneCollider.radius = _dropZoneRadius;
        _dropZoneCollider.center = Vector3.zero;

        Rigidbody rb = _dropZone.GetComponent<Rigidbody>();
        if (rb != null && Stage_3.Instance._isDropZone && Stage_3.Instance != null)
        {
            rb.isKinematic = true;
        }

        DropZoneTrigger dropZoneTrigger = _dropZone.gameObject.GetComponent<DropZoneTrigger>();
        if(dropZoneTrigger == null)
        {
            dropZoneTrigger = _dropZone.gameObject.AddComponent<DropZoneTrigger>();
        }
        dropZoneTrigger.stage_3 = this;

        GameObject lightObj = new GameObject("DropZoneLight");
        lightObj.transform.SetParent(_dropZone, false);
        lightObj.transform.localPosition = new Vector3(0, 10f, 0);
        lightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        Light dropzoneLight = lightObj.AddComponent<Light>();
        dropzoneLight.type = LightType.Spot;
        dropzoneLight.spotAngle = 30f;
        dropzoneLight.range = 15f;
        dropzoneLight.intensity = 20f;
        dropzoneLight.color = Color.white;
    }

    protected override bool CheckClear()
    {
        return _indropZoneWeaponCount >= _totalWeapons;
    }

    protected override void EndStage()
    {
        base.EndStage();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Do()
    {
    }

    public void DropZoneWeaponCount()
    {
        _indropZoneWeaponCount++;
        _isDropZone = false;

        if (CheckClear())
        {
            EndStage();
        }
    }

    //public void DropZoneTrigger(Collider other)
    //{
    //    if (!_isDropZone) return;

    //    Collider[] colliders = Physics.OverlapSphere(other.transform.position, 0.5f);
    //    bool isGround = false;
    //    foreach(Collider col in colliders)
    //    {
    //        if (col.CompareTag("Ground"))
    //        {
    //            isGround = true;
    //            break;
    //        }
    //    }

    //    if (isGround)
    //    {
    //        other.enabled = false;

    //        _indropZoneWeaponCount++;
    //        _isDropZone = false;

    //        if (CheckClear())
    //        {
    //            EndStage();
    //        }

    //        Destroy(other.gameObject, 1.5f);
    //    }
    //}
}

        //if (_isDropZone)
        //{
        //    Collider[] colliders = Physics.OverlapSphere(_dropZone.position, _dropZoneRadius);
        //    foreach(Collider col in colliders)
        //    {
        //        if (col.CompareTag("Sword"))
        //        {
        //            Collider[] groundColliders = Physics.OverlapSphere(col.transform.position, 0.05f);
        //            bool isGround = false;
        //            foreach(Collider ground in groundColliders)
        //            {
        //                if (ground.CompareTag("Ground"))
        //                {
        //                    isGround = true;
        //                    break;
        //                }
        //            }

        //            if (isGround)
        //            {
        //                col.enabled = false;

        //                _indropZoneWeaponCount++;
        //                _isDropZone = false;

        //                if (CheckClear())
        //                {
        //                    EndStage();
        //                }

        //                Destroy(col.gameObject, 1.5f);
        //                break;
        //            }
        //        }
        //    }
        //}