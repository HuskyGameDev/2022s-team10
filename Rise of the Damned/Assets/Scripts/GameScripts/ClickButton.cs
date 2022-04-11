using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickButton : MonoBehaviour
{
    private Collider2D thisCollider;

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
            if (gameObject.name.Equals("RetryButton"))
            {
                SceneManager.LoadScene("LevelOne");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
