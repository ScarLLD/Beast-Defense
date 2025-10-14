using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dragonFiraParticle;

    public ParticleSystem DragonFire => _dragonFiraParticle;
}
