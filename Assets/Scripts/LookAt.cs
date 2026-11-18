using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public bool ignoreYAxis = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtPosition = target.position;
        if(ignoreYAxis)
            lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);
    }
}
