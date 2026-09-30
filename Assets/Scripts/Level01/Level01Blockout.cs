using UnityEngine;
using UnityEngine.InputSystem;

// Minimal quest interactions for a disposable layout test scene.
public sealed class Level01Blockout : MonoBehaviour
{
    public PlayerHealth player;
    public Transform caregiver;
    public Transform chest;
    public Transform remedy;
    public GameObject shortcutGate;
    public EnemyHealth storeGuard;
    public EnemyHealth[] infirmaryEnemies;
    public Camera viewCamera;
    private bool accepted;
    private bool hasRemedy;
    private bool complete;
    private bool openedChest;
    private bool openedGate;
    private bool overview;
    private string feedback;
    private float feedbackUntil;
    private string prompt;

    private void Update()
    {
        if (player == null || player.health <= 0 || Time.timeScale <= 0) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.mKey.wasPressedThisFrame) overview = !overview;
        bool interact = keyboard.eKey.wasPressedThisFrame;
        prompt = "";
        if (Near(caregiver, 2.2f))
        {
            prompt = complete ? "The caregiver has the remedy." : "E — Speak to caregiver";
            if (interact && !complete)
            {
                if (hasRemedy) { complete = true; Say("Expedition complete. The remedy buys us time."); }
                else { accepted = true; Say("Find a remedy beyond the infirmary. Return safely."); }
            }
        }
        else if (!openedChest && Near(chest, 2.2f))
        {
            prompt = "E — Open supply chest";
            if (interact)
            {
                if (storeGuard != null && storeGuard.health > 0) Say("Defeat the store's guardian first.");
                else
                {
                    openedChest = true;
                    player.GetComponent<PlayerAttack>().damage += 1;
                    chest.GetComponent<SpriteRenderer>().color = Color.gray;
                    Say("Test weapon equipped: +1 base damage. Inventory comes later.");
                }
            }
        }
        else if (!hasRemedy && Near(remedy, 2.2f))
        {
            prompt = "E — Recover remedy and read note";
            if (interact)
            {
                bool guarded = false;
                foreach (var enemy in infirmaryEnemies) guarded |= enemy != null && enemy.health > 0;
                if (!accepted) Say("Speak to the caregiver in the refuge first.");
                else if (guarded) Say("The infirmary's guardians must be defeated first.");
                else
                {
                    hasRemedy = true;
                    remedy.gameObject.SetActive(false);
                    Say("Remedy recovered. Note: 'The oracle's warning has come to pass.'");
                }
            }
        }
        else if (!openedGate && shortcutGate != null && Near(shortcutGate.transform, 3f))
        {
            bool inside = player.transform.position.x > shortcutGate.transform.position.x;
            prompt = inside ? "E — Unbolt return passage" : "The door is bolted from the other side.";
            if (interact && inside)
            {
                openedGate = true;
                shortcutGate.SetActive(false);
                Say("Shortcut open. Follow this passage west, then south to the refuge.");
            }
        }
    }

    private bool Near(Transform target, float radius) => target != null &&
        target.gameObject.activeSelf && Vector2.Distance(player.transform.position, target.position) <= radius;

    private void Say(string text) { feedback = text; feedbackUntil = Time.unscaledTime + 6f; }

    private void LateUpdate()
    {
        if (viewCamera == null || player == null) return;
        Vector3 target = overview ? new Vector3(22, 29, -10) : player.transform.position + new Vector3(0, 0, -10);
        viewCamera.transform.position = target;
        // Overview fits the 46x60 footprint across landscape and portrait windows.
        viewCamera.orthographicSize = overview ? Mathf.Max(32, 25 / viewCamera.aspect) : 8;
    }

    private void OnGUI()
    {
        if (player == null) return;
        if (Time.timeScale <= 0f && player.health > 0) return;
        string objective = complete ? "Expedition complete" : !accepted ? "Speak to the caregiver" :
            !hasRemedy ? "Recover the remedy beyond the infirmary" : "Return to the caregiver";
        GUI.Box(new Rect(16, 16, 510, 94), "CENDRELITH — LEVEL 01 BLOCKOUT\n" + objective +
            $"\nHP {player.health}/{player.maxHealth} | Base damage {player.GetComponent<PlayerAttack>().damage}");
        GUI.Box(new Rect(16, 118, 510, 66), "WASD / arrows: move    Space: attack    E: interact\nM: overview map    Esc: pause");
        if (!string.IsNullOrEmpty(prompt) && player.health > 0)
            GUI.Box(new Rect(Screen.width / 2f - 250, Screen.height - 72, 500, 42), prompt);
        if (Time.unscaledTime < feedbackUntil)
            GUI.Box(new Rect(Screen.width / 2f - 300, 195, 600, 64), feedback);
        if (player.health <= 0)
        {
            GUI.Box(new Rect(Screen.width / 2f - 160, Screen.height / 2f - 65, 320, 130), "YOU DIED");
            if (GUI.Button(new Rect(Screen.width / 2f - 100, Screen.height / 2f, 200, 45), "Restart blockout"))
                FindFirstObjectByType<GameManager>().RestartGame();
        }
    }
}
