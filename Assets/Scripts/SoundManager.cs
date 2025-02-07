using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource shootingChannel;

    public AudioClip shot1911;
    public AudioClip shotM16;

    public AudioSource reloadingSound1911;
    public AudioSource reloadingSoundM16;

    public AudioSource emptyMagazineSound1911;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Colt1911:
                shootingChannel.PlayOneShot(shot1911);
                break;
            case WeaponModel.M16:
                shootingChannel.PlayOneShot(shotM16);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Colt1911:
                reloadingSound1911.Play();
                break;
            case WeaponModel.M16:
                reloadingSoundM16.Play();
                break;
        }
    }
}
