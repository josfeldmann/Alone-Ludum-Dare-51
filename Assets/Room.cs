using System.Collections.Generic;
using UnityEngine;

public enum RoomType { NORMAL, PLAYERSTART, KEY, VAULT }


public class Room : MonoBehaviour {

    public RoomType ROOMTYPE = RoomType.NORMAL;


    public List<Transform> obstaclePoints = new List<Transform>();
    public List<Transform> enemyPoints = new List<Transform>();
    public List<Transform> foodPoints = new List<Transform>();

    public void GenerateRoom( WorldGenerator gen ) {

        List<EditorSpot> spots = new List<EditorSpot>(GetComponentsInChildren<EditorSpot>());

        obstaclePoints = new List<Transform>();
        enemyPoints = new List<Transform>();
        foodPoints = new List<Transform>();

       foreach (EditorSpot e in spots) {
            switch (e.spotType) {
                case SpotType.ENEMY:
                    enemyPoints.Add(e.transform);
                    break;
                case SpotType.OBSTACLE:
                    obstaclePoints.Add(e.transform);
                    break;
                case SpotType.FOOD:
                    foodPoints.Add(e.transform);
                    break;
            }
        }

        obstaclePoints.Shuffle();


        if (ROOMTYPE == RoomType.PLAYERSTART) {
            Transform t = obstaclePoints[0];
            obstaclePoints.RemoveAt(0);
            gen.MovePlayerToPoint(t.position);
        } else if (ROOMTYPE == RoomType.VAULT) {
            Transform t = obstaclePoints[0];
            obstaclePoints.RemoveAt(0);
            GameMasterManager.instance.vault.transform.position = t.position;
        } else if (ROOMTYPE == RoomType.KEY) {
            Transform t = obstaclePoints[0];
            obstaclePoints.RemoveAt(0);
            GameMasterManager.instance.key.transform.position = t.position;
        } else {
            int numberOfholes = Random.Range(gen.holeRange.x, gen.holeRange.y + 1);
            while (numberOfholes > 0 && obstaclePoints.Count > 0) {
                Transform t = obstaclePoints[0];
                obstaclePoints.RemoveAt(0);
                Instantiate(GameMasterManager.instance.worldGenerator.holeGameObject, t.position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), transform);
                numberOfholes--;
            }
            
        }



        foreach (Transform t in obstaclePoints) {
            GameObject g = gen.obstacles.PickRandom();
            Instantiate(g, t.position, Quaternion.Euler(0,0,Random.Range(0f,360f)), transform);
        }


        if (ROOMTYPE != RoomType.PLAYERSTART) {
            enemyPoints.Shuffle();
            for (int i = 0; i < gen.numberOfEnemiesPerRoom; i++) {
                Transform t = enemyPoints[0];
                Enemy e = Instantiate(gen.enemyPrefab, t.position, t.rotation, transform);
                e.RandomizeColors();
                e.looker.localScale = transform.localScale;
                enemyPoints.RemoveAt(0);
                if (enemyPoints.Count == 0) i = gen.numberOfEnemiesPerRoom;
            }
        }

        foodPoints.Shuffle();
        for (int i = 0; i < gen.numberOfFoodPerRoom; i++) {
            Transform t = foodPoints[0];
            Food e = Instantiate(gen.foodPrefabs.PickRandom(), t.position, t.rotation, transform);
            e.transform.localScale = transform.localScale;
            foodPoints.RemoveAt(0);
            if (foodPoints.Count == 0) i = gen.numberOfEnemiesPerRoom;
        }




    }



}
