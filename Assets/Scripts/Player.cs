using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;
    public TextMeshProUGUI playerHealthUI;

    private void Start()
    {
        playerHealthUI.text = $"Health:{HP}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand"))
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
        }
        else
        {
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health:{HP}";
        }
    }

    private void PlayerDead()
    {
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);
    }

    private IEnumerator BloodyScreenEffect()
    {
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
}
