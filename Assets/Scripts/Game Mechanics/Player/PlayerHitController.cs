using System;
using Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

namespace Player
{

 public class PlayerHitController : NetworkBehaviour,IDamagable
 {
  #region Fields

  [SerializeField] private int _health = 0;
  [SerializeField] private float _attackRange;
  [SerializeField] private int _attackPower;
  [SerializeField] private float _pushSpeed = 0;
  [SerializeField] private Rigidbody2D _rb;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods
  void Update()
  {
   if (IsOwner)
   {
    if (Input.GetKeyDown(KeyCode.Space))
    {
     AttackServerRpc();
    }
   }
  }

  private void OnDrawGizmos()
  {
   Gizmos.color=Color.red;
   Gizmos.DrawWireSphere(transform.position,_attackRange);
  }

  #endregion

  #region Private Methods
  
  [ServerRpc]
  private void AttackServerRpc()
  {
   Collider2D[] _enemies = Physics2D.OverlapCircleAll(transform.position, _attackRange);
   
   for (int i = 0; i < _enemies.Length; i++)
   {
    if (_enemies[i].gameObject!=gameObject)
    {
     if (_enemies[i].TryGetComponent(out IDamagable enemy))
     {
      Vector2 _pushForce = _enemies[i].transform.position - transform.position;
      _pushForce.Normalize();
      enemy.DealDamageClientRpc(_attackPower,_pushForce);
     }
    }
   }
  }
  
  #endregion

  #region Public Methods
  
  [ClientRpc]
  private void PushPlayerClientRpc(Vector2 forceDirection)
  {
   _rb.AddForce(forceDirection * _pushSpeed, ForceMode2D.Impulse);
  }
  
  [ClientRpc]
  public void DealDamageClientRpc(int damageToDeal,Vector2 force)
  {
   if (_health <= damageToDeal)
   {
    // kill
   }
   else
   {
    _health -= damageToDeal;
    PushPlayerClientRpc(force);
   }
  }
  
  #endregion
 }
}
