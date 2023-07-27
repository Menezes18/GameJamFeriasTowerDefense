using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    private BuildingData _assignedData;
    private BoxCollider _boxCollider;
    private GameObject _graphic;
    private Transform _colliders;
    private bool _isOverlapping;

    public BuildingData AssignedData => _assignedData;
    public bool IsOverlapping => _isOverlapping;
    
    private Renderer _renderer;
    private Material _defaultMaterial;

    private bool _flaggedForDelete;
    public bool FlaggedForDelete => _flaggedForDelete;

    private BuildingSaveData SaveData;
    public BuildTool buildTool;
    public GameManager gameManager;

    public void Start()
    {
        buildTool = FindObjectOfType<BuildTool>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Init(BuildingData data, BuildingSaveData saveData = null)
    {
        _assignedData = data;
        
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.size = _assignedData.BuildingSize;
        _boxCollider.center = new Vector3(0, (_assignedData.BuildingSize.y + .2f) * 0.5f, 0);
        _boxCollider.isTrigger = true;

        var rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        _graphic = Instantiate(data.Prefab, transform);
        _renderer = _graphic.GetComponentInChildren<Renderer>();
        _defaultMaterial = _renderer.material;

        _colliders = _graphic.transform.Find("Colliders");
        
        if (_colliders != null) _colliders.gameObject.SetActive(false);
        

        if (saveData != null) SaveData = saveData;

    }
    
    public void PlaceBuilding()
    {
        _boxCollider.enabled = false;
        if (_colliders != null) _colliders.gameObject.SetActive(true);
        UpdateMaterial(_defaultMaterial);
        gameObject.layer = 6;
        gameObject.name = _assignedData.DisplayName + " - " + transform.position;
        
        if (transform.childCount > 0)
        {
            Transform primeiroFilho = transform.GetChild(0);
            primeiroFilho.gameObject.tag = "Tower";
        }
    }



    public void UpdateMaterial(Material newMaterial)
    {
        if (_renderer == null) return;
        if (_renderer.material != newMaterial) _renderer.material = newMaterial;
    }

    public void FlagForDelete(Material deleteMat)
    {
        UpdateMaterial(deleteMat);
        _flaggedForDelete = true;
    }

    public void RemoveDeleteFlag()
    {
        UpdateMaterial(_defaultMaterial);
        _flaggedForDelete = false;
    }

    private void OnTriggerStay(Collider other)
    {
        _isOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isOverlapping = false;
    }

    private void OnDestroy()
    {
        Debug.Log("Destroying object");
    }

}

[System.Serializable]
public class BuildingSaveData
{
    public string BuildingName;
    public BuildingData AssignedData;
    public Vector3 Position;
    public Quaternion Rotation;

    public BuildingSaveData(string buildingName, BuildingData assignedData, Vector3 position, Quaternion rotation)
    {
        BuildingName = buildingName;
        AssignedData = assignedData;
        Position = position;
        Rotation = rotation;
    }
}
