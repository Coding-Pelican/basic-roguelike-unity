using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovingObject {
    public int playerDamage;

    private Animator _animator;
    private Transform _target;
    private bool _skipMove;

    public bool SkipMove { get => _skipMove; set => _skipMove = value; }

    protected override void Start() {
        GameManager.instance.AddEnemyToList(this);
        _animator = GetComponent<Animator>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir) {
        if (SkipMove) { // �������� �ǳʶپ�� �Ѵٸ�
            SkipMove = false; // ���� ���� ���� false�� ����� �ǳ� ��
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        // Enemy�� �̹� ���������Ƿ� true�� ����
        SkipMove = true;
    }

    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon) { // ���� x�������� ���� ��
            yDir = _target.position.y > transform.position.y ? 1 : -1; // y�� �󿡼� Ÿ�� ������ ��ĭ ������ ��
        } else { // ���� x �������� �ٸ��ٸ�
            xDir = _target.position.x > transform.position.y ? 1 : -1; // x�� �󿡼� Ÿ�� ������ �� ĭ ������ ��
        }
        AttemptMove<PlayerController>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component) {
        // component�� Player�� ĳ��Ʈ�Ͽ� ����
        PlayerController hitPlayer = component as PlayerController;
        // ���� ��������ŭ ��������Ʈ�� �ٿ���
        hitPlayer.LoseFood(playerDamage);
        _animator.SetTrigger("enemyAttack");
    }
}
