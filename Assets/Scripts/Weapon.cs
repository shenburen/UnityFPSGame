using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    [Header("Éä»÷")]
    public bool isShooting, readyToShoot;
    public float shootingDelay = 2;
    [SerializeField]
    private bool allowRest = true;

    [Header("±¬Õ¨¿ª»ð")]
    public int bulletPerBurst = 3;
    public int burstBulletsLeft;

    [Header("×Óµ¯À©É¢")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("×Óµ¯")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3;

    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("µ¯¼Ð")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public bool isADS;

    public enum WeaponModel
    {
        Colt1911,
        M16,
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto,
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }

            GetComponent<Outline>().enabled = false;

            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0 && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletPerBurst;
                FireWeapon();
            }
        }
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("Recoil_ADS");
        }
        else
        {
            animator.SetTrigger("Recoil");
        }

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowRest)
        {
            allowRest = false;
            Invoke("ReserShot", shootingDelay);
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        animator.SetTrigger("Reload");

        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        isReloading = false;

        int bulletsNeed = magazineSize - bulletsLeft;
        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > bulletsNeed)
        {
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsNeed = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            bulletsLeft += bulletsNeed;
        }
        WeaponManager.Instance.DecreaseTotalAmmo(bulletsNeed, thisWeaponModel);
    }

    private void ReserShot()
    {
        readyToShoot = true;
        allowRest = true;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private void EnterADS()
    {
        animator.SetTrigger("EnterADS");
        isADS = true;
        HUDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("ExitADS");
        isADS = false;
        HUDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }
}
