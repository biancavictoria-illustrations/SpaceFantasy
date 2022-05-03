using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JournalUI/JournalContentCrew")]
public class JournalContentCrew : JournalContent
{
    [SerializeField] private string jobTitle;
    [SerializeField] private string birthday;
    [SerializeField] private string placeOfBirth;
    [SerializeField] private string race;
    [SerializeField] private string height;

    [TextArea(4,20)]
    [SerializeField] private string strengths;
    [TextArea(4,20)]
    [SerializeField] private string weaknesses;

    public string JobTitle()
    {
        return jobTitle;
    }

    public string Birthday()
    {
        return birthday;
    }

    public string PlaceOfBirth()
    {
        return placeOfBirth;
    }

    public string Race()
    {
        return race;
    }

    public string Height()
    {
        return height;
    }

    public string Strengths()
    {
        return strengths;
    }

    public string Weaknesses()
    {
        return weaknesses;
    }
}
