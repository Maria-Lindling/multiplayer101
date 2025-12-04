using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using System;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Renderer))]
public partial class PlayerController : NetworkBehaviour
{
#region Debug
  private const string newName = "John Unity" ;
  private readonly float _gravity = -9.81f;
#endregion


#region UnityEditor
  [Header("Player Settings")]
  [SerializeField] private Color playerColor = Color.white ;
  [SerializeField] private string playerName = "default" ;
  [SerializeField] private float playerMoveSpeed = 3.0f ;
  [SerializeField] private float playerJumpForce = 55.0f ;

  [Header("Camera Settings")]
  [SerializeField] private float mouseSensitivity = 1.0f ;
  [SerializeField] private float maxLookUpAngle = 65.0f;
  [SerializeField] private float maxLookDownAngle = 85.0f;

  [Header("Component Settings")]
  [SerializeField] private CharacterController characterController ;
  [SerializeField] private Transform cameraTransform ;
  [SerializeField] private TextMeshPro namePlate ;
#endregion


#region SyncVar
  private readonly SyncVar<float> _moveSpeed = new();
  private readonly SyncVar<Color> _playerColor = new();
  private readonly SyncVar<string> _playerName = new();
#endregion


#region Readonly
  private readonly Vector3 _jumpVector = new(0.0f, 1.0f, 0.0f);
  private readonly Cooldown _cooldownColorChange = new( new TimeSpan(0,0,0,0,451), false) ;
#endregion


#region Fields
  private Vector3 _moveInput = Vector3.zero ;
  private bool _previousGroundedState = false ;
  private Renderer _renderer ;
#endregion


#region Properties
  private (float Up, float Down) MaxLookAngle => (maxLookUpAngle * -1, maxLookDownAngle);
  private bool HorizonCrossed => cameraTransform.localEulerAngles.x > 180.0f;
#endregion


#region Update
  [TargetRpc]
  private void TargetUpdate(NetworkConnection conn)
  {
    
  }

  [ObserversRpc(ExcludeOwner = true)]
  private void ObserverUpdate()
  {
    FaceNameplateToCamera() ;
  }
#endregion


#region MonoBehavior
  private void Start()
  {
    _renderer = GetComponent<Renderer>() ;
    
    _playerColor.OnChange += OnPlayerColorChange ;
    _playerName.OnChange  += OnPlayerNameChange ;

    StartCoroutine( LateStartServer( 0.0167f ) ) ;
  }

  private void Update()
  {
    ClientUpdate() ;
  }
#endregion
}
