using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager instance; // The Singleton

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps dialogue alive across scenes
        } else {
            Destroy(gameObject);
        }
    }

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    // }

    // // Update is called once per frame
    // void Update()
    // { 
    // }
}
