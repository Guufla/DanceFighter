using UnityEngine;
using UnityEngine.UI; 

public class SuperBarController : MonoBehaviour
{
    public Slider superbar; // slider 
    public float chargeSpeed = 0.3f; // how fast it charges

    public float maxCharge = 10f; // charge value
    private float currentCharge = 0; // current charge

    // Start is called before the first frame update
    void Start()
    {
        if (superbar == null)
        {
            superbar = GetComponent<Slider>();
        }

        superbar.maxValue = maxCharge;
        superbar.value = currentCharge;
    }

    // not needed
     /* void Update()
    {
        if (currentCharge < maxCharge)
        {
            ChargeSuperBar(Time.deltaTime * chargeSpeed);
        }
    }
    */
    public void ChargeSuperBar(float amount)
    {
        currentCharge = Mathf.Clamp(currentCharge + amount, 0 , maxCharge);
        superbar.value = currentCharge;
    }

    public void UseSuperMove()
    {
        if (currentCharge >= maxCharge)
        {
            Debug.Log("Super Move Activated!");
            currentCharge = 0; // Reset bar after using the move
            superbar.value = currentCharge;
        }
    }
}
