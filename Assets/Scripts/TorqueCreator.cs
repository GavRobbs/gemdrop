using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TorqueCreator : MonoBehaviour
{
    public int type = 0;

    bool initialTorque = false;
    bool useTimer = false;
    float maxDragTime = 0.4f;

    [SerializeField]
    GameObject mediumPrefab;

    [SerializeField]
    GameObject largePrefab;

    [SerializeField]
    SphereCollider sphereCollider;

    [SerializeField]
    Rigidbody mRigidbody;

    [SerializeField]
    bool _deployed = false;
    public bool Deployed { get { return _deployed; } set { _deployed = value; } }

    public bool IsMoving
    {
        get
        {
            return GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.01f;
        }
    }

    public float Radius { get => sphereCollider.radius * transform.lossyScale.x; }

    public UnityEvent<Vector3, int> ballCollision = new();

    public bool changeTriggered = false;
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(useTimer == true)
        {
            maxDragTime -= Time.deltaTime;

            if(maxDragTime <= 0.0f)
            {
                useTimer = false;
                var rb = GetComponent<Rigidbody>();
                rb.drag = 0.9f;
            }

        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gem"))
        {
            var otq = collision.gameObject.GetComponent<TorqueCreator>();
            if(!otq.Deployed || !Deployed)
            {
                return;
            }

            GetComponent<Rigidbody>().drag = 0.55f;
            otq.GetComponent<Rigidbody>().drag = 0.55f;
            if (otq.type == this.type)
            {

                if(otq.changeTriggered == false)
                {

                    this.changeTriggered = true;
                    otq.changeTriggered = true;

                    Vector3 spawnPos = collision.GetContact(0).point;
                    ballCollision.Invoke(spawnPos, type + 1);

                    GameObject.Destroy(collision.gameObject);
                    GameObject.Destroy(this.gameObject);
                }
                
               
            }

            if (!initialTorque)
            {
                initialTorque = true;
                var rb = GetComponent<Rigidbody>();
                rb.AddTorque(new Vector3(0.0f, 0.0f, Random.Range(2.0f, 6.0f)));
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!useTimer)
            {
                useTimer = true;
            }
            
        }
    }

    public void ActivateDanger()
    {
        GetComponentInChildren<ParticleSystem>().Play();
    }

    public void DeactivateDanger()
    {
        GetComponentInChildren<ParticleSystem>().Stop();

    }
}
