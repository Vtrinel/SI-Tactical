using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Discs/Disc Informations", fileName = "New Disc Informations")]
public class DiscInformations : ScriptableObject
{
    public DiscType discType = DiscType.Piercing;
    public string tooltipName = "Disc";
    [TextArea] public string tooltipDescription = "This is a Disc";
    public int damagesValue = 2;
}
