using UnityEngine;
using System.Collections;

public class BoxClick2 : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject benchSetPrefab; // Gym BenchSet prefabı

    [Header("Audio Settings")]
    public AudioClip transformSound; // mp3 ses dosyası
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public Animator boxAnimator; // kutu Animator

    private bool isTransformed = false;

    void Start()
    {
        // Eğer objede AudioSource yoksa otomatik ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void OnMouseDown()
    {
        if (!isTransformed)
        {
            StartCoroutine(TransformBox());
        }
    }

    IEnumerator TransformBox()
    {
        isTransformed = true;

        // 1. Animasyonu çalıştır
        if (boxAnimator != null)
            boxAnimator.SetTrigger("Open");

        // 2. Sesi çal
        if (transformSound != null)
            audioSource.PlayOneShot(transformSound);

        // 3. Animasyonun süresini bekle (ör: 0.6 saniye)
        yield return new WaitForSeconds(0.6f);

        // 4. BenchSet prefabını oluştur (zemine oturt)
        Collider boxCollider = GetComponent<Collider>();
        Vector3 spawnPos = boxCollider.bounds.center;

        float groundY = boxCollider.bounds.min.y; // kutunun alt yüzeyi
        Collider prefabCol = benchSetPrefab.GetComponent<Collider>();
        if (prefabCol != null)
        {
            spawnPos.y = groundY + prefabCol.bounds.extents.y;
        }
        else
        {
            spawnPos.y = groundY + 0.08f; // collider yoksa tahmini değer
        }

        GameObject spawned = Instantiate(benchSetPrefab, spawnPos, Quaternion.identity);

        // 5. Kutuyu sil
        Destroy(gameObject);
    }
}