using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField]
    private BoardManager _boardScript; // ����Ŵ��� ��ũ��Ʈ�� ���۷���

    private int _enemySpawnLevel = 3;

    public int EnemySpawnLevel { get => _enemySpawnLevel; set => _enemySpawnLevel = value; }

    #region instance
    public static GameManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _boardScript = GetComponent<BoardManager>();
        InitGame();
    }
    #endregion

    private void InitGame() {
        _boardScript.SetupScene(EnemySpawnLevel);
    }
}
