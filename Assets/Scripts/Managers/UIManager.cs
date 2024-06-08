using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{

 public class UIManager : SingletonManager<UIManager>
 {
  #region Fields
  
  [SerializeField] private GameObject _winScreen;
  [SerializeField] private GameObject _loseScreen;
  [SerializeField] private GameObject _mainScreen;
  [SerializeField] private GameObject _gameScreen;
  
  [SerializeField] private Button _attackButton;
  [SerializeField] private Joystick _movementJoystick;

  #endregion

  #region Properties

  public Button AttackButton => _attackButton;
  public Joystick MovementJoystick => _movementJoystick;
  
  #endregion

  #region Unity Methods
  void Start()
  {
   ControlMainScreen(true);
  }
  
  #endregion

  #region Private Methods

  public void ControlMainScreen(bool val)
  {
   if (val)
   {
    _loseScreen.SetActive(false);
    _winScreen.SetActive(false);
   }
   _mainScreen.SetActive(val);
   _gameScreen.SetActive(!val);
  }
  #endregion

  #region Public Methods


  public void ShowResultScreen(bool isLose)
  {
   if (isLose)
   {
    _loseScreen.SetActive(true);
    _winScreen.SetActive(false);
   }
   else
   {
    _winScreen.SetActive(true);
    _loseScreen.SetActive(false);
   }

  }
  
  #endregion
 }
}
