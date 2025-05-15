using UnityEngine;
using System;

public class ChestTouch : MonoBehaviour
{
    [Header("Fragment Settings")]
    public FragmentData[] fragmentDataList;

    [Header("Chest Objects")]
    public GameObject chestModel;
    public GameObject hintUI;

    [Header("Data")]
    public PlayerInventorySO playerInventory;
    public PlayerStatsSO playerStats;

    private bool opened = false;
    private bool isPlayerNearby = false;
    private Camera mainCam;

    public event Action OnChestDestroyed;

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
                // ✅ 改成只要点击的是这个宝箱或它的子物体，就触发
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform) && isPlayerNearby)
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
        int index = UnityEngine.Random.Range(0, fragmentDataList.Length);
        FragmentData chosenData = fragmentDataList[index];

        chestModel.SetActive(false);

        playerInventory.AddFragment(chosenData, 1);
        playerStats?.AddFragment(chosenData, 1);

        NotificationUI.Instance?.ShowFragmentNotification(chosenData.icon);
        BackpackUIController.Instance?.RefreshGrid();

        StartCoroutine(CheckUnlockAfterDelay(NotificationUI.Instance != null ? NotificationUI.Instance.duration : 2f));

        OnChestDestroyed?.Invoke();
    }

    private System.Collections.IEnumerator CheckUnlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (BackpackManager.Instance != null)
        {
            TreasureData craftable = BackpackManager.Instance.CheckForCraftableTreasureAfterCollect();
            if (craftable != null)
            {
                NotificationUI.Instance?.ShowUnlockNotification(craftable.image);
            }
        }
    }
}