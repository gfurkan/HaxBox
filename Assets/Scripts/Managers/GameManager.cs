using System;

namespace Managers
{

 public class GameManager : SingletonManager<GameManager>
 {
  #region Fields
  
  public static event Action<GameStates> OnGameStateChanged;
  private GameStates _currentGameState;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods
  void Awake()
  {
   UpdateGameState(GameStates.Idle);
  }
  #endregion

  #region Private Methods

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

  #endregion
 }
 public enum GameStates
 {
  Idle,Gameplay,Endgame,Win,Lose
 }
}
