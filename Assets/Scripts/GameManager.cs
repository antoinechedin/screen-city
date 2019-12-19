using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Canvas canvas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F3) && canvas != null)
        {
            Transform controls = canvas.transform.Find("Controls");
            if (controls != null)
            {
                controls.gameObject.SetActive(!controls.gameObject.activeSelf);
            }
        }
    }
}
