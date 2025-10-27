using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.UI;
public enum CaseMapState
{
    Vide,
    Salle,
    None
}

//fonction recupe case dispo autour de la case choisi aleatoirement()
//fonction qui regarde si on peux crée la salle, elle verifie que ca fais pas un paté

public class MapGéreratorV2 : MonoBehaviour
{
    public int NbSalles;
    public int hauteurNbSalleMax;
    public int largeurNbSalleMax;
    public GameObject sallePrefabs;
    public GameObject videPrefabs;
    public List<List<CaseMapState>> EnumSalleListe;
    private List<Vector2Int> listPosSalle;

    void Start()
    {
        EnumSalleListe = new();
        listPosSalle = new();
        if (NbSalles > hauteurNbSalleMax * largeurNbSalleMax / 2)
        {
            Debug.LogError("Trop de salles !!!");
        }
        else
        {
            generateMap();
            generateMapInWorld();
        }
    }

    private void generateMap()
    {
        for (int y = 0; y < hauteurNbSalleMax; y++)
        {
            List<CaseMapState> newList = new List<CaseMapState>();
            for (int x = 0; x < largeurNbSalleMax; x++)
            {
                newList.Add(CaseMapState.None);
            }
            EnumSalleListe.Add(newList);
        }
        EnumSalleListe[hauteurNbSalleMax / 2][largeurNbSalleMax / 2] = CaseMapState.Salle;
        listPosSalle.Add(new Vector2Int(largeurNbSalleMax / 2, hauteurNbSalleMax / 2));

        for (int i = 0; i < NbSalles; i++)
        {
            Debug.Log("listPosSalle.Count : " + listPosSalle.Count);
            List<Vector2Int> caseDispo = GetCaseDispo(listPosSalle[Random.Range(0, listPosSalle.Count)]);
            Debug.Log("caseDispo.Count : " + caseDispo.Count);
            int nbCaseDispo = caseDispo.Count;
            if (nbCaseDispo > 0)
            {
                Vector2Int newSalle = caseDispo[Random.Range(0, nbCaseDispo)];
                listPosSalle.Add(newSalle);
                EnumSalleListe[newSalle.y][newSalle.x] = CaseMapState.Salle;
            }
            else
            {
                //i--;
            }
        }


    }

    private List<Vector2Int> GetCaseDispo(Vector2Int salleObserve)
    {
        List<Vector2Int> result = new();
        List<Vector2Int> resultFinal = new();
        if (EnumSalleListe[salleObserve.y + 1][salleObserve.x] == CaseMapState.None)//nord
        {
            result.Add(new Vector2Int(salleObserve.x, salleObserve.y + 1));
        }
        if (EnumSalleListe[salleObserve.y - 1][salleObserve.x] == CaseMapState.None)//sud
        {
            result.Add(new Vector2Int(salleObserve.x, salleObserve.y - 1));
        }
        if (EnumSalleListe[salleObserve.y][salleObserve.x + 1] == CaseMapState.None)//est
        {
            result.Add(new Vector2Int(salleObserve.x + 1, salleObserve.y));
        }
        if (EnumSalleListe[salleObserve.y][salleObserve.x - 1] == CaseMapState.None)//ouest
        {
            result.Add(new Vector2Int(salleObserve.x - 1, salleObserve.y));
        }

        foreach (Vector2Int v in result)
        {
            if (EnumSalleListe[v.y][v.x - 1] == CaseMapState.Salle)
            {
                if (!(EnumSalleListe[v.y + 1][v.x] == CaseMapState.Salle || EnumSalleListe[v.y - 1][v.x] == CaseMapState.Salle))
                {
                    resultFinal.Add(v);
                }
            }
            if (EnumSalleListe[v.y][v.x + 1] == CaseMapState.Salle)
            {
                if (!(EnumSalleListe[v.y + 1][v.x] == CaseMapState.Salle || EnumSalleListe[v.y - 1][v.x] == CaseMapState.Salle))
                {
                    resultFinal.Add(v);
                }
            }
            if (EnumSalleListe[v.y - 1][v.x] == CaseMapState.Salle)
            {
                if (!(EnumSalleListe[v.y][v.x + 1] == CaseMapState.Salle || EnumSalleListe[v.y][v.x - 1] == CaseMapState.Salle))
                {
                    resultFinal.Add(v);
                }
            }
            if (EnumSalleListe[v.y - 1][v.x] == CaseMapState.Salle)
            {
                if (!(EnumSalleListe[v.y][v.x + 1] == CaseMapState.Salle || EnumSalleListe[v.y][v.x + 1] == CaseMapState.Salle))
                {
                    resultFinal.Add(v);
                }
            }
            //Debug.Log("result : " + resultFinal.Count);
        }

        return resultFinal;
    }


    private void generateMapInWorld()
    {
        for (int y = 0; y < largeurNbSalleMax; y++)
        {
            for (int x = 0; x < hauteurNbSalleMax; x++)
            {
                GameObject newSalle;
                switch (EnumSalleListe[y][x])
                {
                    case CaseMapState.None:
                        newSalle = Instantiate(videPrefabs, transform.position, new quaternion(), transform);
                        newSalle.transform.Translate(new Vector2(x, y));
                        break;
                    case CaseMapState.Salle:
                        newSalle = Instantiate(sallePrefabs, transform.position, new quaternion(), transform);
                        newSalle.transform.Translate(new Vector2(x, y));
                        break;
                }
            }
        }
    }
}
