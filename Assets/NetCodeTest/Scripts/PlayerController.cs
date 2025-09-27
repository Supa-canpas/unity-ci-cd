//  NetworkBehaviourを継承する

using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Vector2 _moveInput;
    
    private bool _welcome = false;
    
    private void FixedUpdate()
    {
        // 接続時に生成されたPlayerPrefabsのオブジェクトは、接続したクライアントがオーナー属性を持っています
        
        // IsOwner判定処理を加えないと、他プレイヤーのオブジェクトも操作してしまうことになります
        if (IsOwner)
        {
            Vector2 _input = new Vector2();
            if (Input.GetKey(KeyCode.W))
            {
                _input.y = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _input.y = -1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _input.x = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _input.x = -1;
            }
            
           
            Debug.Log(_input);

            SetMoveInputServerRPc(_input.x, _input.y);

        }

        if (IsServer)
        {
            // サーバー側は移動処理を実行
            Move();
        }
    }

    [Unity.Netcode.ServerRpc]
    private void SetMoveInputServerRPc(float x, float y)
    {
        // 代入した値は、サーバー側のオブジェクトにセットされる
        _moveInput = new Vector2(x, y);
    }
    

    private void Move()
    {
        // ServerRpcによってクライアント側から変更されている_moveInput
        var moveVector = new Vector3(_moveInput.x, 0, _moveInput.y);
        this.GetComponent<Rigidbody>().linearVelocity = moveVector;
    }
}