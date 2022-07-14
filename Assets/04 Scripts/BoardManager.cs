using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[SerializeField]
public class Count {
    public int minimum;
    public int maximum;

    public Count(int minimum, int maximum) {
        this.minimum = minimum;
        this.maximum = maximum;
    }
}

public class BoardManager : MonoBehaviour {
    private Transform _boardHolder;
    private List<Vector3> _gridPositions = new List<Vector3>();

    public List<Vector3> GridPositions { get => _gridPositions; set => _gridPositions = value; }

    public int columns = 16;
    public int rows = 16;
    public Count wallCount = new Count(5, 19); // ���� �� ���� ����,���Ѱ�
    public Count foodCount = new Count(1, 9); // ���� �� ���� ����,���Ѱ�

    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;

    void InitialiseList() {
        GridPositions.Clear();
        for (int x = 1; x < columns - 1; x++) {
            for (int y = 1; y < rows - 2; y++) {
                GridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // outerWall�� floor ����
    void SetupBoard() {
        // �� Board ������Ʈ�� �ν��Ͻ�ȭ�ϰ� �� Ʈ�������� ����Ȧ���� ����
        _boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++) {
            for (int y = -1; y < rows + 1; y++) {
                // �ٴ�Ÿ�� 8�� �� �ϳ��� �������� �������� ��
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                // ���� ��ġ�� �׵θ���� �ܺ�Ÿ�� �� ��� �ٽ� ����
                if (x == -1 || x == columns || y == -1 || y == rows) {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                // �� �������� ���� ��ȸ���� ��ġ�� �ν��Ͻ�ȭ�Ͽ� ����
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                // ������ �ν��Ͻ��� Ʈ�������� boardHolder�� �ڽ����� ��
                instance.transform.SetParent(_boardHolder);
            }
        }
    }

    // gridPosition���� �������� �ϳ� �� �� �ְ� ��
    Vector3 RandomizePosition() {
        // gridPositon�� ���� ������ �������� �ε��� ����
        int randomIndex = Random.Range(0, GridPositions.Count);
        // �������� ������ �ε����� �ش��ϴ� gridPositons�� �����ϴ� ��������
        Vector3 randomPosition = GridPositions[randomIndex];
        // randomIndex�� �ش��ϴ� gridPositions�� �����Ͽ� ��밡���ϰ� ��
        GridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // �־��� ��/���� �� ������ �������� ������ Ÿ���� ��������
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
        // �������� ���� ����
        int objectCount = Random.Range(minimum, maximum + 1);
        // ���� ������ŭ ������Ʈ ����
        for (int i = 0; i < objectCount; i++) {
            Vector3 randomPosition = RandomizePosition();
            // �־��� Ÿ�� ���� ������ ��� ����
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // �� Ÿ���� �ν��Ͻ�ȭ
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level) {
        SetupBoard();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        // ���� ���� ������ ���� �α��Լ������� ����
        // ���� ���� 2���� �� 1, 4���� �� 2, 8���� �� 3
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        // ���������� exit�� ���ܿ� ����
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

}
