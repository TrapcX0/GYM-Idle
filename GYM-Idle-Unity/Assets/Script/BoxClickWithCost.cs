using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BoxClickWithCost : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject benchSetPrefab;
    public Transform spawnOffset; // spawn pozisyonu (isteğe bağlı)

    [Header("Audio Settings")]
    public AudioClip transformSound;
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public Animator boxAnimator;

    [Header("Cost Settings")]
    public int unlockCost = 100;
    public TextMeshProUGUI costText;

    private bool isTransformed = false;

    void Start()
    {
        // AudioSource kontrolü
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Fiyat yazısını göster
        if (costText != null)
            costText.text = unlockCost + " $";
    }

    void OnMouseDown()
    {
        if (isTransformed) return;

        if (GameManager.Instance != null && GameManager.Instance.money >= unlockCost)
        {
            GameManager.Instance.SpendMoney(unlockCost);
            StartCoroutine(TransformBox());
        }
        else
        {
            Debug.Log("Yetersiz bakiye!");
            if (costText != null)
                costText.text = "Yetersiz!";
        }
    }

    IEnumerator TransformBox()
    {
        isTransformed = true;

        // Animasyon
        if (boxAnimator != null)
            boxAnimator.SetTrigger("Open");

        // Ses
        if (transformSound != null)
            audioSource.PlayOneShot(transformSound);

        // Bekleme
        yield return new WaitForSeconds(0.6f);

        // Spawn pozisyonu hesapla
        Vector3 spawnPos;
        if (spawnOffset != null)
        {
            spawnPos = spawnOffset.position;
        }
        else
        {
            Collider boxCol = GetComponent<Collider>();
            spawnPos = boxCol.bounds.center;
            float groundY = boxCol.bounds.min.y;
            Collider prefabCol = benchSetPrefab.GetComponent<Collider>();
            spawnPos.y = groundY + (prefabCol != null ? prefabCol.bounds.extents.y : 0.5f);
        }

        // Prefab oluştur
        Instantiate(benchSetPrefab, spawnPos, Quaternion.identity);

        // Kutuyu sil
        Destroy(gameObject);
    }
}