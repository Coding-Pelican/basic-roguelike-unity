using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {
    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rb2D;
    private float _inverseMoveTime; // �̵� ����� ȿ�������� �ϱ� ���� ����

    public float InverseMoveTime { get => _inverseMoveTime; set => _inverseMoveTime = value; }

    public LayerMask blockingLayer;
    public float moveTime = 0.1f;

    protected virtual void Start() {
        _boxCollider = GetComponent<BoxCollider2D>();
        _rb2D = GetComponent<Rigidbody2D>();
        InverseMoveTime = 1f / moveTime; // �̸� ������ ����� �صξ ���� �����Ⱑ �ƴ� ���ϱⰡ �����ϰ� ��
    }

    protected IEnumerator SmoothMovement(Vector3 end) {
        // ���� �Ÿ��� �븧 ���, �������� ����ϴ� ���� ����� �����Ͽ� �������� ���
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon) { // 0�� ���� ������ ��(���Ƿ�)���� ū ��� ��ƾ�� ���ư�
            Vector3 newPosition = Vector3.MoveTowards(_rb2D.position, end, InverseMoveTime * Time.deltaTime); // �ð��� ����Ͽ� �������� ���ϴ� �� ��ġ ���
            _rb2D.MovePosition(newPosition); // ���� �� ��ġ�� �̵�
            sqrRemainingDistance = (transform.position - end).sqrMagnitude; // ���� �Ÿ� ����
            yield return null; // ���� �Ÿ��� 0�� ������ ������ ����
        }
    }

    protected abstract void OnCantMove<T>(T component)
    where T : Component;

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        _boxCollider.enabled = false; // Raycast ���� ������ �ݶ��̴��� �°� �Ǵ� ���� ����
        hit = Physics2D.Linecast(start, end, blockingLayer); // start���� end�� ������ ĳ��Ʈ ��
        _boxCollider.enabled = true;
        if (hit.transform == null) { // ĳ���� �� ���ο� �ɸ��� ���� ���� ������ �� �ִٸ� �ڷ�ƾ ����
            StartCoroutine(SmoothMovement(end));
            return true;

        }
        return false; // �ɸ��� ���� ������ ������ �� ����
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component {
        // Move�� ȣ��Ǿ����� Linecast�� ������ �Ǵ� ���� ������ ����
        bool canMove = Move(xDir, yDir, out RaycastHit2D hit);
        if (hit.transform == null) return; // �ɸ��� ���� ������ �ڵ带 ��ħ
        T hitComponent = hit.transform.GetComponent<T>(); // hit�� ���� ������Ʈ ���۷����� ��� ��
        if (!canMove && hitComponent != null) OnCantMove(hitComponent);
    }

}
