using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Ground") || objectWeHit.gameObject.CompareTag("Wall") || objectWeHit.gameObject.CompareTag("Target"))
        {
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Beer"))
        {
            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();
        }

        if (objectWeHit.gameObject.CompareTag("Enemy"))
        {
            if (objectWeHit.gameObject.GetComponent<Enemy>().isDead == false)
            {
                objectWeHit.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            }
            CreateBloodSprayEffect(objectWeHit);
            Destroy(gameObject);
        }
    }

    private void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }

    private void CreateBloodSprayEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject bloodSpray = Instantiate(
            GlobalReferences.Instance.bloodSprayEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        bloodSpray.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
