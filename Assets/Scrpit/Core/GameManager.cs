using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player Player => player;
    Player player;

    bool isKidnaped = false;

    public Action onGameStart;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        player.onDie += GameOver;
    }

    public void GameStart() 
    {
        onGameStart?.Invoke();
    }

    public void GameOver() 
    {
        //마지막 저장으로 이동
    }
}
