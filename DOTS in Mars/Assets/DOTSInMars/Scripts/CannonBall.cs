using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 15.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(600.0f, 400.0f, 250.0f) * Time.deltaTime);
        transform.position += new Vector3(10.0f, 10.0f, 0.0f) * Time.deltaTime;
    }
}
