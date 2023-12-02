using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CenfomonParty : MonoBehaviour
{
    [SerializeField] List<Cenfomon> cenfomons;

    public event Action OnUpdated;

    public List<Cenfomon> Cenfomons
    {
        get
        {
            return cenfomons;
        }
        set
        {
            cenfomons = value;
            OnUpdated?.Invoke();
        }
    }

    private void Awake()
    {
        foreach (var cenfomon in cenfomons)
        {
            cenfomon.Init();
        }
    }

    private void Start()
    {
        
    }

    public Cenfomon GetHealthyCenfomon()
    {
        return cenfomons.Where(x=>x.HP > 0).FirstOrDefault();
    }

    public void AddCenfomon(Cenfomon newCenfomon)
    {
        if (cenfomons.Count < 6)
        {
            cenfomons.Add(newCenfomon);
            OnUpdated?.Invoke();
        }
        else
        {
            //Enviar al espacio Cuántico
        }
    }

    public bool CheckForEvolutions()
    {
        return cenfomons.Any(p => p.CheckForEvolution() != null); 
    }

    public IEnumerator RunEvolutions()
    {
        foreach(var cenfomon in cenfomons)
        {
            var evolution = cenfomon.CheckForEvolution();
            if(evolution != null)
            {
               yield return EvolutionManager.i.Evolve(cenfomon, evolution);
            }
        }
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static CenfomonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<CenfomonParty>();
    }
}
