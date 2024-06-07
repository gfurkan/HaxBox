using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Managers.Network
{

 public class NetworkUIManager : MonoBehaviour
 {
  #region Fields
  
  [SerializeField] private Button _hostButton;
  [SerializeField] private Button _clientButton;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods

  private void Awake()
  {
   _hostButton.onClick.AddListener((() =>
   {

   }));
   _clientButton.onClick.AddListener((() =>
   {

   }));
  }

  #endregion

  #region Private Methods

  #endregion

  #region Public Methods

  #endregion
 }
}
