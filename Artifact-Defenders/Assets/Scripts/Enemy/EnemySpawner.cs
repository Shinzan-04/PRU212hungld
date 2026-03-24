using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Transform wolfPrefab;
    [SerializeField] Transform wolfEaterPrefab;
    [SerializeField] Transform enemy00Prefab;
    [SerializeField] Transform enemy01Prefab;
    [SerializeField] Transform boatPrefab;
    [SerializeField] Transform BossPrefab;

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform[] waterSpawnPoints;

    [Header("Rates")]
    [SerializeField] int eaterChance = 3;
    [SerializeField] int boatChance = 2;
    [SerializeField] float spawnTime = 5f;
    [SerializeField] float spawnReductionPer = 0.1f;
    [SerializeField] float spawnFloor = 1f;
    [SerializeField] float bossSpawnTime = 20f;

    Manager gameManager;
    private Transform[] hardEnemies;
    private Transform[] commonEnemies;

    float currentSpawnTime;
    float timer;
    bool bossSpawned = false;

    void Start()
    {
        // Khởi tạo mảng quái vật
        hardEnemies = new Transform[] { wolfEaterPrefab, enemy00Prefab };
        commonEnemies = new Transform[] { wolfPrefab, enemy01Prefab };

        currentSpawnTime = spawnTime;
        timer = Time.time + currentSpawnTime;

        gameManager = FindObjectOfType<Manager>();
    }

    void Update()
    {
        if (gameManager == null) return;

        // 1. Logic sinh Boss
        if (!bossSpawned && gameManager.GetTime() <= bossSpawnTime)
        {
            SpawnBoss();
            bossSpawned = true;
        }

        // 2. Logic sinh quái thường
        if (Time.time > timer)
        {
            Spawn();
            // Giảm dần thời gian chờ giữa các đợt spawn
            currentSpawnTime = Mathf.Max(spawnFloor, currentSpawnTime - spawnReductionPer);
            timer = Time.time + currentSpawnTime;
        }
    }

    void SpawnBoss()
    {
        if (BossPrefab == null) return;

        // Kiểm tra an toàn trước khi lấy vị trí
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (sp != null) Instantiate(BossPrefab, sp.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Chưa gán spawnPoints cho Boss!");
        }
    }

    void Spawn()
    {
        int roll = Random.Range(0, 11);

        // --- KIỂM TRA SINH THUYỀN (WATER) ---
        if (roll <= boatChance && boatPrefab != null)
        {
            if (waterSpawnPoints != null && waterSpawnPoints.Length > 0)
            {
                Transform sp = waterSpawnPoints[Random.Range(0, waterSpawnPoints.Length)];
                if (sp != null)
                {
                    Instantiate(boatPrefab, sp.position, Quaternion.identity);
                    return; // Sinh thuyền xong thì đợi đợt sau
                }
            }
        }

        // --- KIỂM TRA SINH QUÁI TRÊN CẠN (LAND) ---
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (sp == null) return;

            if (roll <= eaterChance)
            {
                if (hardEnemies.Length > 0)
                {
                    Transform enemy = hardEnemies[Random.Range(0, hardEnemies.Length)];
                    if (enemy != null) Instantiate(enemy, sp.position, Quaternion.identity);
                }
            }
            else
            {
                if (commonEnemies.Length > 0)
                {
                    Transform enemy = commonEnemies[Random.Range(0, commonEnemies.Length)];
                    if (enemy != null) Instantiate(enemy, sp.position, Quaternion.identity);
                }
            }
        }
        else
        {
            Debug.LogWarning("Chưa gán spawnPoints cho quái thường!");
        }
    }
}