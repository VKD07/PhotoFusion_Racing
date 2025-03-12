using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code
{
    public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
        private NetworkRunner _networkRunner;
        private bool _spaceButton;
        private NetworkObject _networkPlayerObject;
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        private int joinOrder = 0;

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Vector3 spawnPosition = new Vector3(joinOrder * 10, 1, 0);
                joinOrder++;

                _networkPlayerObject =
                    runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
           
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
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();

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

            data.vertical = Input.GetAxis("Vertical");
            data.horizontal = Input.GetAxis("Horizontal");
            data.buttons.Set(NetworkInputData.SPACEBUTTON, _spaceButton);
            _spaceButton = false;

            input.Set(data);
        }

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