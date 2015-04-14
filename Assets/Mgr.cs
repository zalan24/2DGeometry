using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mgr : MonoBehaviour {

    public static bool enablerandomove = false;
    public static Point[] copyPoints;

    public string[] options;
    public float refreshtime = 0.5f;
    public GameObject linegm;

    int selectedoption = -1;
    int lastselectedoption = -1;
    float refresh = 0;
    List<GameObject> linelist = new List<GameObject>();

    void Start()
    {
        copyPoints = new Point[Point.points.Length];
        //Debug.Log(Dir(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1)));
    }

    public static float Dir(Vector3 p0, Vector3 p1, Vector3 p2)                     //Does not use the axis Y
    {
        p1 -= p0; p2 -= p0;
        return p1.x * p2.z - p2.x * p1.z;
    }

    public static int getCorner(int j = 0, int a = 0, int b = -1)
    {
        if (b == -1) b = Point.numPoints;
        int n = a;
        for (int i = a+1; i < b; ++i)
        {
            if (j == 0)
            {
                if (Point.points[i].transform.position.x < Point.points[n].transform.position.x ||
                Point.points[i].transform.position.x == Point.points[n].transform.position.x && Point.points[i].transform.position.z < Point.points[n].transform.position.z) n = i;
            }
            else if (j == 1)
            {
                if (Point.points[i].transform.position.x < Point.points[n].transform.position.x ||
                Point.points[i].transform.position.x == Point.points[n].transform.position.x && Point.points[i].transform.position.z > Point.points[n].transform.position.z) n = i;
            }
            else if (j == 2)
            {
                if (Point.points[i].transform.position.x > Point.points[n].transform.position.x ||
                Point.points[i].transform.position.x == Point.points[n].transform.position.x && Point.points[i].transform.position.z < Point.points[n].transform.position.z) n = i;
            }
            else if (j == 3)
            {
                if (Point.points[i].transform.position.x > Point.points[n].transform.position.x ||
                Point.points[i].transform.position.x == Point.points[n].transform.position.x && Point.points[i].transform.position.z > Point.points[n].transform.position.z) n = i;
            }
            else if (j == 4)
            {
                if (Point.points[i].transform.position.z < Point.points[n].transform.position.z ||
                Point.points[i].transform.position.z == Point.points[n].transform.position.z && Point.points[i].transform.position.x < Point.points[n].transform.position.x) n = i;
            }
            else if (j == 5)
            {
                if (Point.points[i].transform.position.z < Point.points[n].transform.position.z ||
                Point.points[i].transform.position.z == Point.points[n].transform.position.z && Point.points[i].transform.position.x > Point.points[n].transform.position.x) n = i;
            }
            else if (j == 6)
            {
                if (Point.points[i].transform.position.z > Point.points[n].transform.position.z ||
                Point.points[i].transform.position.z == Point.points[n].transform.position.z && Point.points[i].transform.position.x < Point.points[n].transform.position.x) n = i;
            }
            else if (j == 7)
            {
                if (Point.points[i].transform.position.z > Point.points[n].transform.position.z ||
                Point.points[i].transform.position.z == Point.points[n].transform.position.z && Point.points[i].transform.position.x > Point.points[n].transform.position.x) n = i;
            }
        }
        return n;
    }

    static bool Pointswap(Point corner, int a, int b)
    {
        Vector3 p0, p1, p2;
        p0 = corner.transform.position;
        p1 = Point.points[a].transform.position;
        p2 = Point.points[b].transform.position;
        return (Dir(p0, p1, p2) < 0 || Dir(p0, p1, p2) == 0 && Vector3.Distance(p0, p2) > Vector3.Distance(p0, p1));
    }

    public static void SortPoints(Point corner, int a, int b)
    {
        //Debug.Log(corner.name+"   "+a+"   "+b);
        if (a == b - 1) return;
        if (a == b - 2)
        {
            Point sw;
            if (Pointswap(corner,a,b-1))
            {
                sw = Point.points[b - 1];
                Point.points[b - 1] = Point.points[a];
                Point.points[a] = sw;
            }
        }
        int m = (a + b) / 2;
        SortPoints(corner, a, m);
        SortPoints(corner, m, b);
        int n1, n2;
        n1 = a;
        n2 = m;
        int n = 0;
        while (n1 < m || n2 < b)
        {
            if (n1 < m && (n2 == b || Pointswap(corner,n2,n1)))
            {
                //Debug.Log("asd: "+n1);
                copyPoints[n++] = Point.points[n1++];
            }
            else
            {
                //Debug.Log("asd: " + n2);
                copyPoints[n++] = Point.points[n2++];
            }
        }
        for (int i = 0; i < n; ++i)
        {
            Point.points[a + i] = copyPoints[i];
        }
    }

    public static void SortPoints(int corner, int a = 0, int b = -1)
    {
        if (b == -1) b = Point.numPoints;
        Point sw;
        sw = Point.points[a];
        Point.points[a] = Point.points[corner];
        Point.points[corner] = sw;
        SortPoints(Point.points[a], a+1, b);
        /*for (int i = 1; i < Point.numPoints; ++i)
        {
            for (int j = i+1; j < Point.numPoints; ++j)
            {
                if (Pointswap(Point.points[0],i,j))
                //if (Dir(Point.points[0].transform.position, Point.points[i].transform.position, Point.points[j].transform.position) > 0)
                {
                    sw = Point.points[i];
                    Point.points[i] = Point.points[j];
                    Point.points[j] = sw;
                }
            }
        } */
        if (b - a > 2)
        {
            int n = 0;
            copyPoints[n++] = Point.points[a + 1];
            for (int i = 2; i < b && Dir(Point.points[a].transform.position, Point.points[a + 1].transform.position, Point.points[a + i].transform.position) == 0; ++i)
            {
                copyPoints[n++] = Point.points[a + i];
            }
            for (int i = 0; i < n; ++i) Point.points[1 + i + a] = copyPoints[n - i - 1];
        }
    }

    void addLine(Vector3 p1, Vector3 p2)
    {
        GameObject gm =  Instantiate(linegm, (p1 + p2) / 2, Quaternion.FromToRotation(new Vector3(0,1,0), p2-p1)) as GameObject;
        gm.transform.localScale = new Vector3(gm.transform.localScale.x, Vector3.Distance(p1,p2)/2, gm.transform.localScale.z);
        linelist.Add(gm);
    }

    void DeleteLines()
    {
        foreach (GameObject gm in linelist) Destroy(gm);
        linelist.Clear();
    }

	void Update () {
        refresh -= Time.deltaTime;
        if (refresh > 0 && !enablerandomove) return;
        refresh = refreshtime;
        if (selectedoption == 0)                                                                                                                                    //Convex
        {
            DeleteLines();
            int corner = getCorner();
            SortPoints(corner);
            for (int i = 1; i < Point.numPoints; ++i) Point.points[i].gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
            int n = 0;
            copyPoints[n++] = Point.points[0];
            copyPoints[n++] = Point.points[1];
            for (int i = 2; i < Point.numPoints; ++i)
            {
                while (n >= 2 && Dir(copyPoints[n - 2].transform.position, copyPoints[n - 1].transform.position, Point.points[i].transform.position) < 0) n--;
                copyPoints[n++] = Point.points[i];
            }
            copyPoints[n++] = Point.points[0];
            for (int i = 0; i < n; ++i)
            {
                //Debug.Log(Point.points[i].name);
                Vector3 p1, p2;
                copyPoints[i].gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                p1 = copyPoints[i].transform.position;
                p2 = copyPoints[(i + 1) % n].transform.position;
                //Debug.DrawLine(p1,p2);
                addLine(p1, p2);
                //Drawing.DrawLine(p1,p2,new Color(1,0,0),1);
            }
            Point.points[0].gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            //Debug.Log("-----------");
        } else if (selectedoption == 1)                                                                                                                             //Triangle
        {
            DeleteLines();
            for (int i = 0; i < Point.numPoints; ++i) Point.points[i].gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
            Point p0, p1, p2, p3;
            p2 = p3 = null;
            p0 = Point.points[0];
            p1 = Point.points[getCorner()];
            for (int j = 0; j < 8; ++j)
            {
                p2 = p3 = null;
                p0 = Point.points[0];
                p1 = Point.points[getCorner(j)];
                for (int i = 0; i < Point.numPoints; ++i)
                {
                    if (Point.points[i] == p0 || Point.points[i] == p1) continue;
                    if (Dir(p1.transform.position, p0.transform.position, Point.points[i].transform.position) < 0)
                    {
                        if (p2 == null || Dir(p0.transform.position, p2.transform.position, Point.points[i].transform.position) > 0) p2 = Point.points[i];
                    }
                    else
                    {
                        if (p3 == null || Dir(p0.transform.position, p3.transform.position, Point.points[i].transform.position) < 0) p3 = Point.points[i];
                    }
                }
                if (p1 && p2 && p3)  if (Dir(p3.transform.position, p1.transform.position, p0.transform.position) < 0 ||
                        Dir(p1.transform.position, p2.transform.position, p0.transform.position) < 0 ||
                        Dir(p2.transform.position, p3.transform.position, p0.transform.position) < 0) p2 = p3 = null;
                if (p1 && p2 && p3) break;
            }
            if (p1 && p2 && p3)
            {
                for (int i = 0; i < Point.numPoints; ++i)
                {
                    if (p0 == Point.points[i] || p1 == Point.points[i] || p2 == Point.points[i] || p3 == Point.points[i]) continue;
                    if (Dir(p3.transform.position, p1.transform.position, Point.points[i].transform.position) > 0 &&
                        Dir(p1.transform.position, p2.transform.position, Point.points[i].transform.position) > 0 &&
                        Dir(p2.transform.position, p3.transform.position, Point.points[i].transform.position) > 0)
                    {
                        //Point.points[i].gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);


                        if (Dir(p3.transform.position, Point.points[i].transform.position, p0.transform.position) > 0 &&
                            Dir(Point.points[i].transform.position, p2.transform.position, p0.transform.position) > 0 &&
                            Dir(p2.transform.position, p3.transform.position, p0.transform.position) > 0)
                        {
                            p1 = Point.points[i];
                        }
                        else if (Dir(p3.transform.position, p1.transform.position, p0.transform.position) > 0 &&
                            Dir(p1.transform.position, Point.points[i].transform.position, p0.transform.position) > 0 &&
                            Dir(Point.points[i].transform.position, p3.transform.position, p0.transform.position) > 0)
                        {
                            p2 = Point.points[i];
                        }
                        else
                        {
                            p3 = Point.points[i];
                        }
                    }
                }
                p1.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                p2.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                p3.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                addLine(p1.transform.position, p2.transform.position);
                addLine(p2.transform.position, p3.transform.position);
                addLine(p3.transform.position, p1.transform.position);
            }
            p0.gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        }
        else if (selectedoption == 2)                                                                                                       //Pairs
        {
            DeleteLines();
            if (lastselectedoption != selectedoption)
            {
                if (Point.numPoints % 2 != 0)
                {
                    selectedoption = -1;
                    Debug.LogWarning("To use this function there must be and even number of points");
                    return;
                }
                for (int i = 0; i < Point.numPoints; ++i)
                {
                    if (i % 2 == 0) Point.points[i].gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
                    else Point.points[i].gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                }
            }
            FindPair(0,Point.numPoints);

            /*for (int i = 0; i < Point.numPoints; ++i)
            {
                Vector3 p1, p2;
                p1 = Point.points[i].transform.position;
                p2 = Point.points[(i + 1) % Point.numPoints].transform.position;
                addLine(p1, p2);
            }*/
        }

        lastselectedoption = selectedoption;
        
	}

    void FindPair(int a, int b)
    {
        if (a + 1 >= b) return;
        int corner = getCorner(0, a, b);
        SortPoints(corner, a, b);
        int num, num2;
        num = 0;
        num2 = 0;
        for (int i = a+1; i < b; ++i)
        {
            if (Point.points[i].gameObject.GetComponent<Renderer>().material.color != Point.points[a].gameObject.GetComponent<Renderer>().material.color)
            {
                if (num == num2)
                {
                    addLine(Point.points[i].transform.position, Point.points[a].transform.position);
                    FindPair(a + 1, i);
                    FindPair(i + 1, b);
                    break;
                }
                else
                {
                    num++;
                }
            }
            else num2++;
            //Debug.Log(num + " " + num2);
        }
    }


    void OnGUI()
    {
        Rect r = new Rect();
        r.width = Screen.width * 0.1f;
        r.height = r.width / 4;
        r.x = r.width * 0.2f;
        r.y = r.x;
        for (int i = 0; i < options.Length; ++i)
        {
            if (selectedoption == i)
            {
                GUI.color = new Color(0,1,0);
                GUI.Box(r,options[i]);
                GUI.color = new Color(1, 1, 1);
            }
            else
            {
                if (GUI.Button(r,options[i]))
                {
                    selectedoption = i;
                }
            }
            r.x += r.width * 1.2f;
        }
        if (enablerandomove) GUI.color = new Color(0, 1, 0);
        r.x = Screen.width - r.width * 1.2f;
        r.y = Screen.height - r.height * 1.2f;
        if (GUI.Button(r, "Random move"))
        {
            enablerandomove = !enablerandomove;
            for (int i = 0; i < Point.numPoints; ++i) Point.points[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        GUI.color = new Color(1, 1, 1);
    }
}
