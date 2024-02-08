using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    //components
    private Animator anim;
    private Camera cam;

    [SerializeField] HarvestType[] harvestTypes;

    void Awake()
    {
        //get our components
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if(!attacking)
        {
            attacking = true;
            anim.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }   
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, attackDistance))
        {
            //did we hit a resource?
            if(doesGatherResources && hit.collider.GetComponent<Resource>())
            {
                CollectResource(hit.collider, hit.point, hit.normal);
                //hit.collider.GetComponent<Resource>().Gather(hit.point, hit.normal);
            }

            //did we hit a damagable
            if(doesDealDamage && hit.collider.GetComponent<IDamagable>() != null)
            {
                hit.collider.GetComponent<IDamagable>().TakePhysicalDamage(damage);
            }
        }
    }

    void CollectResource(Collider resource, Vector3 hitPoint, Vector3 hitNormal)
    {
        for(int i = 0; i < harvestTypes.Length; i++)
        {
            if (resource.GetComponent<Resource>().itemToGive.harvestType == harvestTypes[i])
            {
                resource.GetComponent<Resource>().Gather(hitPoint, hitNormal);
            }
        }
    }
}
