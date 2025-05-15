using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite InactiveTabBG, ActiveTabBG;
    
    public void SwitchToTab(int TabID)
    {
        foreach(GameObject go in Tabs)
        {
            go.SetActive(false); //to disable all the tabs
        }
        Tabs[TabID].SetActive(true); //to enable the required tab

        foreach (Image im in TabButtons)
        {
            im.sprite=InactiveTabBG; // change buttonbg 
        }
        TabButtons[TabID].sprite = ActiveTabBG;
    }

}
