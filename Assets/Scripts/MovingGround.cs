using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingGround : MonoBehaviour
{
    [SerializeField]
    DataManager dataManager;

    Rigidbody mRigidbody;
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        Vector3 pos = transform.position;
        //0.0035f
        pos.y += 0.065f * Time.deltaTime;
        transform.position = pos;

        if (pos.y > 11.5)
        {
            dataManager.GameOver();
        }
    }

    public void PushBack(float fac)
    {
        Vector3 pos = transform.position;
        pos.y -= 0.012f * fac;
        pos.y = Mathf.Max(pos.y, -8.55f);
        transform.position = pos;

        var anims = GetComponentsInChildren<Animator>();
        foreach(var anim in anims)
        {
            anim.SetTrigger("gear_reverse");
        }
    }

}
