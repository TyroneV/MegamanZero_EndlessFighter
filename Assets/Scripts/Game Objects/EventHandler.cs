using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventHandler : MonoBehaviour
{
    public static EventHandler current;

    private bool failed = false;

    public int score = 0;
    public AudioSource music;

    private void Awake()
    {
        current = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 10);
        Physics2D.IgnoreLayerCollision(10, 10);
        music = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public IEnumerator Restart(Animator missionTextAnimator,Animator zTransitionAnimator,GameObject finalScoreText,float time)
    {
        if (!failed)
        {
            failed = true;
            yield return new WaitForSeconds(time * 2f);
            music.Stop();
            missionTextAnimator.SetTrigger("MissionFailed");
            yield return new WaitForSeconds(time * 1.5f);
            finalScoreText.SetActive(true);
            yield return new WaitForSeconds(time * 2f);
            zTransitionAnimator.SetTrigger("StartTransition");
            yield return new WaitForSeconds(time);
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }
}
