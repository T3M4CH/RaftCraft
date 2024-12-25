using System.Collections.Generic;
using Game.Scripts.Extension;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Raft;
using Game.Scripts.Raft.Components;
using Game.Scripts.Raft.Interface;
using UnityEngine;

public class NPCNavigation
{
    private readonly IPlayerService _playerService;
    private Dictionary<int, List<LadderObject>> _dataLadder = new Dictionary<int, List<LadderObject>>();
    private Dictionary<int, List<TileEntity>> _dataDoors = new Dictionary<int, List<TileEntity>>();
    private GrabPoint[] _grabPoints;
    
    private const float _heightStep = 5f; 
    
    public EntityPlayer Player { get; private set; }
    public IRaftStructures RaftConstructor { get; private set; }

    public NPCNavigation(IRaftStructures raftStruct, IPlayerService playerService)
    {
        _playerService = playerService;
        RaftConstructor = raftStruct;
        
        _playerService.AddListener(OnSpawnPlayer);

        _grabPoints = RaftConstructor.GetGrabPoints();
        
        var ladders = RaftConstructor.GetStairs();
        var doors = RaftConstructor.GetDoors();
        
        foreach (var ladder in ladders)
        {
            var floorNumber = Mathf.FloorToInt(ladder.StartPoint.position.y / 5f);

            if (_dataLadder.ContainsKey(floorNumber) == false)
            {
                _dataLadder.Add(floorNumber, new List<LadderObject>());
            }

            _dataLadder[floorNumber].Add(ladder);
        }
        
        foreach (var door in doors)
        {
            var floorNumber = Mathf.FloorToInt(door.Position.y / 5f);

            if (_dataDoors.ContainsKey(floorNumber) == false)
            {
                _dataDoors.Add(floorNumber, new List<TileEntity>());
            }

            _dataDoors[floorNumber].Add(door);
        }
    }
    
    private void OnSpawnPlayer(EPlayerStates state, EntityPlayer player)
    {
        if (state == EPlayerStates.SpawnPlayer)
        {
            Player = player;
        }
    }

    public (Transform target, bool isClimb) GetRoute(Vector3 sourcePosition)
    {
        var sourceFloor = Mathf.Max(Mathf.FloorToInt(sourcePosition.y / _heightStep), 0);
        var playerFloor = Mathf.FloorToInt(Player.Position.y / _heightStep);
        
        //to player
        if (sourceFloor == playerFloor)
        {
            var door = DoorCheck(sourceFloor, sourcePosition);
            
            return (door ? door : Player.transform, false);
        }

        var targetFloor = sourceFloor < playerFloor ? _dataLadder[sourceFloor] : _dataLadder[Mathf.Max(sourceFloor - 1, 0)];
        var nearestLadder = GetNearestLadder(targetFloor, Player.Position);
        var differentHorizontal = Mathf.Abs(nearestLadder.StartPoint.position.x - sourcePosition.x) > 0.2f;
        
        //to ladder
        if(differentHorizontal)
        {
            var door = DoorCheck(sourceFloor, sourcePosition);
            
            return sourceFloor < playerFloor
                ? (door ? door : nearestLadder.StartPoint, false)
                : (door ? door : nearestLadder.EndPoint, false);
        }

        //climb
        return sourceFloor < playerFloor
            ? (nearestLadder.EndPoint, true)
            : (nearestLadder.StartPoint, true);
    }
    
    public GrabPoint GetGrabPoint(bool isLeft)
    {
        foreach (var grabPoint in _grabPoints)
        {
            if (grabPoint.IsLeft == isLeft)
            {
                return grabPoint;
            }
        }
        
        return null;
    }

    private Transform DoorCheck(int floorNumber, Vector3 source)
    {
        if (_dataDoors.ContainsKey(floorNumber) == false) return null;
        
        foreach (var door in _dataDoors[floorNumber])
        {
            if(door.State == TileEntity.TileState.Dead) continue;
            
            if (door.Position.x > source.x && door.Position.x < Player.Position.x)
            {
                return door.transform;
            }
            
            if (door.Position.x < source.x && door.Position.x > Player.Position.x)
            {
                return door.transform;
            }
        }

        return null;
    }
    
    private LadderObject GetNearestLadder(List<LadderObject> ladders, Vector3 from)
    {
        var minDistance = 100f;
        var result = ladders.Peek();
        foreach (var ladder in ladders)
        {
            if (ladder.HaveInteraction == false)
            {
                continue;
            }
            var distance = Vector3.Distance(ladder.transform.position, from);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = ladder;
            }
        }

        return result;
    }
    
    public void Destroy()
    {
        _playerService.RemoveListener(OnSpawnPlayer);
    }
}
