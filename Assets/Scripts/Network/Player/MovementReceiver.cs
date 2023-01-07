using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementReceiver : MonoBehaviour
{
    private Vector3? _targetPos;
    private Quaternion? _targetRot;

    public void SetState(Vector3 pos, Quaternion rot)
    {
        _targetPos = pos;
        _targetRot = rot;
    }

    private void Update()
    {
        if (_targetPos != null)
            transform.position = Vector3.Lerp(transform.position, _targetPos.Value, 10f * Time.deltaTime);

        if (_targetRot != null)
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot.Value, 10f * Time.deltaTime);
    }
}
