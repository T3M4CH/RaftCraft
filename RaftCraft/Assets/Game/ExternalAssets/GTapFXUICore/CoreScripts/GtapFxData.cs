using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GtapFXUi/GtapFXUiData", fileName = "GtapFXUiData")]
public class GtapFxData : ScriptableObject
{
    public GameObject CanvasPrefab;

    public Vector3 PositionSpawn;
    
    public GameObject[] PositiveEmojies;

    public GameObject[] NegativaEmojies;
}
