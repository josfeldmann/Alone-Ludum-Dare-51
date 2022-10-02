using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    
    public List<Room> roomPrefab;
    public Vector2Int size = new Vector2Int(3, 3);
    public float roomWidth = 0, roomHeight;
    public Camera cam;
    public BoxCollider2D confiner;
    public BoxCollider2D leftWall, rightWall, upwall, downWall;
    public List<GameObject> obstacles = new List<GameObject>();
    public List<Food> foodPrefabs = new List<Food>();
    public Enemy enemyPrefab;

    public int numberOfEnemiesPerRoom = 1;
    public int numberOfFoodPerRoom = 1;



    public Room[,] roomGrid;

    public void ClearRooms() {
        if (roomGrid == null) roomGrid = new Room[size.x, size.y];
        foreach (Room r in roomGrid) {
            if (r != null)Destroy(r.gameObject);
        }
        roomGrid = new Room[size.x, size.y];


    }


    public void GenerateRooms() {

        ClearRooms();
        List<Room> rooms = new List<Room>(roomPrefab);
        rooms.Shuffle();

        roomGrid = new Room[size.x, size.y];

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Room r = Instantiate(rooms[0], transform);
                r.transform.localPosition = new Vector3(x * roomWidth, y * roomHeight, 0);
                roomGrid[x, y] = r;
                rooms.RemoveAt(0);
                if (rooms.Count == 0) {
                    rooms = new List<Room>(roomPrefab);
                    rooms.Shuffle();
                }
            }
        }

        Vector2Int playerPos = new Vector2Int(UnityEngine.Random.Range(0, size.x), 0);
        Vector2Int keyPos = new Vector2Int(UnityEngine.Random.Range(0, size.x), size.y - 2);
        Vector2Int vaultPos = new Vector2Int(UnityEngine.Random.Range(0, size.x), size.y - 1);

        roomGrid[playerPos.x, playerPos.y].ROOMTYPE = RoomType.PLAYERSTART;
        roomGrid[keyPos.x, keyPos.y].ROOMTYPE = RoomType.KEY;
        roomGrid[vaultPos.x, vaultPos.y].ROOMTYPE = RoomType.VAULT;



        foreach (Room r in roomGrid) {
            r.GenerateRoom(this);
        }


        float mapWidth = roomWidth * size.x;
        float mapHeight = roomHeight * size.y;

        float hOff = roomHeight / 2;
        float wOff = roomWidth / 2;
        float camOffy = cam.orthographicSize;
        float camOffx = cam.orthographicSize * cam.aspect;

        upwall.size = new Vector3(mapWidth, 1, 0);
        upwall.transform.position = new Vector3((mapWidth / 2) - wOff, mapHeight - hOff, 0);
        downWall.size = new Vector3(mapWidth, 1, 0);
        downWall.transform.position = new Vector3((mapWidth / 2) - wOff, 0 - hOff, 0);
        rightWall.size = new Vector3(1, mapHeight, 0);
        rightWall.transform.position = new Vector3(mapWidth - (wOff), (mapHeight/2) - (hOff), 0);
        leftWall.size = new Vector3(1, mapHeight, 0);
        leftWall.transform.position = new Vector3(0 - wOff, (mapHeight / 2) - (hOff), 0);
        confiner.transform.position = new Vector3((mapWidth / 2) - (wOff), (mapHeight / 2) - hOff, 0);
        confiner.size = new Vector2(mapWidth - (camOffx * 2), mapHeight - (camOffy * 2));






    }

    internal void MovePlayerToPoint(Vector3 position) {
        GameMasterManager.instance.player.transform.position = position;
        GameMasterManager.instance.MoveCameraTo(position);
    }
}
