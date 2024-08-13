using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Class",menuName ="Class/Class")]
public class SOClass : ScriptableObject
{
    public ClassType ClassType;
    public string ClassName;
    public Sprite ClassIcon;
}
