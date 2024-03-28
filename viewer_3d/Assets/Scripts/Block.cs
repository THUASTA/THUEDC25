using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject Chunk;
    public const int BlockCount = 4;
    public List<GameObject> BlockGameObjects=new();

    public string[] MaterialNameList = 
    {
        "null",
        "azalea_leaves",
        "glass",
        "magenta_wool",
        "dirt",
        "oak_planks",
        "cobblestone",
        "deepslate_brick",
        "obsidian"
    };
    private GameObject _blockPrefab;
    private  List<Material> _materials=new();
    // Start is called before the first frame update
    void Start()
    {
        Chunk = gameObject;

        _materials = new();
        foreach (string materialName in MaterialNameList)
        {
            _materials.Add(Resources.Load<Material>($"Images/BlockMaterials/Materials/{materialName}"));
        }

        Chunk.GetComponent<MeshRenderer>().material = _materials[0];

        //Chunk.transform.localScale = Vector3.one;
    }
    public void AddChildrenBlock()
    {
        _blockPrefab = Resources.Load<GameObject>("Prefabs/block");
        for (int i = 0; i < BlockCount; i++)
        {
            for (int j = 0; j < BlockCount; j++)
            {
                GameObject newBlock = Instantiate(_blockPrefab, transform);
                newBlock.transform.localPosition = new Vector3((1.0f * (2 * i + 1)) / (2 * BlockCount), 1 - 1.0f / BlockCount, (1.0f * (2 * j + 1)) / (2 * BlockCount)) - new Vector3(0.5f, 0, 0.5f);
                newBlock.transform.localScale = 1.0f / BlockCount * Vector3.one;
                BlockGameObjects.Add(newBlock);
            }
        }
    }

    public void MaterialUpdate(int height)
    {
        int currentCount = _materials.Count;
        int cnt = 0;
        foreach (string materialName in MaterialNameList)
        {
            if (cnt >= currentCount)
            {
                _materials.Add(Resources.Load<Material>($"Images/BlockMaterials/Materials/{materialName}"));
            }
            cnt++;
        }
        foreach (GameObject block in BlockGameObjects)
        {
            block.GetComponent<MeshRenderer>().material = _materials[height];
        }
    }
}
