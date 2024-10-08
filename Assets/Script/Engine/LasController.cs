using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LasController : MonoBehaviour
{
    internal  WedingController weding;

    void Start()
    {
        // Menggunakan FindObjectOfType untuk inisialisasi weding
        weding = FindObjectOfType<WedingController>();
        if (weding == null)
        {
            Debug.LogError("WedingController not found in the scene!");
        }
    }

    void Update()
    {
        weding.GetWorldPoint();
        weding.StartLash();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButton(0))
            {
                weding.GetWorldPoint();
                weding.StartLash();
                
            }
        }
        else
        {
            
          
        }

       
    }


}
