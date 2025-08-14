using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BeastMover))]
public class Beast : MonoBehaviour
{
    [SerializeField] private float _speed;

    private BeastMover _beastMover;
    private SnakeHead _snakeHead;
    private List<Vector3> _road;    

    private void Awake()
    {
        _beastMover = GetComponent<BeastMover>();
    }

    public void Init(SnakeHead snakeHead, List<Vector3> road)
    {
        _snakeHead = snakeHead;
        _road = road;
    }
}
