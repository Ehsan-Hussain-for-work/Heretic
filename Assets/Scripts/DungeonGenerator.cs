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

    private void Start()
    {
        Instance = this;
        genRooms = new List<DungeonPart>();

        Gen();

        //loop might be unnecessary
        foreach (DungeonPart room in genRooms)
        {
            room.FillEmptyDoors();
        }
        isGen = true;
    }

    private void Gen()
    {
        for (int i = 0; i < RoomNum; i++)
        {
            if (genRooms.Count < 1)
            {
                GameObject genRoom = Instantiate(
                    entrance, //where all rooms will start from
                    transform.position,
                    transform.rotation
                );

                if (genRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //? Needs to be implemented or is for multiplayer 
                    //dungeonPart.Spawn(true);
                    genRooms.Add(dungeonPart);
                }

            }
            else
            {
                DungeonPart randGenRoom = null;
                Transform room1Entry = null;
                int totalRetry = 100;
                int retryIndex = 0;

                //In cases where Random lasts too long
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

                if (randGenRoom == null || room1Entry == null)
                {
                    Debug.LogWarning("No available room entry points!");
                    break;
                }

                //TODO when doors can be used
                // GameObject alignedDoor = Instantiate(door, transform.position, transform.rotation);
                //door.GetComponent().Spawn(true); //?not working

                GameObject newGenRoom; //?already in use for me, but not him

                //TODO: when bosses are ready
                // if (specialRooms.Count > 0)
                // {

                // }

                int randIndex = Random.Range(0, rooms.Count);
                newGenRoom = Instantiate(
                    rooms[randIndex],
                    transform.position,
                    transform.rotation
                );

                if (newGenRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //? dungeonPart.Spawn(true);

                    if (dungeonPart.HasAvailableEntry(out Transform room2Entry))
                    {
                        genRooms.Add(dungeonPart);
                        //TODO alignedDoor.transform.position = room1Entry.position;
                        //TODO alignedDoor.transform.rotation = room2Entry.rotation;
                        AlignRooms(
                            //? randGenRoom.transform, //arg 1 isn't used.
                            newGenRoom.transform,
                            room1Entry,
                            room2Entry);

                        if (CheckIntersection(dungeonPart))
                        {
                            dungeonPart.UnusedEntry(room2Entry);
                            randGenRoom.UnusedEntry(room1Entry);
                            RetryPlacement(newGenRoom /*//Todo , alignedDoor*/);
                            continue;
                        }
                    }
                }
            }
        }
    }

    private void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry)
    {
        //forward = Z axes facing outward
        float angle = Vector3.SignedAngle(
            room2Entry.forward,
            -room1Entry.forward,
            Vector3.up
        );

        // room2.TransformPoint(room2Entry.position);
        // room2.eulerAngles = new Vector3(
        //     room2.eulerAngles.x,
        //     room2.eulerAngles.y + angle,
        //     room2.eulerAngles.z
        // ); //room2 faces room1 (eulerAngles = X Y Z angles in 3D degrees rotation)
        room2.Rotate(Vector3.up, angle);

        //calcs the diff between room1 & room2
        Vector3 offset = room1Entry.position - room2Entry.position;

        room2.position += offset; //moves room2 to be next to room1

        //everything teleports into place aligned, even from rounding values
        Physics.SyncTransforms();
    }

    private bool CheckIntersection(DungeonPart dungeonPart)
    {
        bool didIntersect = false;
        Collider[] hits = Physics.OverlapBox(
            dungeonPart.collider.bounds.center,
            dungeonPart.collider.bounds.size / 2, //apparently OverlapBox() doubles this
            Quaternion.identity, roomsLM //apparently this isn't taking rotations into count
        );

        foreach (Collider hit in hits)
        {
            if (hit == dungeonPart.collider) //room hits itself
            {
                continue; //"we don't care" response
            }

            if (hit != dungeonPart.collider) //room hits something else
            {
                didIntersect = true;
                break;
            }
        }

        return didIntersect;
    }

    //might need a better safety feature
    private void RetryPlacement(GameObject item /*//TODO , GameObject door*/)
    {
        DungeonPart randGenRoom = null;
        Transform room1Entry = null;
        int totalRetry = 100;
        int retryIndex = 0;

        while (randGenRoom == null && retryIndex < totalRetry) //no infinite loop
        {
            int randLinkIndex = Random.Range(0, genRooms.Count /*- 1*/);

            DungeonPart testRoom = genRooms[randLinkIndex];

            if (testRoom.HasAvailableEntry(out room1Entry))
            {
                randGenRoom = testRoom;
                break;
            }
            retryIndex++;
        }

        if (item.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
        {
            if (dungeonPart.HasAvailableEntry(out Transform room2Entry))
            {
                //TODO when doors can be used
                // door.transform.position = room1Entry.transform.position;
                // door.transform.rotation = room1Entry.transform.rotation;
                AlignRooms(
                    //? randGenRoom.transform, //removed, cuz argument 1 isn't used.
                    item.transform,
                    room1Entry,
                    room2Entry
                );

                if (CheckIntersection(dungeonPart))
                {
                    //apparently this'll prevent retries on bad rooms
                    genRooms.Remove(dungeonPart);

                    dungeonPart.UnusedEntry(room2Entry);
                    randGenRoom.UnusedEntry(room1Entry);

                    RetryPlacement(item);
                }
            }

        }
    }

    public List<DungeonPart> GetDungeonParts() => genRooms;

    public bool IsGen() => isGen;
}
