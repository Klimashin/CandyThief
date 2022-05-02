using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogData")]
public class DialogData : ScriptableObject
{
    public List<Replica> Texts;
}

public enum Character
{
    Main,
    CandyKing
}

[Serializable]
public class Replica
{
    public Character Character;
    [TextArea] public string Text;
}
