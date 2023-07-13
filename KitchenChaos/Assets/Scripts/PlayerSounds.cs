using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    [SerializeField] Player player;
        
    private float footstepTimer;
    private float footstepTimerMax = 0.1f;
    private float volume = 1;

    private void Update()
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer < 0)
        {
            footstepTimer = footstepTimerMax;

            if (player.IsWalking)
            {
                SoundManager.Instance.PlayFootstepSound(player.transform.position, volume);
            }
        }
    }

}
