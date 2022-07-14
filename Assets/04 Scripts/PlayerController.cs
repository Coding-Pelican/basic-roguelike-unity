using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MovingObject {
    private Animator _animator;
    private int _food;

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public TextMeshProUGUI foodText;

    protected override void Start() {
        _animator = GetComponent<Animator>();
        _food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + _food;
        base.Start();
    }

    private void OnDisable() {
        GameManager.instance.playerFoodPoints = _food;
    }

    private void CheckIfGameOver() {
        if (_food <= 0)
            GameManager.instance.GameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir) {
        _food--;
        foodText.text = "Food: " + _food;
        base.AttemptMove<T>(xDir, yDir); // �θ� Ŭ������ AttemptMove ȣ��
        CheckIfGameOver();
        GameManager.instance.isPlayersTurn = false;
    }

    void Update() {
        if (!GameManager.instance.isPlayersTurn) return;
        // 1�Ǵ� -1�� ����, ���� ���� �����
        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");
        // ���η� �������ٸ� ���δ� 0
        if (horizontal != 0) vertical = 0;
        // ��� �����ε� �����̶�� ����� �־��ٸ�
        if (horizontal != 0 || vertical != 0) AttemptMove<Wall>(horizontal, vertical); //�÷��̾�� ������ ��ȣ�ۿ��� �� �����Ƿ� ���ʸ��� Wall�� �Ѱ���
    }

    protected override void OnCantMove<T>(T component) {
        // �μ��� ���� component�� Wall�� ĳ����
        Wall hitWall = component as Wall;
        // DamageWall �޼ҵ�� �������� ��
        hitWall.DamageWall(wallDamage);
        _animator.SetTrigger("playerAttack");
    }

    private void Restart() {
        SceneManager.LoadScene(0);
    }

    public void LoseFood(int loss) {
        // �ִϸ����� ���� ����
        _animator.SetTrigger("playerHit");
        _food -= loss; // ���� ����Ʈ ����
        foodText.text = "-" + loss + " Food: " + _food;
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Exit")) {
            Invoke(nameof(Restart), restartLevelDelay);
            enabled = false;
        } else if (other.CompareTag("Food")) {
            _food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food:" + _food;
            other.gameObject.SetActive(false);
        } else if (other.CompareTag("Soda")) {
            _food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food:" + _food;
            other.gameObject.SetActive(false);
        }
    }
}
