using TMPro;
using UnityEngine;

public class RoundTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (timerText == null || GameManager.Instance == null)
        {
            return;
        }

        bool showTimer = GameManager.Instance.CurrentState == GameManager.GameState.Playing;
        timerText.gameObject.SetActive(showTimer);

        if (showTimer)
        {
            timerText.text = GameManager.Instance.GetFormattedElapsedTime();
        }
    }
}
