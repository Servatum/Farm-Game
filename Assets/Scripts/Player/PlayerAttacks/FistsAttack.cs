using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FistsAttack : MonoBehaviour
{
    public Animator FistAnim;

    void Update()
    {
        GetMouseButtonDown();
    }
    void GetMouseButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("LeftClicked");
            FistAnim.SetTrigger("LAttack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("RightClicked");
            FistAnim.SetTrigger("RAttack");
        }
    }
}
