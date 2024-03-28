using System.Collections;
using System;
using System.Collections.Generic;
using EDCViewer.Messages;
using UnityEngine;


public class MineralFormation : MonoBehaviour
{
    public GameObject IronMine;
    public GameObject GoldMine;
    public GameObject DiamondMine;
    public GameObject DiamondBlock;
    public GameObject IronBlock;
    public GameObject GoldBlock;

    public float BlockScale;
    public float MineScale;

    private void Start()
    {
        DiamondMine = Resources.Load<GameObject>("Prefabs/Diamond/diamond");
        GoldMine = Resources.Load<GameObject>("Prefabs/Gold/gold_ingot");
        IronMine = Resources.Load<GameObject>("Prefabs/Iron/iron_ingot");

        DiamondBlock = Resources.Load<GameObject>("Prefabs/DiamondBlock/DiamondBlock");
        GoldBlock = Resources.Load<GameObject>("Prefabs/GoldBlock/GoldBlock");
        IronBlock = Resources.Load<GameObject>("Prefabs/IronBlock/IronBlock");

        BlockScale = 0.25f;
        MineScale = 4.0f;
    }

    public void OreFormation(CompetitionUpdate.Mine.OreType oreType, string mineId, Vector3 orePosition)
    {
        if (oreType == CompetitionUpdate.Mine.OreType.IronOre)
        {
            IronOreFormation(mineId, orePosition);
        }
        else if (oreType == CompetitionUpdate.Mine.OreType.GoldOre)
        {
            GoldOreFormation(mineId, orePosition);
        }
        else if (oreType == CompetitionUpdate.Mine.OreType.DiamondOre)
        {
            DiamondOreFormation(mineId, orePosition);
        }
    }

    void IronOreFormation(string mineId, Vector3 IronOrePosition)
    {
        if (!Controller.Mines.ContainsKey(mineId))
        {
            Debug.Log($"Creating IronMine with id {mineId}");
            GameObject ironOre = Instantiate(IronBlock, IronOrePosition, Quaternion.identity);
            ironOre.transform.localScale = Vector3.one* BlockScale;
            Controller.Mines.Add(mineId, ironOre);
        }
    }

    

    void GoldOreFormation(string mineId, Vector3 GoldMinePosition)
    {
        if (!Controller.Mines.ContainsKey(mineId))
        {
            Debug.Log($"Creating GoldMine with id {mineId}");
            GameObject goldOre = Instantiate(GoldBlock, GoldMinePosition, Quaternion.identity);
            goldOre.transform.localScale = Vector3.one* BlockScale;
            Controller.Mines.Add(mineId, goldOre);
        }
    }

    

    void DiamondOreFormation(string mineId, Vector3 DiamondMinePosition)
    {
        if (!Controller.Mines.ContainsKey(mineId))
        {
            Debug.Log($"Creating DiamondMine with id {mineId}");
            GameObject diamondOre = Instantiate(DiamondBlock, DiamondMinePosition, Quaternion.identity);
            diamondOre.transform.localScale = Vector3.one* BlockScale;
            Controller.Mines.Add(mineId, diamondOre);
        }
    }

    public void UpdateOreInfo(CompetitionUpdate.Mine.OreType oreType, string mineId, int accumulatedOreCount)
    {
        if (Controller.Mines.ContainsKey(mineId))
        {
            Controller.OccumulatedOreCounts[mineId] = accumulatedOreCount;
            if (!Controller.Ores.ContainsKey(mineId))
            {
                Controller.Ores.Add(mineId, new());
            }
            while (Controller.Ores[mineId].Count < accumulatedOreCount)
            {
                GameObject ore = Instantiate(
                    oreType switch
                    {
                        CompetitionUpdate.Mine.OreType.DiamondOre => DiamondMine,
                        CompetitionUpdate.Mine.OreType.GoldOre => GoldMine,
                        CompetitionUpdate.Mine.OreType.IronOre => IronMine,
                        _ => throw new ArgumentOutOfRangeException($"OreType {oreType} is not defined.")
                    },
                    Controller.Mines[mineId].transform.position + new Vector3(0, 0.2f, 0),
                    Quaternion.identity
                );
                ore.transform.localScale = Vector3.one * MineScale;
                Controller.Ores[mineId].Add(ore);
            }
            while (Controller.Ores[mineId].Count > accumulatedOreCount)
            {
                Destroy(Controller.Ores[mineId][0]);
                Controller.Ores[mineId].RemoveAt(0);
            }
        }
    }

    public void OreDestroy(string mineId,int playerId)
    {
        Controller.Mines[mineId].transform.Translate((Controller.PlayerSteve[playerId].transform.position) * Time.deltaTime);
        float distance = Vector3.Distance(Controller.Mines[mineId].transform.position, Controller.PlayerSteve[playerId].transform.position);
        if(distance < 0.5f)
        {
            if (!Controller.Mines.ContainsKey(mineId))
            {
                Destroy(Controller.Mines[mineId]);
                Controller.Mines.Remove(mineId);
            }
        }
    }
}
