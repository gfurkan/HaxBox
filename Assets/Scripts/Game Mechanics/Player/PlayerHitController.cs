using System;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Player
{

 public class PlayerHitController : NetworkBehaviour,IDamagable
 {
  #region Fields

  [SerializeField] private int _health = 0;
  [SerializeField] private float _attackRange;
  [SerializeField] private int _attackPower;

  public static event Action<Vector2> OnDamagedWithPush;

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
     Attack();
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

  private void Attack()
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
      enemy.DealDamage(_attackPower,_pushForce);
     }
    }
   }
  }
  
  #endregion

  #region Public Methods
  
  public void DealDamage(int damageToDeal,Vector2 force)
  {
   if (_health <= damageToDeal)
   {
    // kill
   }
   else
   {
    OnDamagedWithPush?.Invoke(force);
    _health -= damageToDeal;
   }
  }
  
  #endregion
 }
}
