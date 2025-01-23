using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class GemDropper : MonoBehaviour
{
    [SerializeField]
    GameObject tinyPrefab;

    [SerializeField]
    GameObject midTinyPrefab;

    [SerializeField]
    GameObject midPrefab;

    [SerializeField]
    GameObject midLargePrefab;

    [SerializeField]
    GameObject largePrefab;

    [SerializeField]
    GameObject yugePrefab;

    [SerializeField]
    float scrollSpeed;

    [SerializeField]
    GameObject lightEffect;

    [SerializeField]
    LineRenderer targetLine;

    [SerializeField]
    MovingGround ground;

    [SerializeField]
    GameObject comboPrefab;

    [SerializeField]
    DataManager dataManager;

    GameControls controls;

    float combo_counter = 1.0f;
    bool countingCombo = false;
    int combo_level = 1;

    List<AudioSource> pops = new();

    [SerializeField]
    List<AudioClip> pop_clips;

    Queue<TorqueCreator> gemQueue;

    TorqueCreator current_gem;

    bool isDragging = false;
    Vector3 target_position = Vector3.zero;

    void Awake()
    {
        controls = new();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Default.Touch.performed += OnTouchPerformed;
        controls.Default.Drag.started += OnDragStarted;
        controls.Default.Drag.performed += OnDragPerformed;
        controls.Default.Drag.canceled += OnDragCancelled;
    }

    void OnTouchPerformed(InputAction.CallbackContext context)
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        if (current_gem != null)
        {
            current_gem.GetComponent<Rigidbody>().isKinematic = false;
            current_gem.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, -4.0f, 0.0f), ForceMode.Impulse);
            current_gem.GetComponent<SphereCollider>().enabled = true;
            current_gem.Deployed = true;
            current_gem = null;
            targetLine.gameObject.SetActive(false);
            StartCoroutine(CallNextGem());
        }
    }

    void OnDragStarted(InputAction.CallbackContext context)
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        isDragging = true;
        Vector3 dragStarted = context.ReadValue<Vector2>();
        dragStarted.z = Camera.main.nearClipPlane;
        target_position = Camera.main.ScreenToWorldPoint(dragStarted);
        target_position.x = Mathf.Clamp(target_position.x, -4.5f + current_gem.Radius, 4.5f - current_gem.Radius);
        target_position.y = current_gem.transform.position.y;
        target_position.z = 0.0f;
    }

    void OnDragPerformed(InputAction.CallbackContext context)
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        isDragging = true;
        if (current_gem != null && isDragging)
        {
            Vector2 dragStarted = context.ReadValue<Vector2>();
            target_position = Camera.main.ScreenToWorldPoint(dragStarted);
            target_position.x = Mathf.Clamp(target_position.x, -4.5f + current_gem.Radius, 4.5f - current_gem.Radius);
            target_position.y = current_gem.transform.position.y;
            target_position.z = 0.0f;
        }
        
    }

    void OnDragCancelled(InputAction.CallbackContext context)
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        isDragging = false;

    }

    void Start()
    {
        GetComponentsInChildren<AudioSource>(pops);
        gemQueue = new();
        targetLine.positionCount = 2;
        targetLine.useWorldSpace = true;

        for (int i = 0; i < 3; ++i)
        {
            GenerateNextGem();
        }

        PlaceGem();
    }

    

    void PlayPop()
    {
        foreach(var audio_player in pops)
        {
            if (audio_player.isPlaying)
            {
                break;
            }
            else
            {
                audio_player.PlayOneShot(pop_clips[Random.Range(0, pop_clips.Count)]);
                break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (dataManager.IsGameOver)
        {
            return;
        }

        if (countingCombo)
        {
            combo_counter -= Time.deltaTime;

            if(combo_counter <= 0.0f)
            {
                countingCombo = false;
                combo_counter = 1.0f;
                combo_level = 1;
            }
        }

        if (current_gem != null)
        {
            if (isDragging)
            {
                current_gem.transform.position = target_position;
            }

            targetLine.SetPosition(0, current_gem.transform.position);

            RaycastHit hit;
            if (Physics.Raycast(current_gem.transform.position, Vector3.down, out hit, 100, LayerMask.GetMask("Ground", "Gem")))
            {
                Vector3 opos = hit.point;
                opos.x = target_position.x;

                targetLine.SetPosition(1, hit.point);
            }

        }

        /*if(Touchscreen.current.primaryTouch.press.isPressed == false & isDragging)
        {
            OnDragCancelled(default);
        }*/

    }

    void PlaceGem()
    {
        TorqueCreator upGem = gemQueue.Dequeue();
        Vector3 pos = upGem.transform.position;
        pos.x = target_position.x;
        upGem.transform.position = pos;

        current_gem = upGem;


        GenerateNextGem();
    }

    void GenerateNextGem()
    {
        int balltype_rand = Random.Range(0, 1000);

        TorqueCreator newBall = null;

        Vector3 pos = new Vector3(500, 12.8f, 0.0f);

        if (balltype_rand <= 550)
        {
            newBall = GameObject.Instantiate(tinyPrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else if (balltype_rand <= 800)
        {
            newBall = GameObject.Instantiate(midTinyPrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else if (balltype_rand <= 900)
        {
            newBall = GameObject.Instantiate(midPrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else
        {
            newBall = GameObject.Instantiate(midLargePrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }

        newBall.GetComponent<Rigidbody>().isKinematic = true;

        pos.y -= newBall.Radius;
        newBall.transform.position = pos;

        newBall.GetComponent<SphereCollider>().enabled = false;
        newBall.ballCollision.AddListener(CreateNewBallAtPosition);
        gemQueue.Enqueue(newBall);
    }

    public void CreateNewBallAtPosition(Vector3 pos, int type)
    {

        PlayPop();

        if (!countingCombo)
        {
            countingCombo = true;
        }
        else
        {
            combo_level += 1;
            combo_counter += 0.5f;
            Debug.Log("COMBO TIME");
        }

        ground.PushBack((float)type);

        GameObject ps = GameObject.Instantiate(lightEffect, pos, Quaternion.identity);
        GameObject.Destroy(ps, 3.0f);

        if(combo_level > 1)
        {
            var go = GameObject.Instantiate(comboPrefab, pos, Quaternion.identity);
            go.GetComponentInChildren<TextMeshProUGUI>().text = "COMBO X" + combo_level;
            GameObject.Destroy(go, 2.0f);
        }

        dataManager.AddScore(type, combo_level);

        if (type == 6)
        {
            return;
        }

        TorqueCreator newBall = null;
       
        if (type == 1)
        {
            newBall = GameObject.Instantiate(midTinyPrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else if (type == 2)
        {
            newBall = GameObject.Instantiate(midPrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else if (type == 3)
        {
            newBall = GameObject.Instantiate(midLargePrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else if (type == 4)
        {
            newBall = GameObject.Instantiate(largePrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }
        else
        {
            newBall = GameObject.Instantiate(yugePrefab, pos, Quaternion.identity).GetComponent<TorqueCreator>();
        }


        Debug.Log(newBall.gameObject.name + "New ball deployed");
        newBall.Deployed = true;
        newBall.ballCollision.AddListener(CreateNewBallAtPosition);

    }

    IEnumerator CallNextGem()
    {
        yield return new WaitForSeconds(1.0f);
        targetLine.gameObject.SetActive(true);
        PlaceGem();
    }
}
