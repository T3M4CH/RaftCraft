using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MonoShark : MonoBehaviour
{
    [SerializeField] private float moveTime;
    [SerializeField] private float rotateTime;
    [SerializeField] private Transform firstPosition;
    [SerializeField] private Transform secondPosition;

    private Sequence _sequence;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
        _transform.position = firstPosition.position;

        StartCoroutine(SharkMove());
    }

    private IEnumerator SharkMove()
    {
        while (true)
        {
            var currentRotateTime = 0f;
            var startRotation = _transform.rotation;
            while (currentRotateTime < rotateTime)
            {
                yield return null;
                currentRotateTime += Time.deltaTime;
                _transform.rotation = Quaternion.Lerp(startRotation, secondPosition.rotation, currentRotateTime / rotateTime);
            }

            var currentMoveTime = 0f;
            var startPosition = _transform.position;

            while (currentMoveTime < moveTime)
            {
                yield return null;
                currentMoveTime += Time.deltaTime;
                _transform.position = Vector3.Lerp(startPosition, secondPosition.position, currentMoveTime / moveTime);
            }

            currentRotateTime = 0;
            startRotation = _transform.rotation;

            while (currentRotateTime < rotateTime)
            {
                yield return null;
                currentRotateTime += Time.deltaTime;
                _transform.rotation = Quaternion.Lerp(startRotation, firstPosition.rotation, currentRotateTime / rotateTime);
            }

            currentMoveTime = 0f;
            startPosition = _transform.position;

            while (currentMoveTime < moveTime)
            {
                yield return null;
                currentMoveTime += Time.deltaTime;
                _transform.position = Vector3.Lerp(startPosition, firstPosition.position, currentMoveTime / moveTime);
            }
        }
    }
}