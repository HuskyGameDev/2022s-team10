using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickButton : MonoBehaviour
{
    private Collider2D thisCollider;
    public string toScene;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetMouseButtonDown(0) && thisCollider.OverlapPoint(mousePos))
        {
            if (toScene == "Quit")
                Application.Quit();
            else if (toScene == "No")
                Invis();
            else
                SceneManager.LoadScene(toScene);
        }
    }

    public void Invis()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0;
        sr.color = color;
    }
}
