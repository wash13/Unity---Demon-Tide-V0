﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonStagger : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void stagger()
    {
        anim.Play("Weakness");
    }
}
