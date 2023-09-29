using UnityEngine;
using UnityEngine.UI;

public class UIText : MonoBehaviour
{
    public Text display_Text;
    private float current = 0;


    private static Text food_cpt_txt;
    private static int food_cpt;

    private void Awake()
    {
        food_cpt_txt = GameObject.FindWithTag("Food_cpt").GetComponent<Text>();
        food_cpt = 0;
    }

    public void Update()
    {
        current = (int)(1f / Time.unscaledDeltaTime);
        display_Text.text = current.ToString() + " FPS";
    }

    public static void addFood()
    {
        food_cpt += 1;
        food_cpt_txt.text = food_cpt.ToString();
    }
}