using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Managers.Network
{

 public class NetworkUIManager : MonoBehaviour
 {
  #region Fields

  [SerializeField] private Button _serverButton;
  [SerializeField] private Button _hostButton;
  [SerializeField] private Button _clientButton;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods

  private void Awake()
  {
   _serverButton.onClick.AddListener((() =>
   {
    NetworkManager.Singleton.StartServer();
   }));
   _hostButton.onClick.AddListener((() =>
   {
    NetworkManager.Singleton.StartHost();
   }));
   _clientButton.onClick.AddListener((() =>
   {
    NetworkManager.Singleton.StartClient();
   }));
  }

  #endregion

  #region Private Methods

  #endregion

  #region Public Methods

  #endregion
 }
}
