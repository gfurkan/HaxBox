using Unity.Netcode;
using UnityEngine;

namespace Managers
{

 public class UIManager : SingletonManager<UIManager>
 {
  #region Fields
  
  [SerializeField] private GameObject _winScreen;
  [SerializeField] private GameObject _loseScreen;
  [SerializeField] private GameObject _mainScreen;

  #endregion

  #region Properties

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
