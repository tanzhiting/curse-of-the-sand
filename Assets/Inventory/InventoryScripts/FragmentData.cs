using UnityEngine;

[CreateAssetMenu(fileName = "New Fragment", menuName = "Inventory/Fragment")]
public class FragmentData : ScriptableObject
{
    public string fragmentID;
    public string fragmentName;
    public Sprite icon;
}