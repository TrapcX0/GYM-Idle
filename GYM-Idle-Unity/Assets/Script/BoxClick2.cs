using UnityEngine;
using TMPro;
using System.Collections;

public class BoxClick2 : MonoBehaviour
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
    public TextMeshProUGUI costText;

    private bool isTransformed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (costText != null)
            costText.text = unlockCost + " $";
    }

    void OnMouseDown()
    {
        if (isTransformed) return;

        // Güçlü para kontrolü
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager bulunamadı!");
            return;
        }

        if (GameManager.Instance.money < unlockCost)
        {
            Debug.Log("Yetersiz bakiye!");
            if (costText != null)
                costText.text = "Yetersiz!";
            return; // ❌ Kutu açılmasın
        }

        // ✅ Para yeterli → aç
        GameManager.Instance.SpendMoney(unlockCost);
        StartCoroutine(TransformBox());
    }

    IEnumerator TransformBox()
    {
        isTransformed = true;

        if (boxAnimator != null)
            boxAnimator.SetTrigger("Open");

        if (transformSound != null)
            audioSource.PlayOneShot(transformSound);

        yield return new WaitForSeconds(0.6f);

        Collider boxCollider = GetComponent<Collider>();
        Vector3 spawnPos = boxCollider.bounds.center;

        float groundY = boxCollider.bounds.min.y;
        Collider prefabCol = benchSetPrefab.GetComponent<Collider>();
        spawnPos.y = groundY + (prefabCol != null ? prefabCol.bounds.extents.y : 0.05f);

        Instantiate(benchSetPrefab, spawnPos, Quaternion.identity);

        Destroy(gameObject);
    }
}