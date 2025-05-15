using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwiperController : MonoBehaviour, IEndDragHandler
{
    [Header("Pages")]
    [SerializeField] private int        maxPage = 5;
    [SerializeField] private Vector3    pageStep = new Vector3(-800, 0, 0);
    [SerializeField] private RectTransform pagesRect;

    [Header("Tween")]
    [SerializeField] private float      tweenTime = 0.5f;
    [SerializeField] private LeanTweenType tweenType = LeanTweenType.easeOutExpo;

    [Header("Pagination Dots")]
    [SerializeField] private Button[]   dotButtons;
    [SerializeField] private Sprite     barClose, barOpen;

    [Header("Arrows")]
    [SerializeField] private Button     previousBtn, nextBtn;

    private int     currentPage;
    private Vector3 basePos, targetPos;
    private float   dragThreshold;

    private void Awake()
    {
        // 1) Cache initial position and threshold
        basePos       = pagesRect.localPosition;
        dragThreshold = Screen.width / 15f;

        // 2) REMOVE any persistent listeners in the Inspector!
        previousBtn.onClick.RemoveAllListeners();
        nextBtn    .onClick.RemoveAllListeners();

        // 3) WIRE Next/Prev exactly once in code
        previousBtn.onClick.AddListener(OnPreviousClicked);
        nextBtn    .onClick.AddListener(OnNextClicked);

        // 4) CLEAR and WIRE dotButtons exactly once
        for (int i = 0; i < dotButtons.Length; i++)
        {
            dotButtons[i].onClick.RemoveAllListeners();
            int pageIndex = i + 1;
            dotButtons[i].onClick.AddListener(() => GoToPage(pageIndex));
            dotButtons[i].transition = Selectable.Transition.None;
        }

        // 5) Start on page 1
        GoToPage(1);
    }

    private void OnNextClicked()
    {
        // Guarantee: only moves +1
        GoToPage(currentPage + 1);
    }

    private void OnPreviousClicked()
    {
        // Guarantee: only moves -1
        GoToPage(currentPage - 1);
    }

    public void GoToPage(int pageIndex)
    {
        if (pageIndex < 1 || pageIndex > maxPage)
            return; // out of range => ignore

        currentPage = pageIndex;
        targetPos   = basePos + pageStep * (pageIndex - 1);

        // 1) Animate pages container
        pagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);

        // 2) Update UI
        UpdateDots();
        UpdateArrows();

        // 3) Clear any UI selection (no flashes)
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - eventData.pressPosition.x;
        if (Mathf.Abs(delta) > dragThreshold)
        {
            if (delta > 0) OnPreviousClicked();
            else           OnNextClicked();
        }
        else
        {
            // snap back
            GoToPage(currentPage);
        }
    }

    private void UpdateDots()
    {
        for (int i = 0; i < dotButtons.Length; i++)
        {
            bool isCurrent = (i == currentPage - 1);
            dotButtons[i].GetComponent<Image>().sprite = isCurrent ? barOpen : barClose;
        }
    }

    private void UpdateArrows()
    {
        previousBtn.gameObject.SetActive(currentPage > 1);
        nextBtn    .gameObject.SetActive(currentPage < maxPage);
    }
}
