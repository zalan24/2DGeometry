using UnityEngine;
using System.Collections;

public class Point : MonoBehaviour {

    public static int numPoints=0;
    public static Point[] points = new Point[10000];

	void Awake () {
        if (numPoints >= points.Length)
        {
            Destroy(gameObject);
            Debug.LogWarning("Too many points");
        }
        else
        {
            points[numPoints++] = this;
        }
	}

    void Update()
    {
        if (Mgr.enablerandomove)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(Random.Range(0.0f, 2.0f) - 1, Random.Range(0.0f, 2.0f) - 1, Random.Range(0.0f, 2.0f) - 1) * 5);
        }
    }

    void OnDestroy()
    {
        int n = 0;
        while (n < numPoints && points[n] != this) n++;
        if (n < numPoints)
        {
            numPoints--;
            Point sw = points[n];
            points[n] = points[numPoints];
            points[numPoints] = sw;
        }
        else
        {
            Debug.LogError("Destroying points: point not found");
        }
    }
}
