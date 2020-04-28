using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Controls the camera orbiting the planet</para>
///
/// v1.0 04/2020
/// Written by Fabian Kober
/// fabian-kober@gmx.net
/// </summary>
public class CameraController : MonoBehaviour
{
    // ######################## EXPOSED VARS ######################## //

    #region EXPOSED VARS

    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _followSpeed = 0.05f;

    #endregion


    // ######################## PRIVATE VARS ######################## //

    #region PRIVATE VARS

    private Vector3 _vectorToTarget;

    #endregion


    // ######################## INITS ######################## //

    #region INITS

    ///<summary>
    /// Does the Init for this Behaviour
    ///</summary>
    private void Init()
    {
        transform.position = Planet.Instance.transform.position;
    }

    #endregion


    // ######################## UNITY EVENT FUNCTIONS ######################## //

    #region UNITY EVENT FUNCTIONS

    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        _vectorToTarget = _cameraTarget.position - transform.position;
        _vectorToTarget.Normalize();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-_vectorToTarget, transform.up), _followSpeed);
    }

    #endregion
}