using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
#region UnityEditor
  [SerializeField] private string identity ;
  [SerializeField] private string label ;
  [SerializeField] private RawImage indicatorBox ;
  [SerializeField] private TextMeshProUGUI indicatorText ;
#endregion


#region Enum
  public enum UpdateValues
  {
    Visibility,
    Color,
    Text,
  }
#endregion


#region Fields
  private bool _visibility = false ;
  private Color _color ;
  private string _text ;
#endregion


#region Properties
  public bool Visibility
  {
    get { return _visibility ; }
    private set
    {
      _visibility = value ;
      indicatorBox.gameObject.SetActive(_visibility) ;
      indicatorText.gameObject.SetActive(_visibility) ;
    }
  }
  public Color Color
  {
    get { return _color ; }
    private set
    {
      _color = value ;
      indicatorBox.color = _color ;
    }
  }
  public string Text
  {
    get { return _text ; }
    private set
    {
      _text = value ;
      indicatorText.text = _text ;
    }
  }
#endregion


#region Callbacks
  public void OnClientActive(PlayerEventContext ctx)
  {
    if( !CheckIsTarget(ctx) )
      return ;
    Text = label ;
    Visibility = true ;
  }

  public void OnUpdateVisibility(PlayerEventContext ctx)
  {
    if( !CheckIsTarget(ctx) )
      return ;
    Visibility = ctx.Visibility ;
  }

  public void OnUpdateColor(PlayerEventContext ctx)
  {
    if( !CheckIsTarget(ctx) )
      return ;
    Color = ctx.Color ;
  }

  public void OnUpdateText(PlayerEventContext ctx)
  {
    if( !CheckIsTarget(ctx) )
      return ;
    Text = ctx.Text ;
  }
#endregion


#region Checks
  private bool CheckIsTarget(PlayerEventContext ctx) => ctx.Target == "any" || identity == ctx.Target ;
#endregion
}