using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Progreso del Jugador")] 
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private int currentNukes = 0;
    [SerializeField] private int currentShields = 0;
    [SerializeField] private int currentBerserkers = 0;

    [Header("UI HUD")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI nukesText;
    [SerializeField] private TextMeshProUGUI shieldsText;
    [SerializeField] private TextMeshProUGUI berserkersText;

    // Métodos para añadir ítems al inventario
    public void AddMoney(int amount)
    {
        if (!moneyText) return;
        
        currentMoney += amount;
        UpdateHUD();
    }

    public void AddScore(int amount)
    {
        if (!scoreText) return;
        
        currentScore += amount;
        UpdateHUD();
    }

    public void AddNuke(int amount)
    {
        if (!nukesText) return;
        
        currentNukes += amount;
        UpdateHUD();
    }

    public void AddShield(int amount)
    {
        if (!shieldsText) return;
        
        currentShields += amount;
        UpdateHUD();
    }

    public void AddBerserker(int amount)
    {
        if (!berserkersText) return;
        
        currentBerserkers += amount;
        UpdateHUD();
    }

    // Método para actualizar el HUD
    private void UpdateHUD()
    {
        if (moneyText != null) moneyText.text = $"{currentMoney}";
        if (scoreText != null) scoreText.text = $"{currentScore}";
        if (nukesText != null) nukesText.text = $"{currentNukes}";
        if (shieldsText != null) shieldsText.text = $"{currentShields}";
        if (berserkersText != null) berserkersText.text = $"{currentBerserkers}";
    }
}
