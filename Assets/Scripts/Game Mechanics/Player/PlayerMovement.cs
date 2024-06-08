using Managers;
using Unity.Netcode;
using UnityEngine;

namespace Player
{

 public class PlayerMovement : NetworkBehaviour
 {
  #region Fields
  
  [SerializeField] private PlayerNetwork _playerNetwork;
  [SerializeField] private float _movementSpeed = 0;
  [SerializeField] private float _maxSpeed = 0;
  [SerializeField] private float _decelerationFactor = 0;

  private NetworkVariable<bool> _isMovementEnabled =
   new NetworkVariable<bool>(false);

  #endregion
  
  #region Unity Methods

  private void Start()
  {
   LocatePlayers();
   ControlLines();
  }

  void FixedUpdate()
  {
   if (IsOwner)
   {
    ControlMovementServerRpc();
    
    if (_isMovementEnabled.Value)
    {
     MovePlayer();
    }
    
   }
  }
  #endregion

  #region Private Methods

  [ServerRpc]
  void ControlMovementServerRpc()
  {
   if (IsHost)
    if (NetworkManager.Singleton.ConnectedClientsList.Count > 1)
    {
     _isMovementEnabled.Value = true;
    }
    else
    {
     _isMovementEnabled.Value = false;
    }
  }
  void MovePlayer()
  {
   Vector2 speedInputs=Vector2.zero;

   speedInputs.x = UIManager.Instance.MovementJoystick.Horizontal;
   speedInputs.y = UIManager.Instance.MovementJoystick.Vertical;

   if (speedInputs.magnitude != 0)
   {
    _playerNetwork.Rb.AddForce(speedInputs * _movementSpeed);
   }
   else
   {
    _playerNetwork.Rb.velocity *= (1 - _decelerationFactor * Time.fixedDeltaTime);
   }

   
   if (_playerNetwork.Rb.velocity.magnitude > _maxSpeed)
   {
    _playerNetwork.Rb.velocity = _playerNetwork.Rb.velocity.normalized * _maxSpeed;
   }
  }
  
  void LocatePlayers()
  {
   if (IsOwner)
   {
    if (IsHost)
    {
     transform.position = GameManager.Instance.HostPosition.position;
    }
    else
    {
     transform.position = GameManager.Instance.ClientPosition.position;
    }
   }
  }

  void ControlLines()
  {
   if (IsOwner)
   {
    if (IsHost)
    {
     GameManager.Instance.HostCollider.isTrigger=true;
     GameManager.Instance.ClientCollider.isTrigger=false;
    }
    else
    {
     GameManager.Instance.ClientCollider.isTrigger=true;
     GameManager.Instance.HostCollider.isTrigger=false;
    }
   }
  }
  #endregion
 }
}
