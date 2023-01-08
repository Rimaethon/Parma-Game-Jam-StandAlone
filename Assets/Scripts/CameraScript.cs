using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject helmet;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = helmet.transform.localPosition + new Vector3(1.724f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = helmet.transform.localPosition + new Vector3(0, 1.724f, 0);
    }
}
