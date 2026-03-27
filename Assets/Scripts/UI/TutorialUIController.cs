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
    [SerializeField] private bool useDefaultTutorialPages = true;
    [SerializeField] private List<TutorialPage> pages = new()
    {
        new TutorialPage { body = "You are Gloopy, a little alien stuck on a strange planet. You should not be here and need to find your spaceship in order to go home." },
        new TutorialPage { body = "Move controlled hosts with WASD. In your base form you cannot move at all, so click a nearby host to target it and press Space to infect it." },
        new TutorialPage { body = "Walkers move on land, swimmers move through water, and flyers can cross both while ignoring obstacles." },
        new TutorialPage { body = "Watch out for the police. They can arrest you if they catch you while you are exposed in base form." },
        new TutorialPage { body = "Keep an eye on your host timer. If a swimmer dies while you are over water, you will drown and lose." }
    };

    private int currentPageIndex;

    private void Start()
    {
        if (useDefaultTutorialPages)
        {
            BuildDefaultPages();
        }

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
        AudioManager.Instance?.PlayButtonClick();
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
        AudioManager.Instance?.PlayButtonClick();
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

    private void BuildDefaultPages()
    {
        pages = new List<TutorialPage>
        {
            new TutorialPage
            {
                body = "You are Gloopy, a little alien stuck on a strange planet. You should not be here and need to find your spaceship in order to go home."
            },
            new TutorialPage
            {
                body = "Move controlled hosts with WASD. In your base form you cannot move at all, so click a nearby host to target it and press Space to infect it."
            },
            new TutorialPage
            {
                body = "Walkers move on land, swimmers move through water, and flyers can cross both while ignoring obstacles."
            },
            new TutorialPage
            {
                body = "Watch out for the police. They can arrest you if they catch you while you are exposed in base form."
            },
            new TutorialPage
            {
                body = "Keep an eye on your host timer. If a swimmer dies while you are over water, you will drown and lose."
            }
        };
    }
}
