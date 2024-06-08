using System.Threading.Tasks;
using Interfaces;
using Managers;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHitController : NetworkBehaviour, IDamagable
    {
        #region Fields
        
        [Header("Values")]
        [SerializeField] private float _startHealth = 0;
        [SerializeField] private float _attackRange;
        [SerializeField] private float _pushSpeed = 0;
        [SerializeField] private int _attackPower;
        
        [Header("Objects")]
        [SerializeField] private PlayerNetwork _playerNetwork;
        [SerializeField] private ParticleSystem _deathParticle;
        [SerializeField] private Image _healthFillImage;

        private float _currentHealth = 0;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            UIManager.Instance.AttackButton.onClick.AddListener(Attack);
            _currentHealth = _startHealth;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                if (IsHost)
                {
                    SetColor(Color.blue);
                }
                else
                {
                    SetColor(Color.red);
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

        [ServerRpc (RequireOwnership = false)]
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
                        enemy.DealDamageServerRpc(_attackPower, _pushForce);
                    }
                }
            }
        }

        [ClientRpc]
        private void ControlHealthFillImageClientRpc(float health)
        {
            float fillAmount = health.Map(0, _startHealth, 0, 1);
            _healthFillImage.fillAmount = fillAmount;
        }
        
        [ClientRpc]
        private void ControlFinishScreenClientRpc(ulong loserId)
        {
            ControlFinishScreen(loserId);
        }

        private async void ControlFinishScreen(ulong loserId)
        {
            await Task.Delay(1000);
            if (loserId != NetworkManager.Singleton.LocalClientId)
            {
                UIManager.Instance.ShowResultScreen(false);
            }
            else
            {
                UIManager.Instance.ShowResultScreen(true);
            }
        }
        [ClientRpc]
        private void PushPlayerClientRpc(Vector2 forceDirection)
        {
            _playerNetwork.Rb.AddForce(forceDirection * _pushSpeed);
        }
        [ClientRpc]
        private void SpawnParticleEffectClientRpc()
        {
            var particleSystem = Instantiate(_deathParticle, transform.position, quaternion.identity);
            var main = particleSystem.main;
            main.startColor = _playerNetwork.PlayerRenderer.material.color;
            gameObject.SetActive(false);
        }
        private void SetColor(Color color)
        {
            _playerNetwork.PlayerRenderer.material.color = color;
            _playerNetwork.AttackRenderer.material.color = color;
        }

        private void Attack()
        {
            if (IsOwner)
            { 
                AttackServerRpc();
            }
        }
        #endregion

        
        #region Public Methods
        
        [ServerRpc(RequireOwnership = false)]
        public void DealDamageServerRpc(int damageToDeal, Vector2 force)
        {
            if (_currentHealth <= damageToDeal)
            {
                SpawnParticleEffectClientRpc();
                ControlHealthFillImageClientRpc(0);
                ControlFinishScreenClientRpc(OwnerClientId);
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
