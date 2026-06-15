using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    private float speedStep = 0.5f;
    private float maxSpeed = 5.0f;
    private float minSpeed = 0.2f;
    private float defaultSpeed = 1.0f;

    private void Awake()
    {
        // Ce log DOIT apparaître dès que le jeu se lance si l'objet est dans la scène
        Debug.Log("[Toolbox Time] Le script est EN VIE sur le GameObject !");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp)) DecreaseSpeed();
        if (Input.GetKeyDown(KeyCode.PageDown)) IncreaseSpeed();
        if (Input.GetKeyDown(KeyCode.End)) ResetSpeed();
    }

    private void IncreaseSpeed()
    {
        Time.timeScale = Mathf.Min(Time.timeScale + speedStep, maxSpeed);
        Debug.Log($"[Toolbox Time] Vitesse : {Time.timeScale}x");
    }

    private void DecreaseSpeed()
    {
        Time.timeScale = Mathf.Max(Time.timeScale - speedStep, minSpeed);
        Debug.Log($"[Toolbox Time] Vitesse : {Time.timeScale}x");
    }

    private void ResetSpeed()
    {
        Time.timeScale = defaultSpeed;
        Debug.Log("[Toolbox Time] Reset -> 1.0x");
    }
}