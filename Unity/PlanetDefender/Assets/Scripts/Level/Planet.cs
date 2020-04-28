using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FK.Utility;

/// <summary>
/// <para>The Planet that is the stage of the game</para>
///
/// v0.1 04/2020
/// Written by Fabian Kober
/// fabian-kober@gmx.net
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Planet : Singleton<Planet>
{
    // ######################## STRUCTS & CLASSES ######################## //

    #region STRUCTS & CLASSES

    #endregion


    // ######################## ENUMS & DELEGATES ######################## //

    #region ENUMS & DELEGATES

    #endregion


    // ######################## EVENTS ######################## //

    #region EVENTS

    #endregion


    // ######################## PROPERTIES ######################## //

    #region PROPERTIES
    public float Radius => _radius;
    #endregion


    // ######################## EXPOSED VARS ######################## //

    #region EXPOSED VARS

    [SerializeField] private Transform _model;
    [SerializeField] private float _radius = 5.0f;

    [SerializeField] private GameObject TestPrefab;
    [SerializeField] private int TestGridPoints = 1000;
    #endregion


    // ######################## PUBLIC VARS ######################## //

    #region PUBLIC VARS

    #endregion


    // ######################## PROTECTED VARS ######################## //

    #region PROTECTED VARS

    #endregion


    // ######################## PRIVATE VARS ######################## //

    #region PRIVATE VARS
    private SphereCollider _collider;
    #endregion


    // ######################## INITS ######################## //

    #region CONSTRUCTORS

    #endregion

    #region INITS

    ///<summary>
    /// Does the Init for this Behaviour
    ///</summary>
    private void Init()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = _radius;
        _model.localScale = new Vector3(_radius, _radius, _radius) * 2f;
        
        BuildGrid();
    }

    #endregion


    // ######################## UNITY EVENT FUNCTIONS ######################## //

    #region UNITY EVENT FUNCTIONS

    private void Start()
    {
        Init();
    }

    private void Update()
    {
    }

    #endregion


    // ######################## FUNCTIONALITY ######################## //

    #region FUNCTIONALITY

    private void BuildGrid()
    {
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f));
        for (int i = 0; i < TestGridPoints; ++i)
        {
            Vector3 pos = Vector3.zero;
            pos.y = 1f - (i / (TestGridPoints - 1f)) * 2;
            float rad = Mathf.Sqrt(1 - pos.y * pos.y);

            float theta = phi * i;
            pos.x = Mathf.Cos(theta)*rad;
            pos.z = Mathf.Sin(theta) * rad;

            pos *= _radius;
            GameObject.Instantiate(TestPrefab, pos, Quaternion.identity);
        }
    }
    #endregion


    // ######################## COROUTINES ######################## //

    #region COROUTINES 

    #endregion


    // ######################## UTILITIES ######################## //

    #region UTILITIES

    #endregion
}