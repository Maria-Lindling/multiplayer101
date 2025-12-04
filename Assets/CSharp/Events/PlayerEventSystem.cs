using UnityEngine;
using UnityEngine.Events;

public class PlayerEventSystem : MonoBehaviour
{
#region Instance
  private static PlayerEventSystem _instance ;
#endregion


#region UnityEditor
  [Header("Events")]
  [SerializeField] private UnityEvent<PlayerEventContext> onCreated ;
  [SerializeField] private UnityEvent<PlayerEventContext> onTakeDamage ;
  [SerializeField] private UnityEvent<PlayerEventContext> onTouchGround ;
  [SerializeField] private UnityEvent<PlayerEventContext> onLeaveGround ;
  [SerializeField] private UnityEvent<PlayerEventContext> onChangeColor ;
#endregion


#region Properties
  public static UnityEvent<PlayerEventContext> OnCreated     => _instance.onCreated ;
  public static UnityEvent<PlayerEventContext> OnTakeDamage  => _instance.onTakeDamage ;
  public static UnityEvent<PlayerEventContext> OnTouchGround => _instance.onTouchGround ;
  public static UnityEvent<PlayerEventContext> OnLeaveGround => _instance.onLeaveGround ;
  public static UnityEvent<PlayerEventContext> OnChangeColor => _instance.onChangeColor ;
#endregion


#region MonoBehavior
  void Start()
  {
    if( _instance != null )
    {
      Destroy( this ) ;
    }
    else
    {
      _instance = this ;
    }
  }
#endregion
}
