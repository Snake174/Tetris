using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] shapes;
    public static System.Random random = new System.Random(System.Environment.TickCount);
    private int poolSize = 0;

    /// <summary>
    /// Словарь с индексом фигуры и процентом её выпадения
    /// </summary>
    private readonly Dictionary<int, int> dict = new Dictionary<int, int>()
    {
        // GroupI
        { 0, 10 },
        // GroupJ
        { 1, 15 },
        // GroupL
        { 2, 15 },
        // GroupO
        { 3, 10 },
        // GroupS
        { 4, 15 },
        // GroupT
        { 5, 20 },
        // GroupZ
        { 6, 15 },
    };

    /// <summary>
    /// Возвращает индекс фигуры в словаре с учётом процента её выпадения
    /// </summary>
    /// <returns></returns>
    private int GetObjectByWeight()
    {
        int randomNumber = random.Next(0, poolSize) + 1;
        int accumulatedProbability = 0;

        foreach (KeyValuePair<int, int> p in dict)
        {
            accumulatedProbability += p.Value;

            if (randomNumber <= accumulatedProbability)
            {
                return p.Key;
            }
        }

        return 0;
    }

    public void SpawnNext()
    {
        Instantiate(shapes[GetObjectByWeight()], transform.position, Quaternion.identity);
    }

    void Start()
    {
        foreach (KeyValuePair<int, int> p in dict)
        {
            poolSize += p.Value;
        }

        SpawnNext();
    }
}
