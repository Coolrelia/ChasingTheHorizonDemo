using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DialogueManagerPlus : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private AudioClip currentAudio;

    public TextAsset conversationTextAsset;
    public Sprite[] portraits;
    public Sprite[] cgs;
    public AudioClip[] audioTracks;

    private Queue<string> inputStream = new Queue<string>();
    [SerializeField] private Queue<string> outputStream = new Queue<string>();

    private void Start()
    {
        ReadTextFile();
        Invoke("StartDialogue", 2f);
    }

    private void ReadTextFile()
    {
        string txt = conversationTextAsset.text;

        string[] lines = txt.Split(System.Environment.NewLine.ToCharArray()); // Split dialogue lines by new line

        foreach(string line in lines) // for every line of dialogue
        {
            if (!string.IsNullOrEmpty(line)) // ignore empty lines of dialogue
            {
                if(line.StartsWith("["))
                {
                    string name = line.Substring(0, line.IndexOf(']') + 1);
                    string dialogue = line.Substring(line.IndexOf(']') + 1);
                    inputStream.Enqueue(name);
                    inputStream.Enqueue(dialogue);
                }
                else if (line.StartsWith("{"))
                {
                    string name = line.Substring(0, line.IndexOf(']') + 1);
                    inputStream.Enqueue(name);
                }
                else if (line.StartsWith("/"))
                {
                    string name = line.Substring(0, line.IndexOf(']') + 1);
                    inputStream.Enqueue(name);
                }
                else if (line.StartsWith("|"))
                {
                    string name = line.Substring(0, line.IndexOf(']') + 1);
                    inputStream.Enqueue(name);
                }
                else
                {
                    inputStream.Enqueue(line);
                }
            }
        }
        inputStream.Enqueue("EndQueue");
    }

    private void StartDialogue()
    {
        outputStream = inputStream;
        PrintDialogue();
    }

    private void PrintDialogue()
    {
        if(outputStream.Peek().Contains("EndQueue"))
        {
            outputStream.Dequeue();
            EndDialogue();
        }

        else if (outputStream.Peek().Contains("/SONG="))
        {
            string name = outputStream.Peek();
            name = outputStream.Dequeue().Substring(name.IndexOf('=') + 1, name.IndexOf(']') - (name.IndexOf('=') + 1));
            SetAudio(name);
            AdvanceDialogue();
        }

        else if(outputStream.Peek().Contains("{CG="))
        {
            string name = outputStream.Peek();
            name = outputStream.Dequeue().Substring(name.IndexOf('=') + 1, name.IndexOf(']') - (name.IndexOf('=') + 1));            
            SetCG(name);
            AdvanceDialogue();
        }
        
        else if (outputStream.Peek().Contains("|FX="))
        {
            string name = outputStream.Peek();
            name = outputStream.Dequeue().Substring(name.IndexOf('=') + 1, name.IndexOf(']') - (name.IndexOf('=') + 1));
            print("play " + name + " sound effect");
            AdvanceDialogue();
        }

        else if(outputStream.Peek().Contains("[NAME="))
        {
            string name = outputStream.Peek();
            name = outputStream.Dequeue().Substring(name.IndexOf('=') + 1, name.IndexOf(']') - (name.IndexOf('=') + 1));
            nameText.text = name;
            SetPortrait(name);
            PrintDialogue();
        }
        else
        {
            dialogueText.text = outputStream.Dequeue();
        }
    }

    private void SetPortrait(string name)
    {
        foreach (Sprite portrait in portraits)
        {
            if (portrait.name == name)
            {
                portraitImage.sprite = portrait;
            }
        }
    }
    private void SetCG(string name)
    {
        foreach(Sprite cg in cgs)
        {
            if(cg.name == name)
            {
                backgroundImage.sprite = cg;
            }
        }
    }
    private void SetAudio(string name)
    {
        foreach(AudioClip track in audioTracks)
        {
            if(track.name == name)
            {
                currentAudio = track;
            }
        }
    }

    public void AdvanceDialogue()
    {
        PrintDialogue();
    }

    private void EndDialogue()
    {
        dialogueText.text = "";
        nameText.text = "";
        portraitImage.sprite = null;
        outputStream.Clear();
    }
}
