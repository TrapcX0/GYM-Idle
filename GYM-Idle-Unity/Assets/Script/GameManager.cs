using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int money = 1000;
    public float startingMoney = 100f;
    public float currentMoney = 100f;

    [Header("UI References")]
    public UnityEngine.UI.Text moneyText; // Para UI'ı için

    // Singleton pattern
    public static GameManager Instance;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Başlangıç parası
        currentMoney = startingMoney;
        UpdateMoneyUI();
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        Debug.Log($"Para eklendi: +{amount}$ | Toplam: {currentMoney}$");

        UpdateMoneyUI();

        // Para efekti göster (opsiyonel)
        ShowMoneyGainEffect(amount);
    }

    public bool SpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"Para harcandı: -{amount}$ | Kalan: {currentMoney}$");

            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.Log($"Yetersiz para! Gereken: {amount}$, Mevcut: {currentMoney}$");
            return false;
        }
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
    }


    public float GetMoney()
    {
        return currentMoney;
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"${currentMoney:F0}";
        }
    }

    void ShowMoneyGainEffect(float amount)
    {
        // Basit para kazanma efekti - console'da göster
        Debug.Log($"💰 +${amount}");

        // Buraya UI efekti ekleyebilirsiniz (text animation vs.)
    }

    // Save/Load sistemi (basit)
    public void SaveGame()
    {
        PlayerPrefs.SetFloat("CurrentMoney", currentMoney);
        PlayerPrefs.Save();
        Debug.Log("Oyun kaydedildi!");
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("CurrentMoney"))
        {
            currentMoney = PlayerPrefs.GetFloat("CurrentMoney", startingMoney);
            UpdateMoneyUI();
            Debug.Log($"Oyun yüklendi! Para: {currentMoney}$");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame(); // Otomatik kayıt
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame(); // Otomatik kayıt
        }
    }
}