using System.Collections;
using System.Collections.Generic;
using EDCViewer.Messages;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject CameraPrefab;
    public GameObject BlockPrefab;
    public GameObject StevePrefab;
    public GameObject BedPrefab;
    public TMP_Text InfoText;
    private CompetitionUpdate.Info.Stage isInfoChanged = CompetitionUpdate.Info.Stage.Ended;
    public MineralFormation mineralFormation;
    public PlayerProperty playerProperty1, playerProperty2;
    public InventoryManager playerInventory1, playerInventory2;
    //public static Dictionary<int, GameObject> Cameras = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> Blocks = new Dictionary<int,GameObject>();
    public static Dictionary<int, GameObject> Beds = new Dictionary<int,GameObject>();
    public static Dictionary<int, GameObject> PlayerSteve = new Dictionary<int, GameObject>();
    public static Dictionary<int, PlayerProperty> PlayerAttributes = new Dictionary<int, PlayerProperty>();
    public static Dictionary<int, InventoryManager> PlayerInventory = new Dictionary<int, InventoryManager>();
    public static Dictionary<string, GameObject> Mines = new Dictionary<string,GameObject>();
    public static Dictionary<string, int> OccumulatedOreCounts = new();
    public static Dictionary<string, List<GameObject>> Ores = new();

    private void Start()
    {
        mineralFormation = GameObject.Find("MineralFormation").GetComponent<MineralFormation>();
        PlayerProperty[] playerProperties= GameObject.Find("Canvas").GetComponents<PlayerProperty>();
        playerProperty1 = playerProperties[0];
        playerProperty2 = playerProperties[1];

        InventoryManager[] playerInventories = GameObject.Find("Canvas").GetComponents<InventoryManager>();
        playerInventory1 = playerInventories[0];
        playerInventory2 = playerInventories[1];

        InfoText = GameObject.Find("Canvas/InfoText").GetComponent<TMP_Text>();
    }
    private void ClearText()
    {
        InfoText.text = null;
    }
    public void AfterMessageReceivedEventHandler(object? sender, IMessage message)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (message.Type == IMessage.MessageType.CompetitionUpdate)
            {
                CompetitionUpdate competitionUpdate = (CompetitionUpdate)message;

                ////camera
                //for(int i = 0; i < competitionUpdate.cameras.Count; i++)
                //{
                //    if (!Cameras.ContainsKey(competitionUpdate.cameras[i].cameraId))
                //    {
                //        GameObject camera = Instantiate(CameraPrefab);
                //        Cameras.Add(competitionUpdate.cameras[i].cameraId, camera);
                //    }
                //}

                //chunk
                for (int i = 0; i < competitionUpdate.chunks.Count; i++)
                {
                    if (Blocks.ContainsKey(competitionUpdate.chunks[i].chunkId))
                    {
                        Blocks[competitionUpdate.chunks[i].chunkId].GetComponent<Block>().MaterialUpdate(competitionUpdate.chunks[i].height);
                    }

                    else
                    {
                        GameObject blockObj = Instantiate(BlockPrefab, new Vector3(competitionUpdate.chunks[i].position.x+0.5f, 0, competitionUpdate.chunks[i].position.y+0.5f), Quaternion.identity);
                        blockObj.transform.parent = GameObject.Find("Blocks").transform;
                        
                        Blocks.Add(competitionUpdate.chunks[i].chunkId, blockObj);
                        Block block = blockObj.GetComponent<Block>();
                        block.AddChildrenBlock();
                        block.MaterialUpdate(competitionUpdate.chunks[i].height);
                    }
                }

                //events
                for (int i = 0; i < competitionUpdate.events.Count; i++)
                {
                    //player attack event
                    if (competitionUpdate.events[i].playerAttackEvent != null)
                    {
                        PlayerSteve[competitionUpdate.events[i].playerAttackEvent.playerId].GetComponent<SteveAnimation>().Attack(PlayerSteve[competitionUpdate.events[i].playerAttackEvent.targetPlayerId].transform.position);
                    }

                    //player dig event
                    else if (competitionUpdate.events[i].playerDigEvent != null)
                    {
                        PlayerSteve[competitionUpdate.events[i].playerAttackEvent.playerId].GetComponent<SteveAnimation>().Dig(Blocks[competitionUpdate.events[i].playerDigEvent.targetChunk].transform.position);
                    }

                    //player pick up event, CompetitionUpdate.cs provides item type and item count, but I think they are useless.I just play the animation.
                    else if (competitionUpdate.events[i].playerPickUpEvent != null)
                    {
                        PlayerSteve[competitionUpdate.events[i].playerPickUpEvent.playerId].GetComponent<SteveAnimation>().PickUp();
                        mineralFormation.OreDestroy(competitionUpdate.events[i].playerPickUpEvent.mineId, competitionUpdate.events[i].playerPickUpEvent.playerId);
                    }

                    //player place block event
                    else if (competitionUpdate.events[i].playerPlaceBlockEvent != null)
                    {
                        PlayerSteve[competitionUpdate.events[i].playerPlaceBlockEvent.playerId].GetComponent<SteveAnimation>().PlaceBlock();
                    }

                    //player try attack event
                    else if (competitionUpdate.events[i].playerTryAttackEvent != null)
                    {
                        PlayerSteve[competitionUpdate.events[i].playerTryAttackEvent.playerId].GetComponent<SteveAnimation>().TryAttack();
                    }
                }

                //Info

                if (competitionUpdate.info.stage == CompetitionUpdate.Info.Stage.Ready && competitionUpdate.info.stage != isInfoChanged)
                {
                    InfoText.text = "Ready";
                    isInfoChanged = CompetitionUpdate.Info.Stage.Ready;
                    
                    Invoke(nameof(ClearText), 2.0f);

                }
                else if (competitionUpdate.info.stage == CompetitionUpdate.Info.Stage.Running && competitionUpdate.info.stage != isInfoChanged)
                {
                    InfoText.text = "Running";
                    isInfoChanged = CompetitionUpdate.Info.Stage.Running;
                    Invoke(nameof(ClearText), 2.0f);

                }
                else if (competitionUpdate.info.stage == CompetitionUpdate.Info.Stage.Battling && competitionUpdate.info.stage != isInfoChanged)
                {
                    InfoText.text = "Battling";
                    isInfoChanged = CompetitionUpdate.Info.Stage.Battling;
                    Invoke(nameof(ClearText), 2.0f);

                }
                else if (competitionUpdate.info.stage == CompetitionUpdate.Info.Stage.Finished && competitionUpdate.info.stage != isInfoChanged)
                {
                    InfoText.text = "Finished";
                    isInfoChanged = CompetitionUpdate.Info.Stage.Finished;
                    Invoke(nameof(ClearText), 2.0f);

                }
                else if (competitionUpdate.info.stage == CompetitionUpdate.Info.Stage.Ended && competitionUpdate.info.stage != isInfoChanged)
                {
                    Invoke(nameof(ClearText), 2.0f);

                    isInfoChanged = CompetitionUpdate.Info.Stage.Ended;
                    playerProperty1.playerId = -1;
                    playerProperty2.playerId = -1;
                    playerInventory1.playerId = -1;
                    playerInventory2.playerId = -1;
                    Invoke(InfoText.text = null, 2.0f);
                    Blocks.Clear();
                    Beds.Clear();
                    PlayerSteve.Clear();
                    PlayerAttributes.Clear();
                    PlayerInventory.Clear();
                    Mines.Clear();
                }



                //mines formation
                for (int i = 0; i < competitionUpdate.mines.Count; i++)
                {
                    mineralFormation.OreFormation(
                        competitionUpdate.mines[i].oreType,
                        competitionUpdate.mines[i].mineId,
                        new Vector3(competitionUpdate.mines[i].position.x + 0.5f, 0.75f, competitionUpdate.mines[i].position.y + 0.5f)
                    );
                    mineralFormation.UpdateOreInfo(
                        competitionUpdate.mines[i].oreType,
                        competitionUpdate.mines[i].mineId,
                        competitionUpdate.mines[i].accumulatedOreCount
                    );
                }

                //player
                for (int i = 0; i < competitionUpdate.players.Count; i++)
                {
                    if (!PlayerSteve.ContainsKey(competitionUpdate.players[i].playerId))
                    {
                        GameObject steve = Instantiate(StevePrefab, new Vector3(competitionUpdate.players[i].homePosition.x, 1,competitionUpdate.players[i].homePosition.y), Quaternion.identity);
                        PlayerSteve.Add(competitionUpdate.players[i].playerId, steve);
                        GameObject bed = Instantiate(BedPrefab, new Vector3(competitionUpdate.players[i].homePosition.x, 1, competitionUpdate.players[i].homePosition.y), Quaternion.identity);
                        bed.transform.Rotate(new Vector3(-90, 0, 0));
                        Beds.Add(competitionUpdate.players[i].playerId, bed);

                        if (playerProperty1.playerId == -1)
                        {
                            PlayerAttributes.Add(competitionUpdate.players[i].playerId, playerProperty1);
                            playerProperty1.playerId = competitionUpdate.players[i].playerId;
                        }
                        else if (playerProperty2.playerId == -1)
                        {
                            PlayerAttributes.Add(competitionUpdate.players[i].playerId, playerProperty2);
                            playerProperty2.playerId = competitionUpdate.players[i].playerId;
                        }

                        if (playerInventory1.playerId == -1)
                        {
                            PlayerInventory.Add(competitionUpdate.players[i].playerId, playerInventory1);
                            playerInventory1.playerId = competitionUpdate.players[i].playerId;
                        }
                        else if (playerInventory2.playerId == -1)
                        {
                            PlayerInventory.Add(competitionUpdate.players[i].playerId, playerInventory2);
                            playerInventory2.playerId = competitionUpdate.players[i].playerId;
                        }
                    }

                    //atrributes

                    PlayerAttributes[competitionUpdate.players[i].playerId].maxHP(competitionUpdate.players[i].attributes.maxHealth);
                    PlayerAttributes[competitionUpdate.players[i].playerId].HPFluctuation(competitionUpdate.players[i].health);
                    PlayerAttributes[competitionUpdate.players[i].playerId].HPScore(competitionUpdate.players[i].health);
                    PlayerAttributes[competitionUpdate.players[i].playerId].ATKFluctuation(competitionUpdate.players[i].attributes.strength);
                    PlayerAttributes[competitionUpdate.players[i].playerId].ATKScore(competitionUpdate.players[i].attributes.strength);
                    PlayerAttributes[competitionUpdate.players[i].playerId].APFluctuation(competitionUpdate.players[i].attributes.agility);
                    PlayerAttributes[competitionUpdate.players[i].playerId].APScore(competitionUpdate.players[i].attributes.agility);

                    //inventory

                    PlayerInventory[competitionUpdate.players[i].playerId].InventoryUpdate(competitionUpdate.players[i].inventory.emerald, competitionUpdate.players[i].inventory.wool);

                    //position 

                    PlayerSteve[competitionUpdate.players[i].playerId].GetComponent<SteveAnimation>().Run(new Vector3(competitionUpdate.players[i].position.x, 1, competitionUpdate.players[i].position.y), 100f);

                }
            }
        });
    }
}
