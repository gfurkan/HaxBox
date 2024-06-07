using UnityEngine;

namespace Interfaces
{
 public interface IDamagable
 {
  #region Public Methods

  public void DealDamageServerRpc(int damageToDeal,Vector2 force);

  #endregion
 }
}
