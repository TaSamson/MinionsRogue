
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum CaseMapState2
{
    Vide,
    Salle,
    None
}

public enum Dir
{
    Nord,
    Sud,
    Est,
    Ouest
}

public class MapGénerato : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int NbSalles;
    public int hauteurNbSalleMax;
    public int largeurNbSalleMax;
    public GameObject sallePrefabs;
    private List<Vector2Int> listPosSalle;
    public List<List<bool>> listlistSalle;
    public GameObject[] salleListe;
    void Start()
    {
        listPosSalle = new();
        listlistSalle = new();
        InitListSalle();
        if (NbSalles > hauteurNbSalleMax * largeurNbSalleMax / 2)
        {

        }
        else
        {
            GenerateMap();
            GenerateMapInWorld();
        }
    }

    private void InitListSalle()
    {
        listlistSalle = new List<List<bool>>();
        for (int y = 0; y < hauteurNbSalleMax; y++)
        {
            List<bool> ligne = new List<bool>();
            for (int x = 0; x < largeurNbSalleMax; x++)
            {
                ligne.Add(false);
            }
            listlistSalle.Add(ligne);
        }
    }

    private void GenerateMap()
    {
        List<Dir> listDir = new()
        {
            Dir.Est,
            Dir.Sud,
            Dir.Nord,
            Dir.Ouest
        };

        listPosSalle.Add(new Vector2Int(largeurNbSalleMax / 2, hauteurNbSalleMax / 2));
        listlistSalle[hauteurNbSalleMax / 2][largeurNbSalleMax / 2] = true;

        for (int i = 0; i < NbSalles; i++)
        {
            int tempcase = Random.Range(0, listPosSalle.Count);
            Dir tempDir = listDir[Random.Range(0, listDir.Count)];
            if (CanIPutNewCase(tempDir, listPosSalle[tempcase]))
            {
                AddCase(tempDir, listPosSalle[tempcase]);
            }
            else
            {
                i--;
            }

        }
    }

    private void AddCase(Dir dir, Vector2Int currentCase)
    {
        Vector2Int tempVec = new();
        switch (dir)
        {
            case Dir.Nord:
                tempVec = new Vector2Int(currentCase.x, currentCase.y + 1);
                break;
            case Dir.Sud:
                tempVec = new Vector2Int(currentCase.x, currentCase.y - 1);
                break;
            case Dir.Est:
                tempVec = new Vector2Int(currentCase.x + 1, currentCase.y);
                break;
            case Dir.Ouest:
                tempVec = new Vector2Int(currentCase.x - 1, currentCase.y);
                break;

        }
        listlistSalle[tempVec.y][tempVec.x] = true;
        listPosSalle.Add(tempVec);
    }

    private int GetNumberVoisin(Vector2Int currentCase, int verif)
    {
        int res = 0;
        Vector2Int temp = new();
        if (currentCase.y + 1 < hauteurNbSalleMax)
        {
            temp.x = currentCase.x;
            temp.y = currentCase.y + 1;
            if (listlistSalle[temp.y + 1][temp.x])
            {
                res++;
            }
        }
        if (currentCase.x + 1 < largeurNbSalleMax)
        {
            temp.x = currentCase.x + 1;
            temp.y = currentCase.y;
            if (listlistSalle[currentCase.y][currentCase.x + 1])
            {
                res++;
            }
        }
        if (currentCase.y - 1 > 0)
        {
            temp.x = currentCase.x;
            temp.y = currentCase.y - 1;
            if (listlistSalle[currentCase.y - 1][currentCase.x])
            {
                res++;
            }
        }
        if (currentCase.x - 1 > 0)
        {
            temp.x = currentCase.x - 1;
            temp.y = currentCase.y;
            if (listlistSalle[currentCase.y][currentCase.x - 1])
            {
                res++;
            }
        }
        if (verif < 1)
        {
            res += GetNumberVoisin(temp, 1);
        }
        return res;
    }

    private bool CanIPutNewCase(Dir dir, Vector2Int currentCase)
    {
        bool res = false;
        Vector2Int tempVec = new();

        switch (dir)
        {
            case Dir.Nord:
                tempVec = new Vector2Int(currentCase.x, currentCase.y + 1);
                res = currentCase.y + 1 < hauteurNbSalleMax;
                break;
            case Dir.Sud:
                tempVec = new Vector2Int(currentCase.x, currentCase.y - 1);
                res = currentCase.y - 1 >= 0;
                break;
            case Dir.Est:
                tempVec = new Vector2Int(currentCase.x + 1, currentCase.y);
                res = currentCase.x + 1 < largeurNbSalleMax;
                break;
            case Dir.Ouest:
                tempVec = new Vector2Int(currentCase.x - 1, currentCase.y);
                res = currentCase.x - 1 >= 0;
                break;

        }
        foreach (Vector2Int v in listPosSalle)
        {
            if (tempVec == v)
            {
                res = res & false;
            }
        }

        if (GetNumberVoisin(tempVec, 0) > 2)
        {
            res = res & false;
        }
        //Debug.Log(GetNumberVoisin(tempVec));



        return res;
    }

    private void GenerateMapInWorld()
    {
        GameObject newSalle;
        foreach (Vector2Int vI in listPosSalle)
        {
            newSalle = Instantiate(sallePrefabs, transform.position, new quaternion(), transform);
            newSalle.transform.Translate(new Vector2(vI.x * 2, vI.y * 2));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
