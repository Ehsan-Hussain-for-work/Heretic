using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

    [SerializeField]
    private GameObject entrance; //1st spot where rooms will branch from

    [SerializeField]
    private List<GameObject> rooms; //rooms that can spawn

    [SerializeField]
    private List<GameObject> specialRooms; //rooms with unique probabilities (boss rooms)

    //! [SerializeField] //"Lethal Company fire exit"? (Might delete)
    //! private List<GameObject> alternateEntrances;

    //! [SerializeField] //same as rooms, but just a different category (Might delete)
    //! private List<GameObject> hallways;

    [SerializeField]
    private GameObject door; //prefab to separate rooms

    [SerializeField]
    private int RoomNum = 10; //total num of rooms

    [SerializeField]
    private LayerMask roomsLM; //almost like a tag

    private List<DungeonPart> genRooms; //all rooms generated from the algorithm

    private bool isGen = false; //checks if room is generated or not

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        genRooms = new List<DungeonPart>();
    }

    private void Gen()
    {
        for (int i = 0; i < RoomNum; i++)
        {
            if (genRooms.Count < 1)
            {
                GameObject genRoom = Instantiate(entrance, transform.position, transform.rotation);

                if (genRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //? Needs to be implemented or is for multiplayer 
                    //dungeonPart.Spawn(true);
                    genRooms.Add(dungeonPart);
                }
                else
                {
                    DungeonPart randGenRoom = null;
                    Transform room1Entry = null;
                    int totalRetry = 100;
                    int retryIndex = 0;

                    while (randGenRoom == null && retryIndex < totalRetry)
                    {
                        int randLinkIndex = Random.Range(0, genRooms.Count);
                        DungeonPart roomToTest = genRooms[randLinkIndex];

                        if (roomToTest.HasAvailableEntry(out room1Entry))
                        {
                            randGenRoom = roomToTest;
                            break;
                        }
                        retryIndex++;
                    } 
                }
            }

        }
    }
}
