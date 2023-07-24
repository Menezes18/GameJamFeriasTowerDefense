using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildTool : MonoBehaviour
{
    [SerializeField] private float _rotateSnapAngle = 90f;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _buildModeLayerMask;
    [SerializeField] private LayerMask _deleteModeLayerMask;
    [SerializeField] private int _defaultLayerInt = 8;
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private Material _buildingMatPositive;
    [SerializeField] private Material _buildingMatNegative;

    private bool _deleteModeEnabled;

    private Camera _camera;

    private Building _spawnedBuilding;
    private Building _targetBuilding;
    private Quaternion _lastRotation;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        BuildingPanelUI.OnPartChosen += ChoosePart;
    }
    
    private void OnDisable()
    {
        BuildingPanelUI.OnPartChosen -= ChoosePart;
    }

    private void ChoosePart(BuildingData data)
    {
        if (_deleteModeEnabled)
        {
            if (_targetBuilding != null && _targetBuilding.FlaggedForDelete) _targetBuilding.RemoveDeleteFlag();
            _targetBuilding = null;
            _deleteModeEnabled = false;
        }

        DeleteObjectPreview();

        var go = new GameObject
        {
            layer = _defaultLayerInt,
            name = "Build Preview"
        };

        _spawnedBuilding = go.AddComponent<Building>();
        _spawnedBuilding.Init(data);
        _spawnedBuilding.transform.rotation = _lastRotation;
    }

    private void Update()
    {
        if (_spawnedBuilding && Keyboard.current.escapeKey.wasPressedThisFrame) DeleteObjectPreview();
        if (Keyboard.current.qKey.wasPressedThisFrame) _deleteModeEnabled = !_deleteModeEnabled;
        
        if (_deleteModeEnabled) DeleteModeLogic();
        else BuildModeLogic();

    }

    private void DeleteObjectPreview()
    {
        if (_spawnedBuilding != null)
        {
            Destroy(_spawnedBuilding.gameObject);
            _spawnedBuilding = null;
        }
    }

    private bool IsRayHittingSomething(LayerMask layerMask, out RaycastHit hitInfo)
    {
        var ray = new Ray(_rayOrigin.position, _camera.transform.forward * _rayDistance);
        return Physics.Raycast(ray, out hitInfo, _rayDistance, layerMask);
    }
    
     
    private void DeleteModeLogic()
    {
        if (IsRayHittingSomething(_deleteModeLayerMask, out RaycastHit hitInfo))
        {
            var detectedBuilding = hitInfo.collider.gameObject.GetComponentInParent<Building>();

            if (detectedBuilding == null) return;

            if (_targetBuilding == null) _targetBuilding = detectedBuilding;

            if (detectedBuilding != _targetBuilding && _targetBuilding.FlaggedForDelete)
            {
                _targetBuilding.RemoveDeleteFlag();
                _targetBuilding = detectedBuilding;
            }

            if (detectedBuilding == _targetBuilding && !_targetBuilding.FlaggedForDelete)
            {
                _targetBuilding.FlagForDelete(_buildingMatNegative);
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Destroy(_targetBuilding.gameObject);
                _targetBuilding = null;
            }
        }
        else
        {
            if (_targetBuilding != null && _targetBuilding.FlaggedForDelete)
            {
                _targetBuilding.RemoveDeleteFlag();
                _targetBuilding = null;
            }
        }
        
    }


    private void BuildModeLogic()
    {
        if (_targetBuilding != null && _targetBuilding.FlaggedForDelete)
        {
            _targetBuilding.RemoveDeleteFlag();
            _targetBuilding = null;
        }
        
        if (_spawnedBuilding == null) return;

        PositionBuildingPreview();
    }

    private void PositionBuildingPreview()
    {
        _spawnedBuilding.UpdateMaterial(_spawnedBuilding.IsOverlapping ? _buildingMatNegative : _buildingMatPositive);
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            _spawnedBuilding.transform.Rotate(0,_rotateSnapAngle,0);
            _lastRotation = _spawnedBuilding.transform.rotation;
        }

        if (IsRayHittingSomething(_buildModeLayerMask, out RaycastHit hitInfo))
        {
            var gridPosition = WorldGrid.GridPositionFromWorldPoint3D(hitInfo.point, 1f);
            _spawnedBuilding.transform.position = gridPosition;
            
            if (Mouse.current.leftButton.wasPressedThisFrame && !_spawnedBuilding.IsOverlapping)
            {
                _spawnedBuilding.PlaceBuilding();
                var dataCopy = _spawnedBuilding.AssignedData;
                _spawnedBuilding = null;
                ChoosePart(dataCopy);
            }   
        }
    }
}
