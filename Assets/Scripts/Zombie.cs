using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int zombieDamage;

    public ZombieHand zombieHand;

    private void Start()
    {
        zombieHand.damage = zombieDamage;
    }
}
