using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionManager : MonoBehaviour
{
    public List<Action<GameObject>> onParticleCollisionEvents = new List<Action<GameObject>>();

    public void TriggerParticleCollision(GameObject collider)
    { 
        foreach(Action<GameObject> action in onParticleCollisionEvents)
        {
            action(collider);
        }
    }
}
