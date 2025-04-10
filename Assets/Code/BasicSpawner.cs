using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code
{
    public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private Transform _spawnTransform;
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
        private NetworkRunner _networkRunner;

        private bool _spaceButton;
        private bool _interactButton;
        private NetworkObject _networkPlayerObject;
        private int _joinOrder;

        [Header("Movement Speeds")] [SerializeField]
        private float _walkSpeed = 3f;

        [SerializeField] private float _sprintMultiplier = 2f;
        [SerializeField] private float _mouseSensitivity = 0.1f;

        private Vector3 _currentMovement;

        private float CurrentSpeed => _walkSpeed * (_playerInputHandler.SprintTriggered ? _sprintMultiplier : 1f);

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                // Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
                Vector3 spawnPosition = new Vector3(_spawnTransform.position.x + _joinOrder, _spawnTransform.position.y, _spawnTransform.position.z);
                _joinOrder++;
                _networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

                _spawnedCharacters.Add(player, _networkPlayerObject);
            }
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        private void Update()
        {
            _spaceButton = _spaceButton || Input.GetKey(KeyCode.Space);
            _interactButton = _interactButton || Input.GetKeyDown(KeyCode.E);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            NetworkInputData _data = new NetworkInputData();

            // if (Input.GetKey(KeyCode.W))
            //     data.direction += Vector3.forward;
            //
            // if (Input.GetKey(KeyCode.S))
            //     data.direction += Vector3.back;
            //
            // if (Input.GetKey(KeyCode.A))
            //     data.direction += Vector3.left;
            //
            // if (Input.GetKey(KeyCode.D))
            //     data.direction += Vector3.right;

            // data.vertical = Input.GetAxis("Vertical");
            // data.horizontal = Input.GetAxis("Horizontal");
            // data.buttons.Set(NetworkInputData.SPACEBUTTON, _spaceButton);
            // _spaceButton = false;
            //
            // Vector3 worldDirection = CalculateWorldDirection();
            // _currentMovement.x = worldDirection.x * CurrentSpeed;
            // _currentMovement.z = worldDirection.z * CurrentSpeed;
            // _data.direction = _currentMovement;
            
            _data.buttons.Set(NetworkInputData.INTERACTBUTTON, _interactButton);
            _interactButton = false;

            _data.direction = new Vector3(_playerInputHandler.MovementInput.x, 0, _playerInputHandler.MovementInput.y);
            _data.currentSpeed = CurrentSpeed;
            _data.mouseXRotation = _playerInputHandler.RotationInput.x * _mouseSensitivity;
            _data.mouseYRotation = _playerInputHandler.RotationInput.y * _mouseSensitivity;

            if (LocalInputSender.Local != null)
            {
                _data.holdPosition = LocalInputSender.Local.HoldPosition;
            }
            
            input.Set(_data);
        }

        // private Vector3 CalculateWorldDirection()
        // {
        //     Vector3 inputDirection = new Vector3(_playerInputHandler.MovementInput.x, 0, _playerInputHandler.MovementInput.y);
        //     Vector3 worldDirection = transform.TransformDirection(inputDirection);
        //     return worldDirection.normalized;
        // }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        private async void StartGame(GameMode mode)
        {
            _networkRunner = gameObject.AddComponent<NetworkRunner>();
            _networkRunner.ProvideInput = true;

            SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            await _networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

        }

        private void OnGUI()
        {
            if (_networkRunner == null)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                {
                    StartGame(GameMode.Host);
                }

                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    StartGame(GameMode.Client);
                }
            }
        }
    }
}