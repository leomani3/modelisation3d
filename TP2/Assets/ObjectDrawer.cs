using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrawer : MonoBehaviour
{
    public Vector3[] pointsSphere;
    private Mesh msh;

    public Material mat;

    [Header("Plan")]
    [Range(1, 20)]
    public int planWidth;
    [Range(1, 20)]
    public int planHeight;

    [Header("Cylinder")]
    [Range(1, 20)]
    public int cylinderHeight;
    [Range(1, 20)]
    public int cylinderRadius;
    [Range(1, 100)]
    public int nbMeridienCylinder;

    [Header("Sphere")]
    [Range(1, 20)]
    public int sphereRadius;
    [Range(1, 100)]
    public int nbMeridienSphere;
    [Range(1, 100)]
    public int nbParallelSphere;

    private void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        msh = new Mesh();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            drawPlan(planWidth, planHeight, msh);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            drawCylinder(cylinderHeight, cylinderRadius, nbMeridienCylinder, msh);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            drawSphere(sphereRadius, nbMeridienSphere, nbParallelSphere, msh);
        }
    }

    public void drawSphere(int radius, int nbMeridien, int nbParallel, Mesh msh)
    {
        msh.Clear();

        pointsSphere = new Vector3[nbMeridien * (nbParallel-1) + 2];
        int[] triangles = new int[(nbMeridien * nbParallel * 6) + (nbMeridien * 3)];

        //Points

        //pole nord
        pointsSphere[pointsSphere.Length - 2] = new Vector3(0, radius, 0);
        //pole sud
        pointsSphere[pointsSphere.Length - 1] = new Vector3(0, -radius, 0);

        float x, y, z;
        int indexPoint = 0;
        for (int j = 1; j < nbParallel; j++)
        {
            float phi = Mathf.PI / nbParallel * j;
            for (int i = 0; i < nbMeridien; i++)
            {
                float theta = 2 * Mathf.PI / nbMeridien * i;

                x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                y = radius * Mathf.Cos(phi);
                z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);

                pointsSphere[indexPoint] = new Vector3(x, y, z); ;
                indexPoint++;
            }
        }


        //triangles
        int indexTriangle = 0;
        //face du haut
        for (int i = 0; i < nbMeridien; i++)
        {
            triangles[indexTriangle] = i;
            triangles[indexTriangle + 1] = pointsSphere.Length - 2; ;
            triangles[indexTriangle + 2] = i + 1;
            if (i != nbMeridien-1)
            {
                indexTriangle += 3;
            }
        }
        triangles[indexTriangle] = nbMeridien - 1;
        triangles[indexTriangle + 1] = pointsSphere.Length-2;
        triangles[indexTriangle + 2] = 0;
        indexTriangle += 3;


        //faces lattérales 
        for (int j = 0; j < nbParallel - 2; j++)
        {
            for (int i = 0; i < nbMeridien - 1; i++)
            {
                triangles[indexTriangle] = (j + 1) * nbMeridien + (i);
                triangles[indexTriangle + 1] = (j) * nbMeridien + (i);
                triangles[indexTriangle + 2] = (j) * nbMeridien + (i+1);

                triangles[indexTriangle + 3] = (j + 1) * nbMeridien + (i);
                triangles[indexTriangle + 4] = (j) * nbMeridien + (i + 1);
                triangles[indexTriangle + 5] = (j + 1) * nbMeridien + (i + 1);

                if (i != nbMeridien - 1)
                {
                    indexTriangle += 6;
                }
            }
            triangles[indexTriangle] = (j + 1) * nbMeridien + (nbMeridien - 1);
            triangles[indexTriangle + 1] = (j) * nbMeridien + (nbMeridien - 1);
            triangles[indexTriangle + 2] = (j) * nbMeridien;

            triangles[indexTriangle + 3] = (j + 1) * nbMeridien + (nbMeridien - 1);
            triangles[indexTriangle + 4] = (j) * nbMeridien;
            triangles[indexTriangle + 5] = (j + 1) * nbMeridien;

            indexTriangle += 6;
        }

        //face du bas
        for (int i = 0; i < nbMeridien-1; i++)
        {
            triangles[indexTriangle] = nbMeridien * (nbParallel-2) + i;
            triangles[indexTriangle + 1] = nbMeridien * (nbParallel - 2) + i + 1;
            triangles[indexTriangle + 2] = pointsSphere.Length - 1; ;
            if (i != nbMeridien - 1)
            {
                indexTriangle += 3;
            }
        }
        triangles[indexTriangle] = nbMeridien * (nbParallel - 2) + nbMeridien - 1;
        triangles[indexTriangle + 1] = nbMeridien * (nbParallel - 3) + nbMeridien;
        triangles[indexTriangle + 2] = pointsSphere.Length - 1;

        msh.vertices = pointsSphere;
        msh.triangles = triangles;
        msh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    public void drawCylinder(int h, int radius, int nbMeridien, Mesh msh)
    {
        msh.Clear();

        Vector3[] pointsCylinder = new Vector3[nbMeridien * 2 + 2];
        int[] triangles = new int[(nbMeridien * 6) + (nbMeridien * 3 * 2)];

        //Points
        float x, y, z;
        for (int i = 0; i < nbMeridien; i++)
        {
            float angle = (Mathf.PI * 2) / nbMeridien * i;

            //face du bas
            x = radius * Mathf.Cos(angle);
            y = -cylinderHeight / 2;
            z = radius * Mathf.Sin(angle);

            Vector3 pHaut = new Vector3(x, y ,z );
            pointsCylinder[i] = pHaut;

            //face du haut
            x = radius * Mathf.Cos(angle);
            y = cylinderHeight / 2;
            z = radius * Mathf.Sin(angle);

            Vector3 pBas = new Vector3(x, y, z);
            pointsCylinder[nbMeridien + i] = pBas;
        }
        pointsCylinder[pointsCylinder.Length - 2] = new Vector3(0, -cylinderHeight / 2, 0); //centre face bas
        pointsCylinder[pointsCylinder.Length - 1] = new Vector3(0, cylinderHeight / 2, 0); //centre face haut

        //triangles

        //faces lattéralles
        int indexTriangle = 0;
        for (int i = 0; i < nbMeridien; i++)
        {
            triangles[indexTriangle] = i;
            triangles[indexTriangle + 1] = nbMeridien + i;
            triangles[indexTriangle + 2] = nbMeridien + i + 1;

            triangles[indexTriangle + 3] = i;
            triangles[indexTriangle + 4] = nbMeridien + i + 1;
            triangles[indexTriangle + 5] = i + 1;

            if (i != nbMeridien-1)//pour ne pas incrémenter pour rien à la dernière boucle car on place la dernière face à la main
            {
                indexTriangle += 6;
            }
        }
        triangles[indexTriangle] = nbMeridien - 1;
        triangles[indexTriangle + 1] = nbMeridien * 2 -1;
        triangles[indexTriangle + 2] = nbMeridien;

        triangles[indexTriangle + 3] = nbMeridien - 1;
        triangles[indexTriangle + 4] = nbMeridien;
        triangles[indexTriangle + 5] = 0;
        indexTriangle += 6;

        //face du bas
        for (int i = 0; i < nbMeridien; i++)
        {
            triangles[indexTriangle] = pointsCylinder.Length - 2;
            triangles[indexTriangle + 1] = i;
            triangles[indexTriangle + 2] = i + 1;

            if (i != nbMeridien - 1)
            {
                indexTriangle += 3;
            }
        }
        triangles[indexTriangle] = pointsCylinder.Length - 2;
        triangles[indexTriangle + 1] = nbMeridien-1;
        triangles[indexTriangle + 2] = 0;
        indexTriangle += 3;

        //face du haut
        for (int i = 0; i < nbMeridien; i++)
        {
            triangles[indexTriangle] = pointsCylinder.Length - 1;
            triangles[indexTriangle + 1] = nbMeridien + i + 1;
            triangles[indexTriangle + 2] = nbMeridien +  i;

            if (i != nbMeridien - 1)
            {
                indexTriangle += 3;
            }
        }
        triangles[indexTriangle] = pointsCylinder.Length - 1;
        triangles[indexTriangle + 1] = nbMeridien +  0;
        triangles[indexTriangle + 2] = nbMeridien +  nbMeridien - 1;


        msh.vertices = pointsCylinder;
        msh.triangles = triangles;
        msh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = mat;

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
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    private void OnDrawGizmos()
    {
        /*for (int i = 0; i < pointsSphere.Length; i++)
        {
            Gizmos.DrawSphere(pointsSphere[i], 1);
        }*/
    }

}
