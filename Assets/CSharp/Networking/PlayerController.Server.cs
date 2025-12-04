using UnityEngine;
using FishNet.Object;
using System.Collections;
using static UnityEngine.InputSystem.InputAction;

public partial class PlayerController : NetworkBehaviour
{
#region ServerAuth
  [ServerRpc(RequireOwnership = true)]
  public void OnInteractSvr()
  {
    if( !_cooldownColorChange.IsComplete )
      return ;

    _playerColor.Value = Random.ColorHSV() ;

    _cooldownColorChange.Restart() ;
  }

  [ServerRpc(RequireOwnership = true)]
  private void ServerMove(Vector3 value)
  {
    characterController.Move(value) ;
    UpdatePosition(transform.position) ;
  }

  [ObserversRpc(ExcludeServer = true)]
  private void UpdatePosition(Vector3 value) => transform.position = value ;
#endregion


#region SyncVar
  [ServerRpc(RequireOwnership = true)]
  private void SetMoveSpeed(float value)
  {
    _moveSpeed.Value = value ;
  }

  [ServerRpc(RequireOwnership = true)]
  private void SetPlayerColor(Color value)
  {
    _playerColor.Value = value ;
  }

  [ServerRpc(RequireOwnership = true)]
  private void SetPlayerName(string value)
  {
    _playerName.Value = value ;
  }
#endregion


#region Init
  IEnumerator LateStartServer(float waitTime)
  {
    yield return new WaitForSeconds(waitTime);

    if( IsServerInitialized )
    {
      _playerColor.Value = playerColor ;
      _playerName.Value  = playerName ;
      _moveSpeed.Value   = playerMoveSpeed ;
      
      _cooldownColorChange.Restart() ;
      _cooldownColorChange.Shortcut() ;

      ClientSetup( Owner, newName, Color.magenta, 5.0f ) ;
    }
  }
#endregion


#region Update
  [ServerRpc]
  private void ServerUpdate()
  {
    FaceNameplateToCamera() ;
    
    TargetUpdate(Owner) ;
    ObserverUpdate() ;
  }
#endregion
}
