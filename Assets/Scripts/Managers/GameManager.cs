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
  public static event Action<GameStates> OnGameStateChanged;
  private GameStates _currentGameState;
  
  #endregion

  #region Properties

  public Transform HostPosition => _hostPosition;
  public Transform ClientPosition => _clientPosition;
  public Collider2D HostCollider => _hostCollider;
  public Collider2D ClientCollider => _clientCollider;
  
  #endregion

  #region Unity Methods
  void Awake()
  {
    UpdateGameState(GameStates.Idle);
  }
  #endregion
  
  #region Public Methods
  public void UpdateGameState(GameStates state)
  {
   if (state != _currentGameState)
   {
    switch (state)
    {
     case GameStates.Idle:
      
      break;
                
     case GameStates.Gameplay:
      
      break;
                
     case GameStates.Lose:
      
      break;
                
     case GameStates.Win:
      
      break;
                
     case GameStates.Endgame:
                    
      break;
    }

    _currentGameState = state;
    OnGameStateChanged?.Invoke(_currentGameState);
   }
  }


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

 public enum GameStates
 {
  Idle,Gameplay,Endgame,Win,Lose
 }
 
}
