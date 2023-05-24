using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NicknamesStorage", fileName = "NicknamesStorage")]
public class NicknamesStorage : ScriptableObject
{
    [SerializeField] private string[] nicknames = {
        "Angon",
        "Animus",
        "Stream Elements",
        "Core Philosophies",
        "Given Moment",
        "Relative Performance",
        "Trink",
        "Twitch",
        "Username",
        "Onscreen Prompts",
        "Flagship Property",
        "Critical Role",
        "Digital",
        "Cool Virtual Games",
        "Strong Position",
        "Cool New Username",
        "Ultimate Guide",
        "Window Of Opportunity",
        "Chief Operating Officer",
        "Unique Identity",
        "Courtesy flush",
        "Lee Trunk",
        "Artemis"
    };

    [SerializeField] private List<Sprite> iconList = new List<Sprite>();

    public string GetRandomNickname()
    {
        return nicknames[Random.Range(0, nicknames.Length)];
    }

    public Sprite GetRandomIcon()
    {
        return iconList[Random.Range(0, iconList.Count)];
    }
}
