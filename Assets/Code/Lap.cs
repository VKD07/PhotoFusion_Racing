using System;
using Code;
using Fusion;
using TMPro;
using UnityEngine;

public class Lap : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    
    // private void OnTriggerEnter(Collider other)

    // {
    //     if (other.TryGetComponent<CarController>(out CarController player))
    //     {
    //         RPC_SendMessage("Hey Mate!");
    //     }
    // }
    
    // [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    // public void RPC_SendMessage(string message, RpcInfo info = default)
    // {
    //     RPC_RelayMessage(message, info.Source);
    // }
    //
    // [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    // public void RPC_RelayMessage(string message, PlayerRef messageSource)
    // {
    //     _text.text = message;
    //
    //     // if (messageSource == Runner.LocalPlayer)
    //     // {
    //     //     message = $"You said: {message}\n";
    //     // }
    //     // else
    //     // {
    //     //     message = $"Some other player said: {message}\n";
    //     // }
    //
    //     // _messages.text += message;
    // 
    
    
}