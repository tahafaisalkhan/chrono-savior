using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class OnPowerupInteract : MonoBehaviour
{
    [SerializeField] private PowerUp powerup;
    [SerializeField] private float messageDuration = 4f;

    private Text messageText;
    private const string DAMAGE_POWERUP = "DamagePowerUp";
    private const string HEALTH_POWERUP = "HealthPowerUp";
    private const string ONESHOT_POWERUP = "OneShotPowerUp";
    private const string REMOVECOOLDOWN_POWERUP = "RemoveCooldownPowerUp";
    private const string REPLENISHCAP_POWERUP = "ReplenishCapPowerUp";
    private const string SHIELD_POWERUP = "ShieldPowerUp";
    private const string SPEED_POWERUP = "SpeedPowerUp";

    private void Start()
    {
        GameObject messageObject = GameObject.FindGameObjectWithTag("PowerUpMessage");
        if (messageObject != null)
        {
            messageText = messageObject.GetComponent<Text>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            powerup.UsePowerUp(other.gameObject);
            string powerUpName = FormatPowerUpName(gameObject.name.Replace("(Clone)", "").Trim());
            ShowMessage(powerUpName + " Activated");

            if (PowerupPoolingAPI.SharedInstance != null)
            {
                int index = -1;
                string name = powerup.name;
                if (name == DAMAGE_POWERUP) index = 0;
                else if (name == HEALTH_POWERUP) index = 1;
                else if (name == ONESHOT_POWERUP) index = 2;
                else if (name == REMOVECOOLDOWN_POWERUP) index = 3;
                else if (name == REPLENISHCAP_POWERUP) index = 4;
                else if (name == SHIELD_POWERUP) index = 5;
                else if (name == SPEED_POWERUP) index = 6;

                PowerupPoolingAPI.SharedInstance.ReleasePowerup(this, index);
            }
            else
            {
                Debug.LogError("PowerupPoolingAPI is null in OnPowerupInteract");
            }
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            CoroutineManager.Instance.StartCor(DisplayMessage(message));
        }
    }

    private IEnumerator DisplayMessage(string message)
    {
        messageText.text = message;
        messageText.enabled = true;
        yield return new WaitForSeconds(messageDuration);
        messageText.enabled = false;
    }

    private string FormatPowerUpName(string name)
    {
        string formattedName = Regex.Replace(name, "(\\B[A-Z])", " $1");
        return formattedName.Replace("Power Up", "").Trim();
    }
}
