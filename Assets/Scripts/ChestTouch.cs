using UnityEngine;

public class ChestTouch : MonoBehaviour
{
    [Header("Fragment Settings")]
    public FragmentData[] fragmentDataList;

    [Header("Chest Objects")]
    public GameObject chestModel;
    public GameObject hintUI;

    [Header("UI")]
    public NotificationUI notificationUI;

    [Header("Data")]
    public PlayerInventorySO playerInventory;
    public PlayerStatsSO playerStats; // ✅ 添加本局统计数据

    [Header("Backpack Reference")]
    public BackpackManager backpackManager;

    private bool opened = false;
    private bool isPlayerNearby = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (opened) return;

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) ||
            Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform && isPlayerNearby)
                {
                    opened = true;
                    hintUI.SetActive(false);
                    OnChestOpened();
                }
            }
        }

        if (hintUI.activeSelf)
        {
            Vector3 camPosition = mainCam.transform.position;
            Vector3 lookDirection = hintUI.transform.position - camPosition;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                hintUI.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !opened)
        {
            hintUI.SetActive(true);
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hintUI.SetActive(false);
            isPlayerNearby = false;
        }
    }

    void OnChestOpened()
    {
        int index = Random.Range(0, fragmentDataList.Length);
        FragmentData chosenData = fragmentDataList[index];

        chestModel.SetActive(false); // 隐藏宝箱模型

        // 1. 添加碎片数据
        playerInventory.AddFragment(chosenData, 1);

        // ✅ 2. 添加到本局记录
        if (playerStats != null)
        {
            playerStats.AddFragment(chosenData, 1);
        }

        // 3. 弹出碎片获得的通知
        notificationUI.ShowFragmentNotification(chosenData.icon);

        // 4. 通知背包系统：碎片更新 + 刷新 UI
        if (backpackManager != null)
        {
            backpackManager.NotifyFragmentGained();
            backpackManager.RefreshGrid();
            StartCoroutine(CheckUnlockAfterDelay(notificationUI.duration));
        }
    }

    private System.Collections.IEnumerator CheckUnlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        TreasureData craftableTreasure = backpackManager.CheckForCraftableTreasureAfterCollect();
        if (craftableTreasure != null)
        {
            notificationUI.ShowUnlockNotification(craftableTreasure.image);
        }
    }
}