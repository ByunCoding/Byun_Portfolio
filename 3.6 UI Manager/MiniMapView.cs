using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapView : UIView
{
    public Transform _player;
    [SerializeField] public RectTransform _minimap;
    [SerializeField] public RectTransform _playermarker;
    [SerializeField] public GameObject _weaponMarkerPrefab;
    [SerializeField] public GameObject _potionMarkerPrefab;

    private float _minimapScale = 2f;
    private float _displayDistance = 30f;

    private List<Transform> _items = new List<Transform>();
    private Dictionary<Transform, GameObject> _itemMarkers = new Dictionary<Transform, GameObject>();

    private float _updateTimeInterval = 1f;
    private float _updateTimer = 0f;

    private Bounds _mapBounds;
    private Vector2 _mapOrigin;

    private void Start()
    {
        MiniMap();
        UpdateItemList();
    }

    void Update()
    {

        if (_player == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
               _player = player.transform;
            }
        }

        if (_playermarker != null)
        {
            _playermarker.anchoredPosition = Vector2.zero;
        }

        _updateTimer += Time.deltaTime;
        if(_updateTimer >= _updateTimeInterval)
        {
            _updateTimer = 0f;
            UpdateItemList();
        }

        if(_minimap != null)
        {
            _minimap.rotation = Quaternion.identity;
        }

        ItemPositionMarker();
    }
    
    private void MiniMap()
    {
        if (MapManager.Instance != null)
        {
            _mapBounds = MapManager.Instance.LandBounds;
            _mapOrigin = new Vector2(_mapBounds.center.x, _mapBounds.center.z);

            float scaleX = _minimap.rect.width / _mapBounds.size.x;
            float scaleZ = _minimap.rect.height / _mapBounds.size.z;
            _minimapScale = Mathf.Min(scaleX, scaleZ);
        }
    }

    private void UpdateItemList()
    {
        _items.Clear();

        GameObject[] swords = GameObject.FindGameObjectsWithTag("Sword");
        GameObject[] rifles = GameObject.FindGameObjectsWithTag("Rifle");
        GameObject[] potions = GameObject.FindGameObjectsWithTag("Potion");

        foreach (GameObject item in swords) _items.Add(item.transform);
        foreach (GameObject item in rifles) _items.Add(item.transform);
        foreach (GameObject item in potions) _items.Add(item.transform);
    }

    private void ItemPositionMarker()
    {
        if (_player == null || _minimap == null) return;

        foreach (var marker in _itemMarkers.Values)
        {
            if (marker != null)
            {
                marker.SetActive(false);
            }
        }

        foreach(Transform item in _items)
        {
            if (item == null) continue;

            if(item.parent == _player || item.IsChildOf(_player))
            {
                if (_itemMarkers.ContainsKey(item))
                {
                    Destroy(_itemMarkers[item]);
                    _itemMarkers.Remove(item);
                }
                continue;
            }

            // 플레이어와 아이템 사이 방향 벡터
            Vector3 diff = item.position - _player.position;
            Vector2 offset = new Vector2(diff.x, diff.z) * _minimapScale;

            // 거리 계산
            float distance = new Vector2(diff.x, diff.z).magnitude;

            // 표시 가능 범위 확인
            bool inRange = (distance <= _displayDistance);


            // 범위 밖이면 숨기기
            if (!inRange)
            {
                if (_itemMarkers.ContainsKey(item))
                {
                    _itemMarkers[item].SetActive(false);
                }

                continue;
            }

            float angle = _player.eulerAngles.y * Mathf.Deg2Rad;
            float rotateX = diff.x * Mathf.Cos(angle) - diff.z * Mathf.Sin(angle);
            float rotateZ = diff.x * Mathf.Sin(angle) + diff.z * Mathf.Cos(angle);
            offset = new Vector2(rotateX, rotateZ) * _minimapScale;

            // 마커 없으면 생성
            if (!_itemMarkers.ContainsKey(item))
            {
                GameObject markerPrefab;

                if (item.CompareTag("Potion"))
                {
                    markerPrefab = _potionMarkerPrefab;
                }
                else
                {
                    markerPrefab = _weaponMarkerPrefab;
                }

                GameObject marker = Instantiate(markerPrefab, _minimap);
                _itemMarkers.Add(item, marker);
            }

            if (_itemMarkers.ContainsKey(item))
            {
                GameObject marker = _itemMarkers[item];
                marker.SetActive(true);

                // 마커 위치 계산
                RectTransform markerRect = marker.GetComponent<RectTransform>();
                markerRect.anchoredPosition = offset;
            }
        }

        List<Transform> itemsToRemove = new List<Transform>();
        foreach (var pos in _itemMarkers)
        {
            if (pos.Key == null || !_items.Contains(pos.Key))
            {
                Destroy(pos.Value);
                itemsToRemove.Add(pos.Key);
            }
        }

        foreach (Transform removeItem in itemsToRemove)
        {
            _itemMarkers.Remove(removeItem);
        }
    }

    public override void Show()
    {
        base.Show();
        MiniMap();
    }

    public override void UnShow()
    {
        base.UnShow();
    }
}