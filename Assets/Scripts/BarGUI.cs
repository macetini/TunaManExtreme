using UnityEngine;
using UnityEngine.UI;

public class BarGUI : MonoBehaviour
{
    public HeroController heroController;
    //public Image forceBarImage;
    private Image _img;

    void Start()
    {
        _img = GetComponent<Image>();
    }

    void Update()
    {
        float currentJumpForce = heroController.currentJumpForce;
        if (currentJumpForce > 0f)
        {
            float maxJumpForce = heroController.maxJumpForce;            

            _img.fillAmount = currentJumpForce / maxJumpForce;
        }
        else if (_img.fillAmount >= 0f)
        {
            _img.fillAmount = 0f;
        }        
    }
}
