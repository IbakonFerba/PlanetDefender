using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// <para>Planet based Player controller, always keeping the player on the surface of the planet</para>
///
/// v0.1 04/2020
/// Written by Fabian Kober
/// fabian-kober@gmx.net
/// </summary>
public class PlayerController : MonoBehaviour
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

    #endregion


    // ######################## EXPOSED VARS ######################## //

    #region EXPOSED VARS

    [SerializeField] private Transform _cameraController;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _rotationInterpolationSpeed = 0.2f;

    [Header("Physics")]
    [SerializeField] private float _drag = 1f;
    [SerializeField] private float _acceleration = 0.1f;

    #endregion


    // ######################## PUBLIC VARS ######################## //

    #region PUBLIC VARS

    #endregion


    // ######################## PROTECTED VARS ######################## //

    #region PROTECTED VARS

    #endregion


    // ######################## PRIVATE VARS ######################## //

    #region PRIVATE VARS

    private float _planetRadius;
    private Vector3 _planetCenter;

    private Vector3 _gravityDirection;
    private Vector3 _movementInput;
    private Vector3 _rawVelocity;
    private Vector3 _velocity;

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
        Planet planet = Planet.Instance;
        _planetRadius = planet.Radius;
        _planetCenter = planet.transform.position;

        _movementInput = Vector3.zero;
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
        Move();
        SnapToSurface();
        LookForward();
    }

    public void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    #endregion


    // ######################## FUNCTIONALITY ######################## //

    #region FUNCTIONALITY

    private void SnapToSurface()
    {
        _gravityDirection = _planetCenter - transform.position;
        _gravityDirection.Normalize();

        transform.position = _planetCenter - _gravityDirection * _planetRadius;
        transform.rotation = Quaternion.FromToRotation(transform.up, -_gravityDirection)*transform.rotation;
    }

    private void Move()
    {
        Vector3 accelerationForce = _acceleration*Time.fixedDeltaTime * _movementInput;
        _rawVelocity = Vector3.ClampMagnitude((_rawVelocity+accelerationForce)*(1-Time.fixedDeltaTime*_drag), _speed*Time.fixedDeltaTime);
        _velocity = _rawVelocity.magnitude <= 0.0001f ? Vector3.zero : _cameraController.TransformDirection(_rawVelocity);
        transform.position += _velocity;
    }

    private void LookForward()
    {
        if (_velocity.magnitude <= 0.0f)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_velocity, -_gravityDirection), _rotationInterpolationSpeed);
    }

    #endregion


    // ######################## COROUTINES ######################## //

    #region COROUTINES 

    #endregion


    // ######################## UTILITIES ######################## //

    #region UTILITIES

    #endregion
}