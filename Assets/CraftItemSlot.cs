using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemSlot : MonoBehaviour
{
    public Image mainImage;
    public Transform fragmentGroup; // 父物体（用 HorizontalLayoutGroup 排列）
    public GameObject fragmentSlotPrefab; // 小图标+数量的 prefab

    private TreasureData treasureData;
    private BackpackManager manager;

    public void Setup(TreasureData data, BackpackManager manager, int[] ownedCounts)
    {
        this.treasureData = data;
        this.manager = manager;

        mainImage.sprite = data.image;

        // 清空旧的碎片UI
        foreach (Transform child in fragmentGroup)
            Destroy(child.gameObject);

        for (int i = 0; i < data.requiredFragments.Length; i++)
        {
            var frag = data.requiredFragments[i];

            GameObject fragSlot = Instantiate(fragmentSlotPrefab, fragmentGroup);
            Image icon = fragSlot.transform.Find("FragmentIcon").GetComponent<Image>();
            TMP_Text count = fragSlot.transform.Find("FragmentCount").GetComponent<TMP_Text>();

            icon.sprite = frag.fragment.icon;
            count.text = $"{ownedCounts[i]}/{frag.requiredAmount}";
        }
    }

    // 点击时被 Unity Button 事件调用
    public void OnClick()
    {
        manager.OnItemClicked(treasureData);
    }
}