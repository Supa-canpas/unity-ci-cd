using System;
using NetCodeTest.Scripts.Interface;
using Unity.Netcode;
using UnityEngine;

namespace NetCodeTest.Scripts
{
    public class BallController: NetworkBehaviour
    {
        private void Start()
        {
            Invoke(nameof(DestroyThisRpc),10);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsSpawned && collision.transform.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.DamageServerRpc(1);
                Debug.Log("攻撃！！！！");
            }
        }
        
        [Rpc(SendTo.Server)]
        private void DestroyThisRpc()
        {
            Destroy(gameObject);
        }
    }
}