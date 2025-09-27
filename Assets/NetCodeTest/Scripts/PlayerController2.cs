using System;
using NetCodeTest.Scripts.Interface;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetCodeTest
{
    // Client Orientedなサンプルコード
    public class PlayerController2 : NetworkBehaviour, IDamagable
    {
        [SerializeField] private float moveSpeed = 5.0f; // 移動速度
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private TextMeshProUGUI hpText;
        

        private PlayerInputAction _input;
        
        private bool _isWelcome = false;

        public NetworkVariable<int> HP = new(100);
        
        private void Awake()
        {
            _input = new PlayerInputAction();
            _input.main.WelcomeOn.performed += SetWelcome;
            _input.main.WelcomeOff.performed += UnsetWelcome;
            _input.main.Fire.performed += OnFire;
        }

        private void Start()
        {
        }
        
        private void OnEnable()
        {
            _input.Enable();
        }
        
        private void OnDisable()
        {
            _input.Disable();
        }

        private void SetWelcome(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            Debug.Log("Welcome");
            setWelcomeflagRpc(true);
        }
        
        private void UnsetWelcome(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            Debug.Log("Bye");
            setWelcomeflagRpc(false);
        }

        private void OnFire(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            Debug.Log("Fire");
            GenerateBulletServerRpc(this.transform.position, this.transform.rotation);
        }

        [Rpc(SendTo.Everyone)]
        private void setWelcomeflagRpc(bool flag)
        {
            _isWelcome = flag;
        }

        [ServerRpc]
        private void GenerateBulletServerRpc(Vector3 position, Quaternion rotation)
        {
            if (!IsServer) return;
            //弾の生成処理
            Debug.Log("GenerateBulletServer");
            
            //プレイヤーの少し前に生成する
            var bullet = Instantiate(bulletPrefab, position + this.transform.forward * 1f + Vector3.up * 1f, this.transform.rotation);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 40f, ForceMode.Impulse);
        }

        [ServerRpc]
        public void DamageServerRpc(int damage)
        {
            if (!IsServer) return;
            HP.Value -= damage;
            Debug.Log($"被弾。　HP: {HP.Value}");
        }
        
        private void Update()
        {
            var inputVector = _input.main.Move.ReadValue<Vector2>();
            if (IsOwner)
            {
                Debug.Log("Input: " + inputVector);
                this.GetComponent<Rigidbody>().linearVelocity= new Vector3(inputVector.x, 0, inputVector.y) * moveSpeed;
                //入力ベクトルの方を向かせる
                if (inputVector != Vector2.zero)
                {
                    this.transform.rotation = Quaternion.LookRotation(new Vector3(inputVector.x, 0, inputVector.y));
                }
            }
            this.GetComponent<Animator>().SetBool("Welcome", _isWelcome);
        }
    }
}