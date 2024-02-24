using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLine : MonoBehaviour
{
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = transform.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lr.material.color.a > 0)
        {
            Color c = lr.material.color;
            c.a -= Time.deltaTime;
            lr.material.color = c;
        }
        else 
        {
            Destroy(lr);
            Destroy(this);
        }
    }
}
