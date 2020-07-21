using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiHandler : MonoBehaviour
{
    public static GuiHandler current;

    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject healthGui;
    [SerializeField] private Animator missionTextAnimator;
    [SerializeField] private Animator zTransitionAnimator;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject finalScoreText;

    private float defaultHealthBarHeight;

    private void Awake()
    {
        current = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        defaultHealthBarHeight = healthBar.rectTransform.rect.height;
        StartCoroutine(UIStartup());
        missionTextAnimator.SetTrigger("MissionStart");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateScore();
    }
    void UpdateScore()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = $"Score:{EventHandler.current.score}";
        finalScoreText.GetComponent<TextMeshProUGUI>().text = $"Final Score:\n{EventHandler.current.score}";
    }
    void UpdateHealthBar()
    {
        float healthPercentage = Zero.current.CurrentHealth / Zero.current.MaxHealth;
        float newHeight = healthPercentage * defaultHealthBarHeight;
        healthBar.rectTransform.sizeDelta = new Vector2(healthBar.rectTransform.rect.width, newHeight);
        if(healthPercentage == 0)
        {
            healthGui.SetActive(false);
            scoreText.SetActive(false);
            StartCoroutine(EventHandler.current.Restart(missionTextAnimator,zTransitionAnimator,finalScoreText,2f));
        }
    }
    IEnumerator UIStartup()
    {
        healthGui.SetActive(false);
        scoreText.SetActive(false);
        finalScoreText.SetActive(false);
        yield return new WaitForSeconds(2f);
        healthGui.SetActive(true);
        scoreText.SetActive(true);
    }

}
