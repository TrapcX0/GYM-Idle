using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnPoint; // Karakterlerin çıkacağı nokta
    public GameObject[] customerPrefabs; // Karakter prefabları
    public float spawnInterval = 5f; // Spawn aralığı (saniye)
    public int maxActiveCustomers = 10; // Aynı anda sahnede bulunacak maksimum müşteri

    private List<GameObject> activeCustomers = new List<GameObject>();

    void Start()
    {
        // Güvenlik kontrolleri
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point atanmadı! Inspector'dan ayarlayın.");
            return;
        }

        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("Customer prefab listesi boş! En az 1 prefab ekleyin.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true) // Sonsuz döngü
        {
            CleanupDestroyedCustomers(); // Silinenleri listeden çıkar

            if (activeCustomers.Count < maxActiveCustomers)
            {
                GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
                if (prefab != null)
                {
                    GameObject newCustomer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                    activeCustomers.Add(newCustomer);
                }
                else
                {
                    Debug.LogWarning("Seçilen prefab null! Prefab listesinde eksik olabilir.");
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void CleanupDestroyedCustomers()
    {
        // Sahneden silinmiş objeleri listeden çıkar
        activeCustomers.RemoveAll(c => c == null);
    }
}