using Interfaces;
using Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHitController : NetworkBehaviour, IDamagable
    {
        #region Fields

        [SerializeField] private float _startHealth = 0;
        [SerializeField] private float _attackRange;
        [SerializeField] private int _attackPower;
        [SerializeField] private float _pushSpeed = 0;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Image _healthFillImage;

        private float _currentHealth = 0;
        private NetworkVariable<int> attackPowerByDistance = new NetworkVariable<int>();
        private int ist = 0;

        private NetworkVariable<GameEndControls> testClient = new NetworkVariable<GameEndControls>(
            new GameEndControls {
            isGameFinished=false,
            loserId=0,
        },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
        
        public struct GameEndControls : INetworkSerializable
        {
            public bool isGameFinished;
            public int loserId;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref isGameFinished);
                serializer.SerializeValue(ref loserId);
            }
        }
        #endregion

        #region Properties

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentHealth = _startHealth;
        }

        public override void OnNetworkSpawn()
        {
            testClient.OnValueChanged += (GameEndControls prev, GameEndControls next) => { print(testClient.Value.isGameFinished  + "  +  " + testClient.Value.loserId); };
        }
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }

        #endregion

        #region Private Methods

        [ServerRpc]
        private void AttackServerRpc()
        {
            Collider2D[] _enemies = Physics2D.OverlapCircleAll(transform.position, _attackRange);

            for (int i = 0; i < _enemies.Length; i++)
            {
                if (_enemies[i].gameObject != gameObject)
                {
                    if (_enemies[i].TryGetComponent(out IDamagable enemy))
                    {
                        Vector2 _pushForce = _enemies[i].transform.position - transform.position;
                        _pushForce.Normalize();
                        int newAttackPower = 0;
                        
                        if (IsHost)
                        {
                            float lineDistance = Vector2.Distance(GameManager.Instance.ClientCollider.transform.position, GameManager.Instance.HostCollider.transform.position);
                            float _distance = Vector2.Distance(_enemies[i].transform.position, GameManager.Instance.HostCollider.transform.position);
                            newAttackPower = (int)(_distance.Map(0, lineDistance, 1, 2) * _attackPower);
                        }
                        else if (IsClient)
                        {
                            float lineDistance = Vector2.Distance(GameManager.Instance.ClientCollider.transform.position, GameManager.Instance.HostCollider.transform.position);
                            float _distance = Vector2.Distance(_enemies[i].transform.position, GameManager.Instance.ClientCollider.transform.position);
                            newAttackPower = (int)(_distance.Map(0, lineDistance, 1, 2) * _attackPower);
                        }
                        enemy.DealDamageServerRpc(newAttackPower, _pushForce);
                    }
                }
            }
        }

        [ClientRpc]
        void ControlHealthFillImageClientRpc(float health)
        {
            float fillAmount = health.Map(0, _startHealth, 0, 1);
            _healthFillImage.fillAmount = fillAmount;
        }
        
        [ClientRpc]
        void TestClientRpc(ulong loserId)
        {
            if (loserId != NetworkManager.Singleton.LocalClientId)
            {
                UIManager.Instance.ShowWinScreen();
            }
            else
            {
                UIManager.Instance.ShowLoseScreen();
            }

        }

        #endregion

        #region Public Methods
        
        [ClientRpc]
        private void PushPlayerClientRpc(Vector2 forceDirection)
        {
            _rb.AddForce(forceDirection * _pushSpeed);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DealDamageServerRpc(int damageToDeal, Vector2 force)
        {
            if (_currentHealth <= damageToDeal)
            {
                // kill
                ControlHealthFillImageClientRpc(0);
                TestClientRpc(OwnerClientId);
            }
            else
            {
                _currentHealth -= damageToDeal;
                ControlHealthFillImageClientRpc(_currentHealth);
                PushPlayerClientRpc(force);
            }
        }

        #endregion
    }
}
