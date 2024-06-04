using Unity.Netcode;
using UnityEngine;

namespace Player
{

 public class PlayerNetwork : NetworkBehaviour
 {
  #region Fields

  [SerializeField] private Rigidbody2D _rb;
  private readonly NetworkVariable<Vector2> _netPosition = new (writePerm: NetworkVariableWritePermission.Owner);
  private readonly NetworkVariable<Vector2> _netVelocity = new (writePerm: NetworkVariableWritePermission.Owner);

  #endregion

  #region Properties

  #endregion

  #region Unity Methods

  void Update()
  {
   SyncData();
  }
  #endregion

  #region Private Methods

  void SyncData()
  {
   if (IsOwner)
   {
    //_netPosition.Value = transform.position;
    _netVelocity.Value = _rb.velocity;
   }
   else
   {
    //transform.position = _netPosition.Value;
    _rb.velocity = _netVelocity.Value;
   }
  }
  #endregion

  #region Public Methods

  #endregion
  

 }
 
}
