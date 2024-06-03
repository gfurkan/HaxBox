using UnityEngine;

namespace Interfaces
{
 public interface IDamagable
 {
  #region Public Methods

  public void DealDamage(int damageToDeal,Vector2 force);

  #endregion
 }
}
