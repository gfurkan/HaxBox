using UnityEngine;

namespace Interfaces
{
 public interface IDamagable
 {
  #region Public Methods

  public void DealDamageClientRpc(int damageToDeal,Vector2 force);

  #endregion
 }
}
