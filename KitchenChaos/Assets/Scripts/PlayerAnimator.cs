using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Animator animator;
    [SerializeField] private Player player;

    private void Update()
    {
        if (IsOwner)
        {
            animator.SetBool(IS_WALKING, player.IsWalking);
        }
    }

}
