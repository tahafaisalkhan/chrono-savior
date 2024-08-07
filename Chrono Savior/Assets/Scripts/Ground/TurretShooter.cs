using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPosition;
    [SerializeField] private GameObject active;
    [SerializeField] private GameObject idle;
    [SerializeField] private GameObject deathAnimation;
    private const float MAX_HEALTH = 50f;
    private bool isInfinite = false;

    private float currentHealth;
    private Player player;
    private float timer;
    private const float TIME_BETWEEN_SHOTS = 1.5f;
    private const float DETECT_DISTANCE = 5f;

    void Start()
    {
        player = Player.Instance;
        if (player == null)
        {
            Debug.LogError("Player instance is null in TurretShooter.");
        }

        currentHealth = MAX_HEALTH;

        if (idle != null)
        {
            idle.SetActive(true);
        }
        else
        {
            Debug.LogError("Idle is null in TurretShooter.");
        }

        if (active != null)
        {
            active.SetActive(false);
        }
        else
        {
            Debug.LogError("Active is null in TurretShooter.");
        }

        if (MainMenu.mode == MainMenu.Mode.Infinity)
        {
            isInfinite = true;
        }
    }

    void Update()
    {
        if (player == null || idle == null || active == null)
        {
            Debug.LogError("Player, idle, or active is null in TurretShooter.");
            return;
        }

        if (player.AreEnemiesFrozen())
        {
            return;
        }

        Vector2 distanceVec = player.transform.position - transform.position;
        float distanceSquared = distanceVec.sqrMagnitude;

        if (distanceSquared < (DETECT_DISTANCE * DETECT_DISTANCE))
        {
            idle.SetActive(false);
            active.SetActive(true);
            timer += Time.deltaTime;

            if (timer > TIME_BETWEEN_SHOTS)
            {
                Shoot();
                timer = 0;
            }
        }
        else
        {
            idle.SetActive(true);
            active.SetActive(false);
        }
    }

    void Shoot()
    {
        if (bullet == null || bulletPosition == null)
        {
            Debug.LogError("Bullet or bulletPosition is null in TurretShooter.");
            return;
        }

        Instantiate(bullet, bulletPosition.position, Quaternion.identity);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathAnimation == null)
        {
            Debug.LogError("Death animation is null in TurretShooter.");
            return;
        }

        if (!isInfinite && StoryManager.Instance != null)
        {
            StoryManager.Instance.DecreaseEnemyCount();
        }

        if (isInfinite && StateManagement.Instance != null)
        {
            StateManagement.Instance.SetGroundKillCount(StateManagement.Instance.GetGroundKillCount() + 1);
            Debug.Log(StateManagement.Instance.GetGroundKillCount());
        }

        Instantiate(deathAnimation, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
