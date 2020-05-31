using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationController : MonoBehaviour
{
    List<GeneticPathFinder> population = new List<GeneticPathFinder>();
    public int populationSize = 100;
    public int genomeLength;
    public float cutoff = 0.3f;
    [Range(0f,1f)]
    public float mutationRate = 0.01f;
    public Transform spawnPoint;
    public Transform end;
    public int survivorKeep = 5;
    public GameObject creaturePrefab;
    public Text textFitness;
    public Text textGeneration;
    private int generationCount = 1;

    private void Start()
    {
        InitPopulation();
    }
    private void Update()
    {
        if (!hasActive())
        {
            NextGeneration();
            generationCount++;
            textGeneration.text = "Generation: " + generationCount;
        }
    }

    void InitPopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject go = Instantiate(creaturePrefab, 
                spawnPoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathFinder>().
                InitCreature(new DNA(genomeLength), end.position);
            population.Add(go.GetComponent<GeneticPathFinder>());
        }
    }
    void NextGeneration()
    {
        int survivorCut = Mathf.RoundToInt(populationSize * cutoff);
        List<GeneticPathFinder> survivors = new List<GeneticPathFinder>();
        for(int i = 0; i < survivorCut; i++)
        {
            survivors.Add(GetFittest());
        }
        for(int i = 0; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
        }
        population.Clear();

        for (int i = 0; i < survivorKeep; i++)
        {
            GameObject go = Instantiate(creaturePrefab,
                    spawnPoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathFinder>().InitCreature(survivors[i].dna, end.position);
            population.Add(go.GetComponent<GeneticPathFinder>());

        }
        while(population.Count < populationSize)
        {
            for(int i = 0; i < survivors.Count; i++)
            {
                GameObject go = Instantiate(creaturePrefab, 
                    spawnPoint.position, Quaternion.identity);
                go.GetComponent<GeneticPathFinder>().
                    InitCreature(new DNA(survivors[i].dna, 
                    survivors[Random.Range(0, 10)].dna), end.position);
                population.Add(go.GetComponent<GeneticPathFinder>());
                if (population.Count >= populationSize)
                {
                    break;
                }
            }
        }
        for(int i = 0; i < survivors.Count; i++)
        {
            Destroy(survivors[i].gameObject);
        }
    }
    GeneticPathFinder GetFittest()
    {
        float maxFittness = float.MinValue;
        int index = 0;
        for(int i = 0; i < population.Count; i++)
        {
            if(population[i].fitness > maxFittness)
            {
                maxFittness = population[i].fitness;
                index = i;
            }
        }
        GeneticPathFinder fittest = population[index];
        textFitness.text = "Fitness: " + population[index].fitness;
        population.Remove(fittest);
        return fittest;
    }
    bool hasActive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if (!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }
}
