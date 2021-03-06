using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {
    public int playerFoodPoints = 50;
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    [HideInInspector] public bool isPlayersTurn = true;

    public int Level { get => level; set => level = value; }
    public bool IsAnyEnemyMoving { get => _isAnyEnemyMoving; set => _isAnyEnemyMoving = value; }
    public bool IsDoingSetup { get => _isDoingSetup; set => _isDoingSetup = value; }
    public BoardManager BoardScript { get => _boardScript; set => _boardScript = value; }

    [SerializeField] private int level = 1;
    private List<EnemyController> _enemies;
    private GameObject _levelImage;
    private TextMeshProUGUI _levelText;
    private bool _isAnyEnemyMoving;
    private bool _isDoingSetup;

    #region instance
    public static GameManager instance = null;
    [SerializeField] private BoardManager _boardScript; // 보드매니저 스크립트의 레퍼런스

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        _enemies = new List<EnemyController>();
        BoardScript = GetComponent<BoardManager>();
        //InitGame();
    }
    #endregion

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        Level++;
        InitGame();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnLevelFinishedLoading; // OnLevelFinishedLoading 메소드에게 씬변화 이벤트를 듣도록 시킴

    private void OnDisable() => SceneManager.sceneLoaded -= OnLevelFinishedLoading;

    private void Update() {
        if (isPlayersTurn || IsAnyEnemyMoving || IsDoingSetup) return;
        StartCoroutine(MoveEnemies());
    }

    private void InitGame() {
        IsDoingSetup = true;
        _levelImage = GameObject.Find("LevelImage");
        _levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        _levelText.text = "Day " + (Level - 1);
        _levelImage.SetActive(true);
        Invoke(nameof(HideLevelImage), levelStartDelay);

        _enemies.Clear();
        BoardScript.SetupScene(Level);
    }

    private void HideLevelImage() {
        _levelImage.SetActive(false);
        IsDoingSetup = false;
    }

    public void GameOver() {
        _levelText.text = "After " + Level + "days, you starved.";
        _levelImage.SetActive(true);
        enabled = false;
    }

    public void AddEnemyToList(EnemyController enemy) {
        _enemies.Add(enemy);
    }

    IEnumerator MoveEnemies() {
        IsAnyEnemyMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (_enemies.Count.Equals(0)) {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < _enemies.Count; i++) {
            _enemies[i].MoveEnemy();
            yield return new WaitForSeconds(_enemies[i].moveTime);
        }
        isPlayersTurn = true;
        IsAnyEnemyMoving = false;
    }
}
