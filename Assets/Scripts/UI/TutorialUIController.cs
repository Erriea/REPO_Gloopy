using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIController : MonoBehaviour
{
    [System.Serializable]
    private class TutorialPage
    {
        [TextArea(2, 5)]
        public string body;
    }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI nextButtonLabel;
    [SerializeField] private bool showTutorialOnStart = true;
    [SerializeField] private List<TutorialPage> pages = new()
    {
        new TutorialPage { body = "Move controlled hosts with WASD. Your alien base form cannot move on its own." },
        new TutorialPage { body = "Click a nearby host to target it, then press Space to infect it." },
        new TutorialPage { body = "Walkers use land, swimmers use water, and flyers can cross both while ignoring obstacles." },
        new TutorialPage { body = "Reach the spaceship to win. Police can arrest you only while you are exposed in base form." },
        new TutorialPage { body = "Hosts die when their timer runs out. If a swimmer dies in water, you drown and lose." }
    };

    private int currentPageIndex;

    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceTutorial);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }

        bool shouldShowTutorial = showTutorialOnStart && GameLaunchContext.ShowTutorialOnNextGameLoad;
        GameLaunchContext.ShowTutorialOnNextGameLoad = false;

        if (shouldShowTutorial && pages.Count > 0)
        {
            OpenTutorial();
            return;
        }

        SetPanelVisible(false);
        GameManager.Instance?.EndTutorial();
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(AdvanceTutorial);
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipTutorial);
        }
    }

    public void OpenTutorial()
    {
        currentPageIndex = 0;
        GameManager.Instance?.BeginTutorial();
        SetPanelVisible(true);
        RefreshPage();
    }

    public void AdvanceTutorial()
    {
        currentPageIndex++;

        if (currentPageIndex >= pages.Count)
        {
            CloseTutorial();
            return;
        }

        RefreshPage();
    }

    public void SkipTutorial()
    {
        CloseTutorial();
    }

    private void CloseTutorial()
    {
        SetPanelVisible(false);
        GameManager.Instance?.EndTutorial();
    }

    private void RefreshPage()
    {
        if (bodyText != null && currentPageIndex >= 0 && currentPageIndex < pages.Count)
        {
            bodyText.text = pages[currentPageIndex].body;
        }

        if (nextButtonLabel != null)
        {
            bool isLastPage = currentPageIndex >= pages.Count - 1;
            nextButtonLabel.text = isLastPage ? "Start" : "Next";
        }
    }

    private void SetPanelVisible(bool visible)
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(visible);
        }
    }
}
