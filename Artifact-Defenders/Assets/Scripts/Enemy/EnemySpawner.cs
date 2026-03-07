using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible of spawning enemies, including land units and boats.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // === CÁC BIẾN PREFAB ===
    [SerializeField] Transform wolfPrefab;
    [SerializeField] Transform wolfEaterPrefab;
    [SerializeField] Transform enemy00Prefab;
    [SerializeField] Transform enemy01Prefab;
    [SerializeField] Transform boatPrefab; // --- MỚI: Prefab thuyền ---
    [SerializeField] Transform BossPrefab;

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform[] waterSpawnPoints; // --- MỚI: Điểm sinh trên nước ---

    [Header("Rates")]
    [SerializeField] int eaterChance = 3;
    [SerializeField] int boatChance = 2;        // --- MỚI: Tỷ lệ sinh thuyền (ví dụ 2/10) ---
    [SerializeField] float spawnTime;
    [SerializeField] float spawnReductionPer;
    [SerializeField] float spawnFloor;
    [SerializeField] float bossSpawnTime = 20f;

    Manager gameManager;

    private Transform[] hardEnemies;
    private Transform[] commonEnemies;

    float currentSpawnTime;
    float timer;
    bool bossSpawned = false;

    void Start()
    {
        // Khởi tạo các nhóm kẻ thù
        hardEnemies = new Transform[] { wolfEaterPrefab, enemy00Prefab };
        commonEnemies = new Transform[] { wolfPrefab, enemy01Prefab };

        currentSpawnTime = spawnTime;
        timer = Time.time;

        gameManager = FindObjectOfType<Manager>();
    }

    void Update()
    {
        // Boss Logic
        if (!bossSpawned && gameManager != null && gameManager.GetTime() <= bossSpawnTime)
        {
            SpawnBoss();
            bossSpawned = true;
        }

        // Spawn Logic
        if (Time.time > timer)
        {
            Spawn();

            currentSpawnTime -= spawnReductionPer;
            if (currentSpawnTime <= spawnFloor) currentSpawnTime = spawnFloor;

            timer = Time.time + currentSpawnTime;
        }
    }

    void SpawnBoss()
    {
        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        Instantiate(BossPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("⚠️ BOSS SPAWNED!");
    }

    void Spawn()
    {
        int roll = Random.Range(0, 11);

        // 1. --- MỚI: Kiểm tra sinh thuyền ---
        // Nếu roll trúng tỷ lệ thuyền VÀ có thiết lập điểm sinh dưới nước
        if (roll <= boatChance && waterSpawnPoints.Length > 0)
        {
            Vector3 waterPos = waterSpawnPoints[Random.Range(0, waterSpawnPoints.Length)].position;
            Instantiate(boatPrefab, waterPos, Quaternion.identity);
            return; // Sinh thuyền xong thì thoát hàm để chờ đợt sau
        }

        // 2. Logic sinh quái vật trên cạn (giữ nguyên logic cũ)
        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        if (roll <= eaterChance)
        {
            Transform enemyToSpawn = hardEnemies[Random.Range(0, hardEnemies.Length)];
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Transform enemyToSpawn = commonEnemies[Random.Range(0, commonEnemies.Length)];
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}