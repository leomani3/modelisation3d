using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.IO;

public class OffReader : MonoBehaviour
{
    public Material mat;
    public Vector3[] vertices;
    public int[] faces;
    public Vector3[] normals;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
        Mesh msh = new Mesh();

        readOff("customCube");
        vertices = moveToCenter(vertices);
        vertices = normaliseMesh(vertices);
        normals = computeNormals();

        msh.vertices = vertices;
        msh.triangles = faces;
        msh.normals = normals;
        //msh.RecalculateNormals();

        writeOff("blbl", vertices, faces);



        GetComponent<MeshFilter>().mesh = msh;
        GetComponent<MeshRenderer>().material = mat;
    }

    public Vector3 computeCentroid(Vector3[] vertices)
    {
        Vector3 sum = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < vertices.Length; i++)
        {
            sum += vertices[i];
        }
        return sum / vertices.Length;
    }

    public Vector3 computeFaceNormal(int index)
    {
        Vector3 p1 = vertices[faces[(index * 3)]];
        Vector3 p2 = vertices[faces[(index * 3) + 1]];
        Vector3 p3 = vertices[faces[(index * 3) + 2]];

        return (Vector3.Cross(p2 - p1, p3 - p1) + Vector3.Cross(p1 - p2, p3 - p2) + Vector3.Cross(p1 - p3, p2 - p3)) / 3;
    }

    public Vector3[] computeNormals()
    {
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normal = new Vector3(0f, 0f, 0f);
            int facesNb = 0;
            for (int indexFace = 0, j = 0; j < faces.Length/3; j++)
            {
                if (faces[indexFace] == i || faces[indexFace + 1] == i || faces[indexFace + 2] == i)
                {
                    normal += computeFaceNormal(j);
                    facesNb++;
                }
                indexFace += 3;
            }
            normals[i] = normal / facesNb;
        }
        return normals;
    }

    public Vector3[] moveToCenter(Vector3[] vertices)
    {
        Vector3 center = computeCentroid(vertices);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] - center;
        }
        return vertices;
    }


    public Vector3[] normaliseMesh(Vector3[] vertices)
    {
        //on détermine la plus grande norme
        float max = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            if(Mathf.Abs(vertices[i].magnitude) > max)
            {
                max = Mathf.Abs(vertices[i].magnitude);
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] / max;
        }
        return vertices;
    }


    public void writeOff(string fileName, Vector3[] vertices, int[] faces)
    {
        StreamWriter writer = new StreamWriter("Assets/"+fileName+".off");

        writer.WriteLine("OFF");
        writer.WriteLine(vertices.Length+" "+faces.Length/3+" "+0);

        for (int i = 0; i < vertices.Length; i++)
            writer.WriteLine(vertices[i].x.ToString(new CultureInfo("en-US"))+" "+ vertices[i].y.ToString(new CultureInfo("en-US"))+" "+ vertices[i].z.ToString(new CultureInfo("en-US")));
        for (int i = 0; i < faces.Length; i+=3)
            writer.WriteLine("3 "+faces[i]+" "+ faces[i + 1]+" "+ faces[i + 2]);

        writer.Close();
    }

    public void readOff(string fileName)
    {
        int index = 1;
        string[] lines = System.IO.File.ReadAllLines(@"Assets/"+fileName+".off");
        string[] tmp = lines[index].Split(' ');
        index++;

        int nbVertex  = int.Parse(tmp[0]);
        int nbFace  = int.Parse(tmp[1]);
        int nbEdge = int.Parse(tmp[2]);

        vertices = new Vector3[nbVertex];
        faces = new int[nbFace * 3];

        //points
        for (int i = 0; i < nbVertex; i++)
        {
            tmp = lines[index].Split(' ');
            index++;
            float x = float.Parse(tmp[0], CultureInfo.InvariantCulture);
            float y = float.Parse(tmp[1], CultureInfo.InvariantCulture);
            float z = float.Parse(tmp[2], CultureInfo.InvariantCulture);
            vertices[i] = new Vector3(x, y, z);
        }

        //face
        int indexTriangle = 0;
        for (int i = 0; i < nbFace; i++)
        {
            tmp = lines[index].Split(' ');
            index++;

            int ind1 = int.Parse(tmp[1]);
            int ind2 = int.Parse(tmp[2]);
            int ind3 = int.Parse(tmp[3]);

            faces[indexTriangle] = ind1; 
            faces[indexTriangle + 1] = ind2; 
            faces[indexTriangle + 2] = ind3;
            indexTriangle += 3;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(computeCentroid(vertices), 0.01f);
    }
}
