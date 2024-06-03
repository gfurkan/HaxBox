using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{

 public class PlayerMovement : NetworkBehaviour
 {
  #region Fields

  [SerializeField] private Rigidbody2D _rb;
  [SerializeField] private float _movementSpeed = 0;
  [SerializeField] private float _maxSpeed = 0;
  [SerializeField] private float _decelerationFactor = 0;
  [SerializeField] private float _pushSpeed = 0;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods

  private void OnEnable()
  {
   PlayerHitController.OnDamagedWithPush += PushPlayer;
  }

  private void OnDisable()
  {
   PlayerHitController.OnDamagedWithPush -= PushPlayer;
  }

  void FixedUpdate()
  {
   if(IsOwner)
    MovePlayer();
  }
  #endregion

  #region Private Methods

  void MovePlayer()
  {
   Vector2 speedInputs=Vector2.zero;

   speedInputs.x = Input.GetAxis("Horizontal");
   speedInputs.y = Input.GetAxis("Vertical");

   if (speedInputs.magnitude != 0)
   {
    _rb.AddForce(speedInputs * _movementSpeed);
   }
   else
   {
    _rb.velocity *= (1 - _decelerationFactor * Time.fixedDeltaTime);
   }

   
   if (_rb.velocity.magnitude > _maxSpeed)
   {
    _rb.velocity = _rb.velocity.normalized * _maxSpeed;
   }
  }
  
  private void PushPlayer(Vector2 forceDirection)
  {
    _rb.AddForce(forceDirection * _pushSpeed, ForceMode2D.Impulse);
  }
  #endregion

  #region Public Methods

  #endregion
 }
}
