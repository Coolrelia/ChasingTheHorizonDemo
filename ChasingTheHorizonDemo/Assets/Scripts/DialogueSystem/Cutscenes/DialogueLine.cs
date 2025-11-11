using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//A Dialogue Line, holds all the data for a single Dialogue Line
//Used in conjuction with the Dialogue Holder
namespace DialogueSystem
{
    public class DialogueLine : DialogueBaseClass
    {
        private Text textHolder;
        private Sprite portraitSprite;

        [Header("Text Options")]
        [TextArea(3,10)]
        [SerializeField] private string input = null;
        [SerializeField] private Color textColor = Color.black;
        [SerializeField] private Font textFont = null;

        [Header("Time Parameters")]
        [SerializeField] private float delay = 0;

        [Header("Sound Effect")]
        [SerializeField] private AudioClip sound = null;

        [Header("Music")]
        [SerializeField] private AudioClip music = null;
        public bool triggerMusic = false;

        [Header("Character")]
        [SerializeField] private UnitData character = null;

        [Header("Image Holder")]
        [SerializeField] private SpriteRenderer imageHolder = null;

        [Header("Nameplates")]
        public Text namePlate1;
        public Text namePlate2;

        [Header("Textbox")]
        public bool textBox1;
        public bool textBox2;

        public bool lastLine = false;

        private void Awake()
        {
            textHolder = GetComponent<Text>();
            textHolder.text = "";

            portraitSprite = character.portrait;
        }

        private void Start()
        {
            StartCoroutine(StartText());
        }

        private IEnumerator StartText()
        {            
            if(sound != null)
            {
                StartCoroutine(PlaySoundEffect(sound));
                yield return new WaitForSeconds(sound.length);

                imageHolder.sprite = portraitSprite;

                if (textBox1)
                {
                    namePlate1.text = character.unitName;
                    imageHolder.flipX = true;
                }
                else
                {
                    namePlate2.text = character.unitName;
                }
                
                if (triggerMusic && music != null)
                {
                    MusicPlayer.instance.PlayTrack(music);
                }

                StartCoroutine(WriteText(input, textHolder, textColor, textFont, delay, sound));
            }
            else
            {
                imageHolder.sprite = portraitSprite;

                if (textBox1)
                {
                    namePlate1.text = character.unitName;
                    imageHolder.flipX = true;
                }
                else
                {
                    namePlate2.text = character.unitName;
                }
                
                if (triggerMusic && music != null)
                {
                    MusicPlayer.instance.PlayTrack(music);
                }

                StartCoroutine(WriteText(input, textHolder, textColor, textFont, delay, sound));
            }
        }
    }
}