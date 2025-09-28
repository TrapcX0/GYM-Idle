using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleCustomerAI : MonoBehaviour
{
    public float membershipFee = 15f;       // giriş ücreti
    public float workoutTime = 10f;         // alette çalışma süresi
    public float moneyPerSecond = 2f;       // saniye başına kazanılacak para
    public Transform[] workoutMachines;     // Inspector’dan aletleri sürükle bırak

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMesh Agent component bulunamadı!");
            return;
        }

        StartCoroutine(CustomerRoutine());
    }

    IEnumerator CustomerRoutine()
    {
        // 1. Merkeze git
        yield return MoveToPoint(new Vector3(0, 0, 0), 2f);

        // 2. Para öde
        PayMoney();

        // 3. Sonsuz döngü: sürekli alet seç, git, çalış, para kazandır
        while (true)
        {
            // Rastgele bir alet seç
            Transform machine = workoutMachines[Random.Range(0, workoutMachines.Length)];

            // Aletin yanına git
            yield return MoveToPoint(machine.position, 2f);

            // Workout başlasın
            Debug.Log($"{gameObject.name} {machine.name} aletinde çalışmaya başladı!");
            float elapsed = 0f;

            while (elapsed < workoutTime)
            {
                elapsed += Time.deltaTime;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddMoney(moneyPerSecond * Time.deltaTime);
                }

                yield return null;
            }

            Debug.Log($"{gameObject.name} {machine.name} aletinde çalışmayı bitirdi!");
        }
    }

    void PayMoney()
    {
        Debug.Log("Para ödendi: " + membershipFee + " dolar");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(membershipFee);
        }
    }

    // Hedefe gitme fonksiyonu
    IEnumerator MoveToPoint(Vector3 target, float waitTime)
    {
        agent.SetDestination(target);

        // Hedefe varana kadar bekle
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        yield return new WaitForSeconds(waitTime);
    }

    // NavMesh üzerinde geçerli nokta bulma (gerekirse)
    Vector3 GetRandomNavMeshPoint(Vector3 center, float range)
    {
        Vector3 randomPos = center + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center; // fallback
    }
}