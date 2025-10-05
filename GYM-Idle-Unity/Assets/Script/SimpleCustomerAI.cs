using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleCustomerAI : MonoBehaviour
{
    public float membershipFee = 15f;       // giriş ücreti
    public float workoutTime = 10f;         // alette çalışma süresi
    public float moneyPerSecond = 2f;       // saniye başına kazanılacak para

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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
        yield return MoveToPoint(Vector3.zero, 2f);

        // 2. Para öde
        PayMoney();

        // 3. Sonsuz döngü: sürekli alet seç, git, çalış, para kazandır
        while (true)
        {
            Transform machine = GetRandomMachine();
            if (machine == null)
            {
                Debug.LogWarning("Hiç ekipman bulunamadı!");
                yield return new WaitForSeconds(2f);
                continue;
            }

            // Aletin yanına git
            yield return MoveToPoint(machine.position, 0f);

            // Workout başlasın
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isWorkingOut", true);

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

            // Workout bitti
            animator.SetBool("isWorkingOut", false);
            agent.isStopped = false;

            Debug.Log($"{gameObject.name} {machine.name} aletinde çalışmayı bitirdi!");
        }
    }

    void PayMoney()
    {
        Debug.Log("Para ödendi: " + membershipFee + " dolar");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(membershipFee); 
            // Eğer mantığın "müşteri para ödüyor, kasadan düşüyor" ise burayı SpendMoney() yapabilirsin
        }
    }

    // Hedefe gitme fonksiyonu
    IEnumerator MoveToPoint(Vector3 target, float waitTime)
    {
        agent.isStopped = false;
        agent.SetDestination(target);
        animator.SetBool("isWalking", true);

        // Hedefe varana kadar bekle
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitTime);
    }

    // Tag ile sahnedeki tüm makinelerden rastgele seç
    Transform GetRandomMachine()
    {
        GameObject[] machines = GameObject.FindGameObjectsWithTag("Machine");
        if (machines.Length == 0) return null;

        return machines[Random.Range(0, machines.Length)].transform;
    }
}