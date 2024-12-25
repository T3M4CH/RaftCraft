using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GtapParticleUi : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(WaitDestroy());
    }

    private IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(4f);
        gameObject.SetActive(false);
    }
}
