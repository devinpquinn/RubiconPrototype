﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript2 : MonoBehaviour
{
    public float fireRate = 0.25f;                                        // Number in seconds which controls how often the player can fire
    public float weaponRange = 100f;                                        // Distance in Unity units over which the player can fire
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun

    private Camera fpsCam;                                                // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;                                        // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                                // Float to store the time the player will be allowed to fire again, after firing

    public Animator anim;

    public GameObject flashLight;

    public ParticleSystem muzzle;

    public ParticleSystem burst;

    public int mag;
    private int magSize = 24;

    public GameObject bar;
    public Animator ammoAnim;

    void Start()
    {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        // Get and store a reference to our Camera by searching this GameObject and its parents
        fpsCam = GetComponentInParent<Camera>();

        mag = magSize;

        bar.transform.localScale = new Vector3(1, 1, 1);

        flashLight.SetActive(false);
    }


    void Update()
    {
        // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        if (Input.GetButtonDown("Fire2") && Time.time > nextFire)
        {
            if(mag > 0)
            {
                mag--;
                bar.transform.localScale = new Vector3((float) mag / magSize, 1, 1);

                // Update the time when our player can fire next
                nextFire = Time.time + fireRate;

                anim.Play("fireR");
                muzzle.Play();

                // Start our ShotEffect coroutine to turn our laser line on and off
                StartCoroutine(ShotEffect());

                // Create a vector at the center of our camera's viewport
                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

                // Declare a raycast hit to store information about what our raycast has hit
                RaycastHit hit;

                // Set the start position for our visual effect for our laser to the position of gunEnd
                laserLine.SetPosition(0, gunEnd.position);

                // Check if our raycast has hit anything
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
                {
                    // Set the end position for our laser line 
                    laserLine.SetPosition(1, hit.point);
                    if (hit.collider.gameObject.CompareTag("Arena"))
                    {
                        Instantiate(burst, hit.point, Quaternion.identity);
                    }
                    if(hit.collider.gameObject.CompareTag("Player"))
                    {
                        hit.collider.gameObject.GetComponent<HealthTracker>().TakeDamage(1);
                    }
                }
                else
                {
                    // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                    laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
                }
            }
            else
            {
                StartCoroutine("Reload");
            }
        }

        /*if(Input.GetKeyDown(KeyCode.R))
        {
            mag = 0;
            StartCoroutine("Reload");
        }*/
        if (Input.GetKeyDown(KeyCode.T))
        {
            mag = 0;
            StartCoroutine("Reload");
        }

    }


    private IEnumerator ShotEffect()
    {

        // Turn on our line renderer
        laserLine.enabled = true;
        flashLight.SetActive(true);

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
        flashLight.SetActive(false);
    }

    private IEnumerator Reload()
    {
        ammoAnim.Play("ammoBump");
        anim.Play("loadR");
        yield return new WaitForSeconds(1.01f);
        mag = magSize;
        bar.transform.localScale = new Vector3(1, 1, 1);
    }
}
