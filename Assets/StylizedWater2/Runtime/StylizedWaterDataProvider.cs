﻿//Buoyancy API is slated to be revised
//Using the BuoyancySample struct method will support compute-accelerated water displacement queries
#define OLD_API

using System;
using UnityEngine;
using StylizedWater2;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NWH.DWP2.WaterData
{
    #if NWH_DWP2
    public class StylizedWaterDataProvider : WaterDataProvider
    {
        [Tooltip("This reference is required to grab the wave distance and height values")]
        public Material waterMat;
        public enum WaterLevelSource
        {
            Value,
            Mesh
        }
        [Tooltip("Configure what should be used to set the base water level. Relative wave height is added to this value")]
        public WaterLevelSource waterLevelSource = WaterLevelSource.Value;
        [Tooltip("This reference is required to get the base water height. Relative wave height is added to this")]
        public MeshRenderer waterPlane;
        public float waterLevel;

        [Tooltip("Enable if the wave settings are being changed at runtime. Incurs some overhead")]
        public bool dynamicMaterial;

        private float m_waterLevel = 0f;
        private Vector3[] _normals;
        private int _prevArraySize;

        #if !OLD_API
        private Buoyancy.BuoyancySample sampler = new Buoyancy.BuoyancySample();
        private Buoyancy.BuoyancySample samplerSinglePoint = new Buoyancy.BuoyancySample();
        #endif
        
        private void Reset()
        {
            MeshRenderer r = GetComponent<MeshRenderer>();

            if (r)
            {
                waterPlane = r;
                waterMat = r.sharedMaterial;
            }

            waterLevel = this.transform.position.y;
        }

        #if !OLD_API
        private void Start()
        {
            samplerSinglePoint.SetSamplePositions(new Vector3[0]);
        }
        #endif
        
        private void OnValidate()
        {
            if (!waterMat && waterPlane) waterMat = waterPlane.sharedMaterial;
        }

        public override bool SupportsWaterHeightQueries()
        {
            return true;
        }

        public override bool SupportsWaterNormalQueries()
        {
            return true;
        }

        public override bool SupportsWaterFlowQueries()
        {
            return false;
        }
        
        public override void GetWaterHeights(NWH.DWP2.WaterObjects.WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            var n = points.Length;

            m_waterLevel = waterPlane && waterLevelSource == WaterLevelSource.Mesh ? waterPlane.transform.position.y : waterLevel;

            // Resize array if data size changed
            if (n != _prevArraySize)
            {
                #if !OLD_API
                sampler.SetSamplePositions(points);
                #endif
                
                _normals = new Vector3[n];
                waterHeights = new float[n];

                _prevArraySize = n;
            }

            #if !OLD_API
            sampler.hashCode = waterObject.instanceID;
            Buoyancy.SampleWaves(ref sampler, waterMat, m_waterLevel, dynamicMaterial);
            
            for (int i = 0; i < sampler.inputPositions.Length; i++)
            {
                waterHeights[i] = sampler.outputOffset[i].y;
                _normals[i] = sampler.outputNormal[i];
            }
            #else
            for (int i = 0; i < points.Length; i++)
            {
                waterHeights[i] = Buoyancy.SampleWaves(points[i], waterMat, m_waterLevel, 0f, dynamicMaterial, out _normals[i]);
            }
            #endif
        }

        public override void GetWaterNormals(NWH.DWP2.WaterObjects.WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterNormals)
        {
            waterNormals = _normals; // Already queried in GetWaterHeights
        }
        
        public override float GetWaterHeightSingle(NWH.DWP2.WaterObjects.WaterObject waterObject, Vector3 point)
        {
            #if !OLD_API
            samplerSinglePoint.hashCode = waterObject.instanceID;
            samplerSinglePoint.inputPositions[0] = point;
            
            Buoyancy.SampleWaves(ref samplerSinglePoint, waterMat, m_waterLevel, dynamicMaterial);

            return samplerSinglePoint.outputOffset[0].y;
            #else
            return Buoyancy.SampleWaves(point, waterMat, m_waterLevel, 0f, dynamicMaterial, out _);
            #endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StylizedWaterDataProvider))]
    public class StylizedWaterDataProviderInspector : Editor
    {
        SerializedProperty waterMat;
        SerializedProperty dynamicMaterial;
        SerializedProperty waterLevelSource;
        SerializedProperty waterPlane;
        SerializedProperty waterLevel;

        private void OnEnable()
        {
            waterMat = serializedObject.FindProperty("waterMat");
            dynamicMaterial = serializedObject.FindProperty("dynamicMaterial");
            waterLevelSource = serializedObject.FindProperty("waterLevelSource");
            waterPlane = serializedObject.FindProperty("waterPlane");
            waterLevel = serializedObject.FindProperty("waterLevel");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(waterMat);

            if (waterMat.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("A water material must be assigned!", MessageType.Error);
            }
            
            EditorGUILayout.PropertyField(dynamicMaterial);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Water level source");
                waterLevelSource.intValue = GUILayout.Toolbar(waterLevelSource.intValue, new GUIContent[] { new GUIContent("Fixed Value"), new GUIContent("Mesh Object") });
            }

            if (waterLevelSource.intValue == (int)StylizedWaterDataProvider.WaterLevelSource.Value) EditorGUILayout.PropertyField(waterLevel);
            if (waterLevelSource.intValue == (int)StylizedWaterDataProvider.WaterLevelSource.Mesh) EditorGUILayout.PropertyField(waterPlane);
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
#endif
}