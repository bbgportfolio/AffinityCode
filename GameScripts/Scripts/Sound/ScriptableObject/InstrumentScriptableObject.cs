using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InstrumentScriptableObject")]
public class InstrumentScriptableObject : ScriptableObject
{
    public AudioClip[] audioClips;
}
