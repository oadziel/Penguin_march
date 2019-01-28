using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class AnimControlScript : MonoBehaviour {


    public enum AnimState { waddle, slide }
    public AnimState animState = AnimState.waddle;

    StudioEventEmitter EE_foot;
    StudioEventEmitter EE_slide;

    bool lookingToChangeState;

    public int maxAnimChangeTime;
    float timer;

    Animator anim;

    NavMeshAgent NMA;

    public void ChangeState(int id)
    {
        lookingToChangeState = true;
        System.Random rand = new System.Random(id);
        timer = rand.Next(0, maxAnimChangeTime);
        timer *= 0.867f;
        Debug.Log(timer);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        NMA = GetComponent<NavMeshAgent>();
        EE_foot = transform.Find("FootstepNoise").GetComponent<StudioEventEmitter>();
        EE_slide = transform.Find("SlideNoise").GetComponent<StudioEventEmitter>();
        EE_foot.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lookingToChangeState)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeAnim();
                lookingToChangeState = false;
            }
        }
    }

    public void ChangeToFishCarry()
    {
        anim.SetTrigger("fishCarry");
    }

    void ChangeAnim()
    {
        switch (animState)
        {
            case AnimState.waddle:
                animState = AnimState.slide;
                anim.SetTrigger("slide");
                NMA.speed *= 2;
                EE_slide.enabled = false;
                EE_foot.enabled = true;
                break;
            case AnimState.slide:
                animState = AnimState.waddle;
                anim.SetTrigger("waddle");
                NMA.speed *= 0.5f;
                EE_slide.enabled = true;
                EE_foot.enabled = false;
                break;

        }

    }
}
