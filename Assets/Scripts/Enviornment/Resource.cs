using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit;
    public int capacity;
    public GameObject hitParticle;

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        quantityPerHit = Random.Range(1, 4);
        //give the player "quantityPerHit" of the resource
        for(int i = 0; i < quantityPerHit; i++)
        {
            if(capacity <= 0)
            {
                break;
            }
            

            Inventory.instance.AddItem(itemToGive);
        }
        capacity -= 1;

        //create hit particle
        Destroy(Instantiate(hitParticle, hitPoint, Quaternion.LookRotation(hitNormal, Vector3.up)), 1.0f);

        //if we're empty, destroy the resource
        if(capacity <= 0)
        {
            Destroy(gameObject);
        }
    }
}
