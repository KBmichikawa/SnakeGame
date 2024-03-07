using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerLength : NetworkBehaviour
{
    [SerializeField]
    private GameObject tailPrefab;

    public NetworkVariable<ushort> length = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private List<GameObject> _tails;
    private Transform _lastTail;
    private Collider2D _collider;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        _tails = new List<GameObject>();
        _lastTail = transform;

        _collider = GetComponent<Collider2D>();

        if (!IsServer) length.OnValueChanged += LenghtChanged;
    }

    // this will be colled by the server
    [ContextMenu("Add Length")]
    private void AddLength()
    {
        length.Value += 1;
        InstantiateTail();
    }

    private void LenghtChanged(ushort previousValue, ushort newValue)
    {
        Debug.Log("LenghtChanged callback");
        InstantiateTail();
    }

    private void InstantiateTail()
    {
        GameObject tailGameObject = Instantiate(tailPrefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -length.Value;

        if (tailGameObject.TryGetComponent<Tail>(out Tail tail))
        {
            tail.networkedOwner = transform;
            tail.followTransform = _lastTail;
            _lastTail = tailGameObject.transform;
            Physics2D.IgnoreCollision(tailGameObject.GetComponent<Collider2D>(), _collider);
        }
        _tails.Add(tailGameObject);

    }
}
