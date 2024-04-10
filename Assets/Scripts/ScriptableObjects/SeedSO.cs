using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Seed")]
public class SeedSO : ScriptableObject
{
    public string seedName = "New Seed";
    public Sprite seedIcon;
}
