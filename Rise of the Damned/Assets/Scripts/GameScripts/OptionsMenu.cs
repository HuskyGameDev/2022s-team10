using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    private RectTransform t;
    Vector3 hiddenScale, shownScale;
    MainMenu menu;

    // Start is called before the first frame update
    void Start()
    {
        GameObject optionsButton = GameObject.Find("SelectOptions");
        menu = optionsButton.GetComponent<MainMenu>();
        t = GetComponent<RectTransform>();
        hiddenScale = new Vector3(0, 0, 0);
        shownScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (menu.optionsOpen){
            t.localScale = shownScale;
        } else {
            t.localScale = hiddenScale;
        }
    }
}
