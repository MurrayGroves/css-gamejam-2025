using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public readonly List<PlayerLevelManager> allPlayers = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterPlayerLevelManager(PlayerLevelManager playerLevelManager)
    {
        allPlayers.Add(playerLevelManager);

        Debug.Log($"[GameManager] Registered player {allPlayers.Count}: {playerLevelManager.gameObject.name}");
        if (playerLevelManager.powerupDisplay != null)
        {
            Debug.Log(
                $"[GameManager]   -> PowerUp Display: {playerLevelManager.powerupDisplay.gameObject.name} (InstanceID: {playerLevelManager.powerupDisplay.GetInstanceID()})");
            Debug.Log(
                $"[GameManager]   -> Display Path: {GetGameObjectPath(playerLevelManager.powerupDisplay.gameObject)}");
        }
        else
        {
            Debug.LogWarning("[GameManager]   -> PowerUp Display is NULL!");
        }
    }

    public void NotifyPowerUpActivated(string powerUpName, PlayerLevelManager source)
    {
        var players = allPlayers;

        var target = players
            .FirstOrDefault(plm => plm != null && plm != source);

        // Fallback if something weird happens
        if (source == null || target == null)
        {
            Debug.LogWarning("Source or target player is null in NotifyPowerUpActivated.");
            return;
        }


        var attackerSide = source.powerupDisplay;
        var targetSide = target.powerupDisplay;

        // Show messages on the correct halves
        var attackerMsg = $"You used {powerUpName}!";
        var targetMsg = $"{powerUpName} from other player!";

        ShowPowerUpMessage(attackerSide, attackerMsg);
        ShowPowerUpMessage(targetSide, targetMsg);
    }

    private void ShowPowerUpMessage(TMP_Text text, string message)
    {
        if (text == null)
        {
            Debug.LogError("PowerUp text is null!");
            return;
        }

        Debug.Log(
            $"[PowerUpUI] Showing '{message}' on GameObject: {text.gameObject.name} (InstanceID: {text.GetInstanceID()})");
        Debug.Log($"[PowerUpUI] Full path: {GetGameObjectPath(text.gameObject)}");
        Debug.Log($"[PowerUpUI] Canvas: {text.canvas?.name ?? "null"}, Active: {text.gameObject.activeSelf}");

        text.gameObject.transform.parent.gameObject.SetActive(true);
        text.SetText(message);
        text.color = Color.white;
        text.fontSize = 36;

        Debug.Log($"[PowerUpUI] Final text set: '{text.text}'");

        // Hide the message after 2 seconds
        StartCoroutine(HidePowerUpMessageAfterDelay(text, 2f));
    }

    private string GetGameObjectPath(GameObject obj)
    {
        var path = obj.name;
        var current = obj.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private IEnumerator HidePowerUpMessageAfterDelay(TMP_Text text, float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePowerUpMessages(text);
    }

    private void HidePowerUpMessages(TMP_Text text)
    {
        text.gameObject.transform.parent.gameObject.SetActive(false);
    }
}