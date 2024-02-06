using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : Building, IInteractable
{
    public GameObject particle;
    public GameObject light;
    private bool isOn;
    private Vector3 lightStartPos;

    [Header("Damage")]
    public int damage;
    public float damageRate;

    private List<IDamagable> thingsToDamage = new List<IDamagable>();

    void Start()
    {
        lightStartPos = light.transform.localPosition;
        StartCoroutine(DealDamage());
    }

    IEnumerator DealDamage()
    {
        while(true)
        {
            if (isOn)
            {
                for (int x = 0; x < thingsToDamage.Count; x++)
                {
                    thingsToDamage[x].TakePhysicalDamage(damage);
                }
            }

            yield return new WaitForSeconds(damageRate);
        }
    }

    public string GetInteractPrompt()
    {
        return isOn ? "Turn Off" : "Turn On";
    }

    public void OnInteract()
    {
        isOn = !isOn;

        particle.SetActive(isOn);
        light.SetActive(isOn);
    }

    void Update()
    {
        if(isOn)
        {
            float x = Mathf.PerlinNoise(Time.time * 3.0f, 0.0f) / 5.0f;
            float z = Mathf.PerlinNoise(0.0f, Time.time * 3.0f) / 5.0f;

            light.transform.localPosition = lightStartPos + new Vector3(x, 0.0f, z);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<IDamagable>() != null) 
        {
            thingsToDamage.Add(other.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent <IDamagable>() != null)
        {
            thingsToDamage.Remove(other.gameObject.GetComponent<IDamagable>());
        }
    }
}
