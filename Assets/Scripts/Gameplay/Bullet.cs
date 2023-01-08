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

        StartCoroutine(test());
        
        Destroy( gameObject, 1f);
    }

    private IEnumerator test()
    {
        yield return new WaitForSeconds(0.05f);

        transform.position = _targetPos;
    }
    
    private void Update()
    {
        // transform.position += (_targetPos - transform.position).normalized * _speed * Time.deltaTime;
        //
        // if ((_targetPos - transform.position).magnitude <= 1f)
        // {
        //     Destroy(gameObject);
        // }
        
        
    }
}
