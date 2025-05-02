using UnityEngine;

public class ChestTouch : MonoBehaviour
{
    public Sprite[] fragmentSprites;
    public GameObject chestModel;
    public NotificationUI notificationUI; // 拖入场景的NotificationUI
    public GameObject hintUI;

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

        // ⭐️ 让提示 UI 始终水平朝向摄像机（只绕 Y 轴）
        if (hintUI.activeSelf)
        {
            Vector3 camPosition = mainCam.transform.position;
            Vector3 lookDirection = hintUI.transform.position - camPosition;

            // 只保留 XZ 平面方向
            lookDirection.y = 0f;

            // 避免零向量报错
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
        int index = Random.Range(0, fragmentSprites.Length);
        Sprite chosenSprite = fragmentSprites[index];

        chestModel.SetActive(false);
        notificationUI.ShowFragmentNotification(chosenSprite);
    }
}