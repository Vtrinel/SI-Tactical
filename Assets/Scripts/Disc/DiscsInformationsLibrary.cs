using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Discs/Discs Informations Library", fileName = "New Disc Informations")]
public class DiscsInformationsLibrary : ScriptableObject
{
    [SerializeField] List<DiscInformations> discInformations = new List<DiscInformations>();
    Dictionary<DiscType, DiscInformations> discsDictionnary = new Dictionary<DiscType, DiscInformations>();

    public DiscInformations GetDiscInformations(DiscType discType)
    {
        if (discInformations.Count == 0)
            return null;

        if (discsDictionnary.Count == 0)
            GenerateDiscsDictionary();

        if (discsDictionnary.ContainsKey(discType))
        {
            return discsDictionnary[discType];
        }

        return null;
    }

    public void GenerateDiscsDictionary()
    {
        discsDictionnary = new Dictionary<DiscType, DiscInformations>();
        foreach (DiscInformations discInfo in discInformations)
        {
            discsDictionnary.Add(discInfo.discType, discInfo);
        }
    }
}
