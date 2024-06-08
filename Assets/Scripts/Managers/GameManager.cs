using System;
using Unity.Netcode;
using UnityEngine;

namespace Managers
{

 public class GameManager : SingletonManager<GameManager>
 {
  #region Fields
  
  [SerializeField] private Transform _hostPosition;
  [SerializeField] private Transform _clientPosition;
  [SerializeField] private Collider2D _clientCollider;
  [SerializeField] private Collider2D _hostCollider;

  #endregion

  #region Properties

  public Transform HostPosition => _hostPosition;
  public Transform ClientPosition => _clientPosition;
  public Collider2D HostCollider => _hostCollider;
  public Collider2D ClientCollider => _clientCollider;
  
  #endregion

  #region Unity Methods
  
  #endregion
  
  #region Public Methods

  public void StartHost()
  {
   NetworkManager.Singleton.StartHost();
   UIManager.Instance.ControlMainScreen(false);
  }

  public void StartClient()
  {
   NetworkManager.Singleton.StartClient();
   UIManager.Instance.ControlMainScreen(false);
  }
  public void ReturnToHome()
  {
   UIManager.Instance.ControlMainScreen(true);
   NetworkManager.Singleton.Shutdown();
  }
  #endregion
 }

}
