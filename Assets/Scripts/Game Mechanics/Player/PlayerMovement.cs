using Unity.Netcode;
using UnityEngine;

namespace Player
{

 public class PlayerMovement : NetworkBehaviour
 {
  #region Fields

  [SerializeField] private Rigidbody2D _rb;
  [SerializeField] private float _movementSpeed = 0;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods
  void Start()
  {
    
  }

  void FixedUpdate()
  {
    MovePlayer();
  }
  #endregion

  #region Private Methods

  void MovePlayer()
  {
   Vector2 speed=Vector2.zero;

   speed.x = Input.GetAxis("Horizontal");
   speed.y = Input.GetAxis("Vertical");
   
   _rb.velocity=speed*_movementSpeed;
  }
  #endregion

  #region Public Methods

  #endregion
 }
}
