using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IManager
{
    private static PlayerManager Instance;

    [Header("Player Info")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 playerStartPosition;
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetUp()
    {
        var playerObject = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity, transform);
        player = playerObject.GetComponent<Player>();
    }

    public static PlayerManager GetInstance()
    {
        return Instance;
    }

    public static Player GetPlayer()
    {
        return Instance.player;
    }

    public static void SubscribeToOnPlayerJump(Action listener)
    {
        GetPlayer()
            .GetMovementHandle()
            .AddOnJumpListener(listener);
    }
    
    public static void SubscribeToOnPlayerSprint(Action listener)
    {
        GetPlayer()
            .GetMovementHandle()
            .AddOnSprintListener(listener);
    }
    
    public static void SubscribeToOnPlayerStaminaMultiplierChange(Action<float> listener)
    {
        ((PlayerStats)GetPlayer()
            .GetStatistics())
            .AddStaminaUseMultiplierChangeListener(listener);
    }
    
    public static void SubscribeToOnPlayerStaminaChange(Action<float> listener)
    {
        ((PlayerStats)GetPlayer()
            .GetStatistics())
            .AddStaminaChangeListener(listener);
    }
}
