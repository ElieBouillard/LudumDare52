using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;

    private Vector3 _targetPos;
    
    public void Initialize(Vector3 pos)
    {
        _targetPos = pos;
    }
    
    private void Update()
    {
        transform.position += (_targetPos - transform.position).normalized * _speed * Time.deltaTime;

        Debug.Log((_targetPos - transform.position).magnitude);
        
        if ((_targetPos - transform.position).magnitude <= 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
