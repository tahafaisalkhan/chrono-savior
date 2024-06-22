using UnityEngine;
using UnityEngine.UI;

public class ShieldBarController : MonoBehaviour
{
    [SerializeField] private Image shieldBarImage;
    [SerializeField] private Sprite[] shieldBarSprites;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            UpdateShieldBar(player.GetCurrentShield());
        }
    }

    void UpdateShieldBar(float currentShield)
    {
        int shieldIndex = Mathf.FloorToInt(currentShield / (Player.MAX_SHIELD / 7f));
        if (shieldIndex < 0) shieldIndex = 0;
        if (shieldIndex > 6) shieldIndex = 6;
        shieldBarImage.sprite = shieldBarSprites[shieldIndex];
    }
}