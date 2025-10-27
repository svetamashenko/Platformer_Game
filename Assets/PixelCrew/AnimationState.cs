using UnityEngine;

[System.Serializable]
public struct AnimationState
{
    public string Name;
    public bool Loop;
    public Sprite[] Sprites;
    public bool AllowNext;
    public string NextStateName;
}
