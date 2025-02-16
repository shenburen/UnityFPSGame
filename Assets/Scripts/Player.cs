using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;
    public GameObject gameOverUI;
    public TextMeshProUGUI playerHealthUI;
    public bool isDead;

    private void Start()
    {
        playerHealthUI.text = $"Health:{HP}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand") && isDead == false)
        {
            TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            PlayerDead();
            isDead = true;
        }
        else
        {
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health:{HP}";
        }
    }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDie);

        SoundManager.Instance.playerChannel.clip = SoundManager.Instance.gameOverMusic;
        SoundManager.Instance.playerChannel.PlayDelayed(2);

        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator BloodyScreenEffect()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);

        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        Image image = bloodyScreen.GetComponentInChildren<Image>();

        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (bloodyScreen.activeInHierarchy == true)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1);
        gameOverUI.SetActive(true);

        int waveSurvived = GlobalReferences.Instance.waveNumber;
        if (waveSurvived - 1 > SaveLoadManager.Instance.LoadHightScore())
        {
            SaveLoadManager.Instance.SaveHighScore(waveSurvived - 1);
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MainMenu");
    }
}
