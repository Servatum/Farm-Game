using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefaultSlimeAI : MonoBehaviour, IDamagable
{
    public Transform player;
    private NavMeshAgent agent;

    [Header("Stats")]
    public int health;
    public ItemData[] dropOnDeath;

    //components
    private SkinnedMeshRenderer[] meshRenderers;

    void Awake()
    {
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;
    }
    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;

        if(health <=0)
        {
            Die();
        }

        StartCoroutine(DamageFlash());
    }

    void Die()
    { 
        for(int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for(int x = 0; x < meshRenderers.Length; x++)
        {
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int x = 0; x < meshRenderers.Length; x++)
        {
            meshRenderers[x].material.color = Color.white;
        }
    }

}
