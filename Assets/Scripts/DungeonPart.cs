using System.Collections.Generic;
using UnityEngine;

public class DungeonPart : MonoBehaviour
{
    [SerializeField]
    private LayerMask roomsLM;

    [SerializeField]
    private GameObject barricade; //prevents player for doors that go nowhere 

    public List<Transform> entries;
    public new Collider collider; //checks overlapping rooms (new = hide an accessible member)

    public bool HasAvailableEntry(out Transform entrypoint)
    {
        Transform entryResult = null;
        bool result = false;

        int totalRetry = 100;
        int retryIndex = 0;

        if (entries.Count == 1)
        {
            Transform entry = entries[0];
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint res))
            {
                if (res.IsOccupied())
                {
                    result = false;
                    entryResult = null;
                }
                else
                {
                    result = true;
                    entryResult = entry;
                    res.SetOccupied();
                }
                entrypoint = entryResult;
                return result;
            }
        }

        while (entryResult == null && retryIndex < totalRetry)
        {
            int randEntryIndex = Random.Range(0, entries.Count);
            Transform entry = entries[randEntryIndex];

            if (entry.TryGetComponent<EntryPoint>(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    entryResult = entry;
                    result = true;
                    entryPoint.SetOccupied();
                    break;
                }
            }
            retryIndex++;
        }

        entrypoint = entryResult;
        return result;
    }

    public void UnusedEntrypoint(Transform entrypoint)
    {
        if (entrypoint.TryGetComponent<EntryPoint>(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }

    public void FillEmptyDoors()
    {
        entries.ForEach((entry) =>
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    GameObject wall = Instantiate(barricade);
                    //?wall.GetComponent<NetworkObject>().Spawn(true);
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }
}
