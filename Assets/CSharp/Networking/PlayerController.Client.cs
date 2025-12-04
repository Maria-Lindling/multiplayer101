using UnityEngine;
using FishNet.Object;
using static UnityEngine.InputSystem.InputAction;
using System;
using FishNet.Connection;

public partial class PlayerController : NetworkBehaviour
{
#region Input
  public void OnMove(CallbackContext ctx)
  {
    if( !IsOwner )
      return ;

    Vector2 moveInput2d = ctx.ReadValue<Vector2>().normalized ;

    _moveInput = new Vector3(moveInput2d.x, _moveInput.y, moveInput2d.y);
  }

  public void OnJump(CallbackContext ctx)
  {
    if ( !IsOwner || !characterController.isGrounded )
      return;

    _moveInput.y = (_jumpVector * (playerJumpForce / 100 * Math.Abs(_gravity))).y;
  }

  public void OnLook(CallbackContext ctx)
  {
    if( !IsOwner )
      return ;

    // Camera rotation
    Vector2 lookInput = ctx.ReadValue<Vector2>() / 30;
    transform.Rotate(mouseSensitivity * lookInput.x * Vector3.up);
    cameraTransform.Rotate(mouseSensitivity * lookInput.y * Vector3.left);
  }

  public void OnInteract(CallbackContext ctx)
  {
    if( !IsOwner )
      return ;

    OnInteractSvr() ;
  }
#endregion


#region SyncVar
  public void OnPlayerColorChange(Color prev, Color next, bool isServer)
  {
    _renderer.material.color = next ;

    if( !IsOwner )
      return ;
    
    PlayerEventSystem.OnChangeColor.Invoke( new PlayerEventContext(){ Color = next } ) ;
  }

  public void OnPlayerNameChange(string prev, string next, bool isServer)
  {
    namePlate.text = next ;
  }
#endregion


#region Movement
  private Vector3 NextMove
  {
    get
    {
      if ( characterController.isGrounded )
      {
        _moveInput.y -= 1.0f * Time.deltaTime;
      } else {
        _moveInput.y += _gravity * 2.0f * Time.deltaTime;
      }
      Vector3 move = _moveSpeed.Value * Time.deltaTime * (transform.right * _moveInput.x + transform.forward * _moveInput.z);
      move.y = _moveInput.y * Time.deltaTime;
      return move ;
    }
  }

  private void ClientMove(Vector3 value)
  {
    characterController.Move(value) ;
    ServerMove(value) ;
  }

  private void CheckGround()
  {
    if( characterController.isGrounded && !_previousGroundedState )
    {
      PlayerEventSystem.OnTouchGround.Invoke( new PlayerEventContext(){Color=Color.green} ) ;
    }
    else if( !characterController.isGrounded && _previousGroundedState )
    {
      PlayerEventSystem.OnLeaveGround.Invoke( new PlayerEventContext(){Color=Color.red} ) ;
    }
    _previousGroundedState = characterController.isGrounded ;
  }
#endregion


#region Camera
  private void FaceNameplateToCamera()
  {
    namePlate.gameObject.transform.forward = -(Camera.main.gameObject.transform.position - namePlate.gameObject.transform.position) ;
  }

  private void ClampCameraRotation()
  {
    if( !IsOwner )
      return ;

    // it's too early in the morning to math good
    // what the fuck am I even doing here
    cameraTransform.localEulerAngles = new Vector3(
      Mathf.Clamp(
        cameraTransform.localEulerAngles.x + (HorizonCrossed ? -360 : 0),
        MaxLookAngle.Up,
        MaxLookAngle.Down
      ) + (HorizonCrossed ? 360 : 0),
      0.0f, // we always want to be facing "forward" (this rotation is relative to the player!!)
      0.0f // the 'actual' angle can be kept if there's a 'camera lean' feature
    );
  }

  private void LockAndHideCursor()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
  }
#endregion


#region Init
  [TargetRpc]
  public void ClientSetup(NetworkConnection conn, string name, Color color, float movespeed)
  {
    LockAndHideCursor() ;
    SetPlayerName(name) ;
    SetPlayerColor(color) ;
    SetMoveSpeed(movespeed) ;

    cameraTransform.gameObject.SetActive( true ) ;
    PlayerEventSystem.OnCreated.Invoke( new PlayerEventContext() ) ;
  }
#endregion


#region Update
  private void ClientUpdate()
  {
    if( !IsOwner )
      return ;

    ClampCameraRotation();
    ClientMove( NextMove ) ;
    CheckGround() ;

    ServerUpdate() ;
  }
#endregion
}
