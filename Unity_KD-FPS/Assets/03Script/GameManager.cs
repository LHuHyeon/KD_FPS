using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true;
    public static bool isOpenInventory = false;
    public static bool isOpenCraftManual = false;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // <- CursorLockMode.Locked에 들어있음.
    }

    void Update()
    {
        if (isOpenInventory || isOpenCraftManual){
            canPlayerMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else{
            canPlayerMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}