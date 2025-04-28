using UnityEngine;
using System.Collections.Generic;

public class ChildSpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assigned in Inspector! was lazy

    private List<EnemyController> trackedEnemies = new List<EnemyController>();

    void Update()
    {
        // Clean up destroyed enemies
        trackedEnemies.RemoveAll(enemy => enemy == null);
    }

    public void RegisterEnemy(GameObject enemyObject)
    {
        EnemyController ec = enemyObject.GetComponent<EnemyController>();
        if (ec == null)
            return;

        SpriteRenderer sr = enemyObject.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        // Find matching EnemyData by sprite
        EnemyData myData = null;
        foreach (var enemy in GameData.Instance.GetAllEnemies())
        {
            if (GameManager.Instance.enemySpriteManager.Get(enemy.sprite) == sr.sprite)
            {
                myData = enemy;
                break;
            }
        }

        if (myData == null || string.IsNullOrEmpty(myData.child))
            return; // No child to spawn

        trackedEnemies.Add(ec);

        if (myData.childWhen == "hit")
        {
            ec.hp.OnHit += () => SpawnChildren(myData, enemyObject.transform.position);
        }
        else if (myData.childWhen == "death")
        {
            ec.hp.OnDeath += () => SpawnChildren(myData, enemyObject.transform.position);
        }
    }

    private void SpawnChildren(EnemyData parentData, Vector3 position)
    {
        for (int i = 0; i < parentData.childNum; i++)
        {
            SpawnChild(parentData.child, position);
        }
    }

    private void SpawnChild(string childName, Vector3 position)
    {
        Vector2 offset = Random.insideUnitCircle * 1.5f;
        Vector3 spawnPos = position + new Vector3(offset.x, offset.y, 0);

        GameObject child = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyData childData = GameData.Instance.GetEnemyByName(childName);
        if (childData != null)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = GameManager.Instance.enemySpriteManager.Get(childData.sprite);

            EnemyController ec = child.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.hp = new Hittable(childData.hp, Hittable.Team.MONSTERS, child);
                ec.speed = (int)childData.speed;
            }

            GameManager.Instance.AddEnemy(child);

            // Register child if it also has children
            RegisterEnemy(child);
        }
    }
}
