using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic; // ✅ Liste için eklendi

public class SimpleCustomerAI : MonoBehaviour
{
    public float membershipFee = 15f;       // giriş ücreti
    public float workoutTime = 3f;         // alette çalışma süresi

    private NavMeshAgent agent;
    private Animator animator;

    // ✅ Kullanılan aletleri hatırlamak için liste
    private List<Transform> usedMachines = new List<Transform>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMesh Agent component bulunamadi!");
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
                Debug.Log($"{gameObject.name} artık çalışacak alet bulamadı, salonu terk ediyor!");
                // ✅ Çıkış noktasına git (örnek: salonun kapısı)
                yield return MoveToPoint(new Vector3(0, 0, 25f), 0f);
                
                yield return MoveToPoint(new Vector3(20f, 0, 21f), 0f);

                 Destroy(gameObject);
               
                yield break; // coroutine’i bitir
            }

            // Aletin yanına git
            yield return MoveToPoint(machine.position, 0f);

            // Workout başlasın
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isWorkingOut", true);

            Debug.Log($"{gameObject.name} {machine.name} aletinde çalışmaya başladı!");

            // ✅ Çalışma süresi kadar bekle
            yield return new WaitForSeconds(workoutTime);

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

    // ✅ Tag ile sahnedeki tüm makinelerden rastgele seç (kullanılmamış olanlardan)
    Transform GetRandomMachine()
    {
        GameObject[] machines = GameObject.FindGameObjectsWithTag("Machine");
        List<Transform> available = new List<Transform>();

        foreach (GameObject m in machines)
        {
            if (!usedMachines.Contains(m.transform))
                available.Add(m.transform);
        }

        if (available.Count == 0) return null;

        Transform chosen = available[Random.Range(0, available.Count)].transform;
        usedMachines.Add(chosen); // ✅ Seçilen aleti kaydet
        return chosen;
    }
}