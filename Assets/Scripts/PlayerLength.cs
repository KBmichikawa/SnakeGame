using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;


public class PlayerLength : NetworkBehaviour
{
    [SerializeField]
    private GameObject tailPrefab;

    public NetworkVariable<ushort> length = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [CanBeNull]
    public static event System.Action<ushort> ChangedLengthEvent;

    private List<GameObject> _tails;
    private Transform _lastTail;
    private Collider2D _collider;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        _tails = new List<GameObject>();
        _lastTail = transform;

        _collider = GetComponent<Collider2D>();

        if (!IsServer) length.OnValueChanged += LenghtChangedEvent;
    }

    // this will be colled by the server
    [ContextMenu("Add Length")]
    public void AddLength()
    {
        length.Value += 1;
        LenghtChanged();
    }

    private void LenghtChanged()
    {
        InstantiateTail();

        if (!IsOwner) return;

        ChangedLengthEvent?.Invoke(length.Value);
    }

    private void LenghtChangedEvent(ushort previousValue, ushort newValue)
    {
        Debug.Log("LenghtChanged callback");
        LenghtChanged();
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
