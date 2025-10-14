using UnityEngine;
using TMPro;
using System.Collections;

public class BoxClick : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject benchSetPrefab;

    [Header("Audio Settings")]
    public AudioClip transformSound;
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public Animator boxAnimator;

    [Header("Cost Settings")]
    public int unlockCost = 100;
    public TextMeshProUGUI costText; // TextMeshPro desteği

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

        // Para kontrolü
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
        Collider boxCollider = GetComponent<Collider>();
        Vector3 spawnPos = boxCollider.bounds.center;

        float groundY = boxCollider.bounds.min.y;
        Collider prefabCol = benchSetPrefab.GetComponent<Collider>();
        spawnPos.y = groundY + (prefabCol != null ? prefabCol.bounds.extents.y : 1f);

        Instantiate(benchSetPrefab, spawnPos, Quaternion.identity);

        // Kutuyu sil
        Destroy(gameObject);
    }
}