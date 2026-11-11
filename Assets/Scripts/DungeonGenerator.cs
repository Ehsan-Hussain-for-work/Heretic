using System.Collections.Generic;
using System.Reflection.Metadata;
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

    //TODO [SerializeField] //missing door logic (maybe next build?)
    //TODO private GameObject door; //prefab to separate rooms

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
                        DungeonPart testRoom = genRooms[randLinkIndex];

                        if (testRoom.HasAvailableEntry(out room1Entry))
                        {
                            randGenRoom = testRoom;
                            break;
                        }
                        retryIndex++;
                    }

                    //TODO when doors can be used
                    // GameObject alignedDoor = Instantiate(door, transform.position, transform.rotation);

                    GameObject genRoomAgain; //?already in use for me, but not him

                    //TODO: when bosses are ready
                    // if (specialRooms.Count > 0)
                    // {

                    // }

                    int randIndex = Random.Range(0, rooms.Count);
                    genRoomAgain = Instantiate(rooms[randIndex], transform.position, transform.rotation);

                    if (genRoomAgain.TryGetComponent<DungeonPart>(out DungeonPart dungeonPartAgain))
                    {
                        //? dungeonPartAgain.Spawn(true);

                        if (dungeonPartAgain.HasAvailableEntry(out Transform room2Entry))
                        {
                            genRooms.Add(dungeonPartAgain);
                            // alignedDoor.transform.position = room1Entry.position;
                            // alignedDoor.transform.rotation = room2Entry.rotation;
                            AlignRooms(randGenRoom.transform, genRoomAgain.transform, room1Entry, room2Entry);

                            if (HandleIntersection(dungeonPartAgain))
                            {
                                dungeonPartAgain.UnusedEntrypoint(room2Entry);
                                randGenRoom.UnusedEntrypoint(room1Entry);
                                RetryPlacement(genRoomAgain
                                //*Todo , alignedDoor*//
                                );
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    private void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry)
    {
        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);

        room2.TransformPoint(room2Entry.position);
        room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y + angle, room2.eulerAngles.z); //3D rotation as X Y Z angles in degrees.

        Vector3 offset = room1Entry.position - room2Entry.position; //calcs the diff between room1 & room2

        room2.position += offset; //moves room2 to be next to room1

        Physics.SyncTransforms(); //ensures everything that teleports into place stays aligned, even from small rounding
    }

    private bool HandleIntersection(DungeonPart dungeonPart)
    {
        bool didIntersect = false;
        Collider[] hits = Physics.OverlapBox(dungeonPart.collider.bounds.center, dungeonPart.collider.bounds.size / 2, Quaternion.identity, roomsLM);

        foreach (Collider hit in hits)
        {
            if (hit == dungeonPart.collider)
            {
                continue;
            }
            else
            {
                didIntersect = true;
                break;
            }
        }

        return didIntersect;
    }

    //TODO
    private void RetryPlacement(GameObject item /*//TODO , GameObject door*/)
    {
        DungeonPart randGenRoom = null;
        Transform room1Entry = null;
        int totalRetry = 100;
        int retryIndex = 0;

        while (randGenRoom == null && retryIndex < totalRetry)
        {

        }
    }

    //TODO
    public void AlignRooms(Transform value1, Transform value2, Transform room1, Transform room2)
    {

    }

    public List<DungeonPart> GetDungeonParts() => genRooms;

    public bool IsGen() => isGen;
}
