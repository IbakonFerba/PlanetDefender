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

    [Header("Grid")] 
    [SerializeField] private int _gridPoints = 1000;
    [SerializeField] private GridCellObject _cellObject;
    #endregion


    // ######################## PUBLIC VARS ######################## //

    #region PUBLIC VARS

    #endregion


    // ######################## PROTECTED VARS ######################## //

    #region PROTECTED VARS

    #endregion


    // ######################## PRIVATE VARS ######################## //

    #region PRIVATE VARS

    private Grid _grid;
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
        _model.localScale = new Vector3(_radius, _radius, _radius) * 2f;
        
        LoadGrid();
    }

    private void LoadGrid()
    {
        _grid = new Grid(_gridPoints);
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
        for (int i = 0; i < _grid.Size; ++i)
        {
            Vector3 pos = Vector3.zero;
            float lng = phi * i;
            float lat = Mathf.Asin(-1f + 2f * (lng/phi) / _grid.Size);
//            pos.y = 1f - (i / (_grid.Size - 1f)) * 2;
//            float rad = Mathf.Sqrt(1 - pos.y * pos.y);
//
//            float theta = phi * i;
//            pos.x = Mathf.Cos(theta)*rad;
//            pos.z = Mathf.Sin(theta) * rad;

            pos.x = 1 * Mathf.Cos(lat) * Mathf.Cos(lng);
            pos.z = 1 * Mathf.Sin(lat);
            pos.y = 1 * Mathf.Cos(lat) * Mathf.Sin(lng);

            pos *= _radius;
            _grid.SetCellObject(i,GameObject.Instantiate(_cellObject, pos, Quaternion.identity));
        }
    }

    public void ClosestGridCell(Vector3 pos)
    {
        float lat = Mathf.Asin(pos.y / _radius);
        float lng = Mathf.Atan2(pos.z, pos.x);
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f));
        int i = Mathf.RoundToInt((Mathf.Sin(lat)+1f)/2f)*_grid.Size;
        Debug.Log(i);
//        Debug.Log($"Lat {lat}, Long {lng}");
    }
    #endregion


    // ######################## COROUTINES ######################## //

    #region COROUTINES 

    #endregion


    // ######################## UTILITIES ######################## //

    #region UTILITIES

    #endregion
}