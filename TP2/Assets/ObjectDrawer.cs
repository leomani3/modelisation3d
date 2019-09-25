using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrawer : MonoBehaviour
{
    private Mesh msh;

    [Range(1, 20)]
    public int planWidth;
    [Range(1, 20)]
    public int planHeight;
    public Material planMat;

    public int sphereHeight;
    public int sphereRadius;
    public int nbMeridien;

    private void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        msh = new Mesh();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            drawPlan(planWidth, planHeight, msh);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            drawCylinder(sphereHeight, sphereRadius, nbMeridien, msh);
        }
    }

    public void drawCylinder(int h, int radius, int nbMeridien, Mesh msh)
    {
        msh.Clear();

        Vector3[] pointsSphere = new Vector3[nbMeridien * 2];
        int[] triangles = new int[nbMeridien * 6];

        //Points
        int indexPoints = 0;
        float x, y, z;
        for (int i = 0; i < nbMeridien; i++)
        {
            float angle = (Mathf.PI * 2) / nbMeridien * i;

            //face du bas
            x = radius * Mathf.Cos(angle);
            y = -sphereHeight;
            z = radius * Mathf.Sin(angle);

            Vector3 pHaut = new Vector3(x, y ,z );
            pointsSphere[i] = pHaut;

            //face du haut
            x = radius * Mathf.Cos(angle);
            y = sphereHeight;
            z = radius * Mathf.Sin(angle);

            Vector3 pBas = new Vector3(x, y, z);
            pointsSphere[nbMeridien + i] = pBas;
        }

        //triangles

    }

    public void drawPlan(int planWidth, int planHeight, Mesh msh)
    {
        msh.Clear();

        Vector3[] pointsPlan = new Vector3[(planWidth+1) * (planHeight+1)];
        int[] triangles = new int[planWidth * planHeight * 6];

        //Points
        int indexPoints = 0;
        for (int j = 0; j <= planHeight; j++)
        {
            for (int i = 0; i <= planWidth; i++)
            {
                pointsPlan[indexPoints] = new Vector3(i, 0, j);
                indexPoints++;
            }
        }

        //Triangles
        int indexTriangle=0;
        for (int j = 0; j < planHeight; j++)
        {
            for (int i = 0; i < planWidth; i++)
            {
                triangles[indexTriangle] = j * (planWidth + 1) + i;
                triangles[indexTriangle + 1] = (j+1) * (planWidth+1) + i;
                triangles[indexTriangle + 2] = (j + 1) * (planWidth + 1) + (i + 1);

                triangles[indexTriangle + 3] = (j) * (planWidth + 1) + (i);
                triangles[indexTriangle + 4] = (j + 1) * (planWidth + 1) + (i + 1);
                triangles[indexTriangle + 5] = (j) * (planWidth+1) + (i + 1);

                indexTriangle += 6;
            }
        }

        msh.vertices = pointsPlan;
        msh.triangles = triangles;
        msh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = planMat;
    }

    private void OnDrawGizmos()
    {
    }

}
