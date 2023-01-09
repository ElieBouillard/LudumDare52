using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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
}
