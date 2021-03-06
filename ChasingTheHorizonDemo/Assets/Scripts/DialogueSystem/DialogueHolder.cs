﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        [Header("Dialogue Boxs")]
        [SerializeField] private Vector3 box1;
        [SerializeField] private Vector3 box2;

        [Header("Portraits")]
        [SerializeField] private Image portrait1;
        [SerializeField] private Image portrait2;

        public GameObject screenDim;

        CursorController cursor;

        private void Awake()
        {
            StartCoroutine(DialogueSequence());
        }

        public void Start()
        {
            cursor = FindObjectOfType<CursorController>();
        }

        public void StartDialogue()
        {
            StartCoroutine(DialogueSequence());
            cursor.ResetState();
            cursor.DialogueCursor = true;
        }

        private IEnumerator DialogueSequence()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            for (int i=0; i<transform.childCount; i++)
            {
                //Another check regarding whether it's a cutscene or not
                if(!currentScene.name.Contains("Cutscene"))
                {
                    screenDim.SetActive(true);
                }
                
                Deactivate();
                transform.GetChild(i).gameObject.SetActive(true);

              
                //Checks for a transition line
                if (transform.GetChild(i).GetComponent<TransitionLine>() == true)
                {
                    transform.GetChild(i).GetComponent<TransitionLine>().ChangeScene();
                }
                //Checks which Dialogue Box to use
                else if (transform.GetChild(i).GetComponent<DialogueLine>().textBox1 == true)
                {
                    transform.GetChild(i).GetComponent<RectTransform>().localPosition = box1;
                }
                else if(transform.GetChild(i).GetComponent<DialogueLine>().textBox2 == true)
                {
                    transform.GetChild(i).GetComponent<RectTransform>().localPosition = box2;
                }
                
                yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogueLine>().finished);

                //Checks if it's not a map scene
                if (!currentScene.name.Contains("Cutscene"))
                {
                    ClearDialogue();
                }
            }
        }

        private void Deactivate()
        {
            for(int i=0; i<transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void ClearDialogue()
        {
            foreach(DialogueLine line in FindObjectsOfType<DialogueLine>())
            {
                if(line.finished == true)
                {
                    screenDim.SetActive(false);
                    portrait1.sprite = null;
                    portrait2.sprite = null;
                    Deactivate();

                    cursor.ResetState();
                    cursor.MapCursor = true;
                }
            }
        }
    }
}