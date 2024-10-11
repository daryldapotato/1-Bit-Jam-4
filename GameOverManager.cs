using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Transform playerSpriteMask;
    public Transform player;
    public AudioSource playerHurtAudioSource;
    public ParticleSystem playerHurtParticle;
    public GameObject gameOverPanel;
    public ScoreManager scoreManager;
    public TextMeshProUGUI finalScoreText;

    private int health = 4;
    private float nextInvibilityEnd;
    private bool gameOver = false;
    private Animator playerAnim;
    public static GameOverManager instance;

    private void Awake()
    {
        Time.timeScale = 1f;

        instance = this;
        playerAnim = player.GetComponent<Animator>();

        playerSpriteMask.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (Time.time >= nextInvibilityEnd)
            playerAnim.Play("Default");

        if (Camera.main.WorldToViewportPoint(player.position).y < -0.1f && !gameOver)
        {
            playerHurtParticle.transform.position = new Vector3(player.position.x, Camera.main.ViewportToWorldPoint(Vector3.zero).y + 1f, 0);
            playerHurtParticle.Emit(10);
            playerHurtAudioSource.Play();

            StartCoroutine(GameOver());
        }
    }

    public void DamagePlayer()
    {
        if (Time.time < nextInvibilityEnd)
            return;

        health--;
        nextInvibilityEnd = Time.time + 1.5f;
        playerAnim.Play("InvincibleFlash");
        playerHurtAudioSource.Play();

        playerSpriteMask.localScale = new Vector3(1f - (health * 0.25f), 1f - (health * 0.25f), 1f);

        playerHurtParticle.Emit(8);

        if (health <= 0)
            StartCoroutine(GameOver());
    }

    public IEnumerator GameOver()
    {
        if (gameOver)
            yield break;

        gameOver = true;

        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerWeapons>().enabled = false;
        finalScoreText.text = "Score: " + Mathf.FloorToInt(scoreManager.score) + " m";

        yield return new WaitForSeconds(0.5f);
        gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
    }

    public void Replay()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, 0.34f));
    }

    public IEnumerator LoadScene(int index, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(index);
    }
}
