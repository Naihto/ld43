using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager Instance;

    public List<Word> optionList;

    public List<char> sacrificedChar;

    [SerializeField] List<string> data;

    [SerializeField] int dialogueCtr;
    //[SerializeField] Queue<string> dialogueQueue;
    [SerializeField] string currDialogue;

    [SerializeField] bool isSentenceCompleted;
    [SerializeField] int optionVal;
    [SerializeField] char heldChar;

    [SerializeField] float displaySpeed;
    [SerializeField] TextAsset textFile;
    [SerializeField] TMP_Text textbox;

    [Header("Bool Checks")]
    [SerializeField] bool isNextSentence;

    [SerializeField] bool isInGame;

    [SerializeField] bool isOptionSelecting;

    [SerializeField] bool isSacrificing;
    [SerializeField] bool isSacrificingCheck;

    [SerializeField] bool isOffering;
    [SerializeField] bool isOfferingCheck;

    [SerializeField] bool isResetCheck;
    [SerializeField] bool isGameOver;

    [SerializeField] GameObject dialogueIndicator;

    [Header("Option Select Properties")]
    [SerializeField] GameObject optionBoxes;
    [SerializeField] Image optionTimerUI;
    [SerializeField] float optionTimer;
    [SerializeField] float optionTimerDur;

    [Header("Gameplay Properties")]
    [SerializeField] int initHappiness = 5;
    [SerializeField] int maxHappiness = 10;
    [SerializeField] int happiness;
    [SerializeField] int offeringCost = 2;

    [Header("Sound Variables")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip voiceClip;
    [SerializeField] AudioClip tapClip;
    [SerializeField] AudioClip goodAlert;
    [SerializeField] AudioClip badAlert;

    [Header("UI Variables")]
    [SerializeField] Image charaImage;

    [SerializeField] TMP_Text happinessTextbox;
    [SerializeField] TMP_Text alertTextbox;

    [SerializeField] GameObject sacrificedPanel;
    [SerializeField] TMP_Text sacrificedCharTextbox;
    [SerializeField] Toggle sacrificedSortToggle;

    [SerializeField] GameObject sacrificePopupPanel;
    [SerializeField] GameObject sacrificePopupImages;
    [SerializeField] TMP_Text sacrificePopupTextbox;
    [SerializeField] TMP_Text sacrificePopupTextbox2;

    [SerializeField] GameObject popupPanel;
    [SerializeField] GameObject popupImages;
    [SerializeField] TMP_Text popupTextbox;
    [SerializeField] TMP_Text popupTextbox2;

    [SerializeField] GameObject offeringPopupPanel;
    [SerializeField] GameObject offeringPopupImages;
    [SerializeField] TMP_Text offeringPopupTextbox;
    [SerializeField] TMP_Text offeringPopupTextbox2;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //dialogueQueue = new Queue<string>();
        dialogueCtr = 0;
    }

    void Start()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        //print("ResetGame");
        textbox.text = "Typeless";

        optionList[0].SetWord("Start", -1);
        optionList[1].SetWord("Begin", -1);
        optionList[2].SetWord("Date", -1);

        isResetCheck = false;
        isSentenceCompleted = false;

        isOptionSelecting = true;
        optionBoxes.SetActive(true);

        sacrificedChar.Clear();

        happiness = Mathf.Abs(initHappiness);
        UpdateHappiness(0);

        alertTextbox.text = "";

        popupPanel.SetActive(false);
        sacrificePopupPanel.SetActive(false);
        SetPanelActive(sacrificedPanel, false);
        SetPanelActive(offeringPopupPanel, false);
        optionTimerUI.transform.parent.gameObject.SetActive(false);

        UpdateSacrificedCharTextbox();

        isInGame = false;
        isGameOver = false;
    }

    public void OpenDialogueFile(int num, string name)
    {
        //print("OpenDialogueFile");
        isGameOver = false;
        isInGame = true;
        textFile = Resources.Load<TextAsset>("Dialogue" + Path.DirectorySeparatorChar + ((name == "") ? "dialogue" : name) + num.ToString());
        SetDialogue();
    }

    void SetDialogue()
    {
        //print("SetDialogue");
        //dialogueQueue.Clear();
        

        data.Clear();
        dialogueCtr = 0;

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            data = Regex.Split(textFile.text, "\r\n").ToList();
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
            data = Regex.Split(textFile.text, "\n|\r").ToList();
        else
            data = Regex.Split(textFile.text, "\n|\r|\r\n").ToList();

        //foreach (string str in data)
        //    dialogueQueue.Enqueue(str);

        InitGameUI();

        isSentenceCompleted = true;
        //print("SetDialogue");
        DisplayNextDialogue();
    }

    void InitGameUI()
    {
        sacrificedPanel.SetActive(true);
    }

    void EndGame()
    {
        ResetGame();
    }

    void DisplayNextDialogue()
    {
        //print("DisplayNextDialogue");

        if (isGameOver)
        {
            OpenDialogueFile(0, "");
            return;
        }

        if (!isSentenceCompleted)
        {
            EndSentence();
            return;
        }

        isSentenceCompleted = false;

        //currDialogue = dialogueQueue.Dequeue();
        currDialogue = data[dialogueCtr];
        dialogueCtr++;

        //print("Dialogue Ctr: " + dialogueCtr);

        string[] str = currDialogue.Split('|');

        //print("Curr dialogue: " + currDialogue);

        if (string.IsNullOrEmpty(currDialogue))
        {
            //print("String IsNullOrEmpty");
            isSentenceCompleted = true;
            DisplayNextDialogue();
            return;
        }
            

        if (string.IsNullOrWhiteSpace(currDialogue))
        {
            //print("String IsNullOrWhiteSpace");
            isSentenceCompleted = true;
            DisplayNextDialogue();
            return;
        }

        //print("Split String str[0]: " + str[0]);

        int number;
        if (int.TryParse(str[0], out number))
        {
            if (int.Parse(str[0]) == optionVal)
            {
                //print("Dialogue option: " + optionVal);
                optionVal = int.Parse(str[1]);

                if(str[2].ToLower() == "image")
                    ImageKey(str[3]);
                else if(str[2].ToLower() == "animation")
                    AnimKey(str[3]);
                else
                {
                    UpdateHappiness(int.Parse(str[3]));
                    StartCoroutine(DelayedDisplay(str[2]));
                }
                return;
            }
            else
            {
                //print("OptionVal: " + optionVal + " != " + str[0]);
                isSentenceCompleted = true;
                DisplayNextDialogue();
                return;
            }
        }
        else
        {
            if (optionVal != 0)
            {
                //print("OptionVal = " + optionVal + " && str = " + str[0]);
                isSentenceCompleted = true;
                DisplayNextDialogue();
                return;
            }

            isNextSentence = false;

            switch (str[0].ToLower())
            {
                case "image":
                    ImageKey(str[1]);
                    break;

                case "dialogue":
                    //print("Dialogue");

                    currDialogue = str[1];
                    StartCoroutine(DelayedDisplay(currDialogue));
                    break;

                case "option":
                    //print("Option");
                    //int ctr = 0;
                    for (int i = 0; i < optionList.Count; i++)
                    {
                        if (i + 1 < str.Length)
                        {
                            if (!optionList[i].gameObject.activeSelf)
                                optionList[i].gameObject.SetActive(true);

                            optionList[i].SetWord(str[i + 1], i + 2);
                        }
                        else
                            optionList[i].gameObject.SetActive(false);
                        //if (str[i + 1].ToCharArray().All(sacrificedChar.Contains))
                        //    ctr++;
                    }
                    isOptionSelecting = true;
                    optionBoxes.SetActive(true);
                    optionTimer = Mathf.Abs(optionTimerDur);
                    optionTimerUI.transform.parent.gameObject.SetActive(true);
                    break;

                case "animation":
                    AnimKey(str[1]);
                    break;

                case "end":
                    EndGame();
                    textbox.text = str[1];
                    break;
            }
        }

        if (isNextSentence)
        {
            //print("isNextSentence");
            DisplayNextDialogue();
        }
    }

    void ImageKey(string str)
    {
        //print("ImageKey: " + str);
        charaImage.sprite = Resources.Load<Sprite>("Dialogue_Images/" + str);
        isSentenceCompleted = true;
        DisplayNextDialogue();
    }

    void AnimKey(string str)
    {
        //print("AnimKey: " + str);
        charaImage.GetComponent<Animator>().Play(str);
        isSentenceCompleted = true;
        DisplayNextDialogue();
    }

    IEnumerator DelayedDisplay(string text)
    {
        isNextSentence = false;
        ////print("Frame before DelayedDisplay");
        //yield return new WaitForEndOfFrame();

        //print("DelayedDisplay: " + text);

        if (!string.IsNullOrEmpty(text))
        {

            bool play = true;
            textbox.text = text;
            textbox.maxVisibleCharacters = 0;
            for (int i = 0; i < text.Length; i++)
            {
                textbox.maxVisibleCharacters += 1;
                if (char.IsLetter(text[i]) && play)
                {
                    play = false;
                    audioSource.pitch = 2 + (UnityEngine.Random.Range(-10, 20) * -0.005f);
                    audioSource.PlayOneShot(voiceClip, 1);
                }

                if (text[i] == ' ')
                    play = true;

                yield return new WaitForSeconds(displaySpeed);
            }
        }

        isSentenceCompleted = true;
        if (string.IsNullOrEmpty(text))
        {
            //print("DelayedDisplay Text is empty: " + text);
            DisplayNextDialogue();
        }
    }

    public void EndSentence()
    {
        //print("EndSentence");
        StopAllCoroutines();
        textbox.maxVisibleCharacters = textbox.text.Length;
        isSentenceCompleted = true;
    }

    void TypedLetter(char letter)
    {
        ////print("TypedLetter: " + letter);
        foreach (var opt in optionList)
        {
            if (opt.GetChar() == letter)
            {
                opt.CharTyped();
            }
        }
    }

    public void SelectOption(int val)
    {
        //print("SelectOption: " + val);

        sacrificePopupImages.SetActive(false);
        isOptionSelecting = false;
        optionTimerUI.transform.parent.gameObject.SetActive(false);

        if (val == -1)
        {
            textbox.text = "Date Start!";
            OpenDialogueFile(1, "");
            return;
        }

        optionVal = val;
        SacrificeCharPrompt();
    }

    void SacrificeCharPrompt()
    {
        //print("SacrificeCharPrompt");

        heldChar = '\0';
        isSacrificing = false;
        isSacrificingCheck = false;

        sacrificePopupPanel.SetActive(true);
        sacrificePopupImages.SetActive(false);
        offeringPopupPanel.SetActive(false);
        StartCoroutine(SacrificeInputGrace());
    }

    void OfferingCharPrompt(bool val)
    {
        isOffering = false;
        isOfferingCheck = false;
        offeringPopupImages.SetActive(false);

        if (!val)
        {
            heldChar = '\0';
            isOfferingCheck = false;
            offeringPopupPanel.SetActive(false);
        }
        else
        {
            offeringPopupPanel.SetActive(true);

            if (sacrificedChar.Count == 0)
            {
                //StartCoroutine(AlertPopUp(alertTextbox, "You have no letters sacrificed", 1));
                offeringPopupTextbox.text = "You have no letters sacrificed";
                return;
            }

            StartCoroutine(OfferingInputGrace());
        }
    }

    IEnumerator SacrificeInputGrace()
    {
        //print("SacrificeInputGrace");
        sacrificePopupTextbox.text = "";
        sacrificePopupTextbox2.text = "";
        yield return new WaitForSeconds(0.5f);
        sacrificePopupTextbox.text = "Input a letter now!";
        isSacrificing = true;
    }

    IEnumerator OfferingInputGrace()
    {
        //print("OfferingInputGrace");
        offeringPopupTextbox.text = "What letter do you wish to receive at the cost of " + offeringCost + " of their happiness?";
        offeringPopupTextbox2.text = "";
        offeringPopupTextbox2.text = "";
        yield return new WaitForSeconds(0.5f);
        offeringPopupTextbox2.text = "Input a letter now!";
        isOffering = true;
    }

    void SacrificeCharCheck(char letter)
    {
        //print("SacrificeCharCheck");
        offeringPopupTextbox2.text = "";
        isSacrificing = false;
        isSacrificingCheck = true;
        heldChar = letter;
        sacrificePopupTextbox.text = Char.ToUpper(letter).ToString();
        sacrificePopupTextbox2.text = "Are you sure?";
        sacrificePopupImages.SetActive(true);
    }

    void OfferingCharCheck(char letter)
    {
        //print("OfferingCharCheck");
        isOffering = false;
        isOfferingCheck = true;
        heldChar = letter;
        offeringPopupTextbox.text = Char.ToUpper(letter).ToString();
        sacrificePopupTextbox2.text = "Are you sure?";
        offeringPopupImages.SetActive(true);
    }

    void SacrificeChar()
    {
        //print("SacrificeChar");
        if (heldChar == '\0')
        {
            SacrificeCharPrompt();
            return;
        }

        sacrificedChar.Add(heldChar);
        heldChar = '\0';

        UpdateSacrificedCharTextbox();
        sacrificePopupPanel.SetActive(false);

        isSacrificing = false;
        isSacrificingCheck = false;

        isSentenceCompleted = true;
        DisplayNextDialogue();
    }

    void OfferingChar()
    {
        if (heldChar == '\0')
        {
            OfferingCharPrompt(true);
            return;
        }

        sacrificedChar.RemoveAt(sacrificedChar.IndexOf(heldChar));
        UpdateSacrificedCharTextbox();
        offeringPopupPanel.SetActive(false);

        isOffering = false;
        isOfferingCheck = false;

        UpdateHappiness(-Mathf.Abs(offeringCost));
    }

    void ResetOptions()
    {
        //print("ResetOptions");
        foreach (var opt in optionList)
        {
            opt.SetWord("", -1);
        }
    }

    private void Update()
    {
        if (isOptionSelecting || isSacrificing || isOffering)
        {
            foreach (char c in Input.inputString)
            {
                //if (!System.Char.IsLetter(c))
                //    break;

                audioSource.pitch = 1;
                audioSource.PlayOneShot(tapClip, 1);

                char letter = Char.ToLower(c);

                if (sacrificedChar.Count == 0)
                {
                    if (isOffering)
                        break;
                    else
                    if (isSacrificing)
                    {
                        if (!System.Char.IsLetter(c))
                            break;

                        SacrificeCharCheck(letter);
                        break;
                    }
                    else
                    {
                        TypedLetter(letter);
                    }
                }
                else
                {
                    bool b = false;

                    if (isOffering || isSacrificing)
                        b = true;

                    foreach (char rip_letter in sacrificedChar)
                    {
                        if (letter == rip_letter)
                        {
                            if (isOffering)
                            {
                                if (!System.Char.IsLetter(c))
                                    break;

                                OfferingCharCheck(letter);
                                break;
                            }

                            if (isSacrificing)
                            {
                                if (!System.Char.IsLetter(c))
                                    break;

                                sacrificePopupTextbox2.text = "Input an unsacrificed letter.";
                                b = true;
                                break;
                            }

                            b = true;
                        }
                        else
                        {
                            if (isSacrificing)
                            {
                                if (!System.Char.IsLetter(c))
                                    break;

                                b = false;
                            }
                            else if (isOffering)
                            {
                                if (!System.Char.IsLetter(c))
                                    break;

                                b = true;
                            }
                        }
                    }

                    if(isOffering || isSacrificing)
                    {
                        if (isOffering && !b)
                            offeringPopupTextbox2.text = "Input a sacrificed letter.";

                        if (isSacrificing && !b)
                            SacrificeCharCheck(letter);
                    }
                    else
                    {
                        if (!b)
                            TypedLetter(letter);
                    }
                }
            }

            if (isOptionSelecting && isInGame && !popupPanel.activeSelf && !sacrificePopupPanel.activeSelf && !offeringPopupPanel.activeSelf)
            {
                if (optionTimer > 0)
                {
                    if (!optionTimerUI.transform.parent.gameObject.activeSelf)
                        optionTimerUI.transform.parent.gameObject.SetActive(true);

                    optionTimer -= Time.deltaTime;
                    optionTimerUI.fillAmount = optionTimer / optionTimerDur;

                    if (Input.GetKeyDown(KeyCode.Delete) && !popupPanel.activeSelf)
                    {
                        optionTimer = 0;
                    }
                }
                else
                    SelectOption(1);
            }
        }

        if (isSacrificingCheck || isResetCheck || isOfferingCheck)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(tapClip, 1);

                if (isResetCheck)
                    ResetGame();

                if (!popupPanel.activeSelf)
                {
                    if (isSacrificingCheck)
                        SacrificeChar();

                    if (isOfferingCheck)
                        OfferingChar();
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(tapClip, 1);

                if (isResetCheck)
                {
                    popupPanel.SetActive(false);
                    isResetCheck = false;
                }

                if (!popupPanel.activeSelf)
                {
                    if (isSacrificingCheck)
                    {
                        //print("TEST");
                        SacrificeCharPrompt();
                    }

                    if (isOfferingCheck)
                        OfferingCharPrompt(true);
                }
            }
        }

        if (isInGame)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isOptionSelecting && !isSacrificing && !isSacrificingCheck)
            {
                //print("Space");

                audioSource.pitch = 1;
                audioSource.PlayOneShot(tapClip, 1);
                isNextSentence = true;
                DisplayNextDialogue();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(tapClip, 1);

                UpdateSacrificedCharTextbox();
                SetPanelActive(sacrificedPanel);
            }

            if (!sacrificePopupPanel.activeSelf)
                if (Input.GetKeyDown(KeyCode.Backslash))
                {
                    audioSource.pitch = 1;
                    audioSource.PlayOneShot(tapClip, 1);

                    OfferingCharPrompt(!offeringPopupPanel.activeSelf);
                }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(tapClip, 1);

                StartCoroutine(ResetGameInputGrace());
            }
        }

        if (dialogueIndicator != null)
            if (dialogueIndicator.activeSelf != isSentenceCompleted)
                dialogueIndicator.SetActive(isSentenceCompleted);
    }

    IEnumerator ResetGameInputGrace()
    {
        isResetCheck = false;
        if (popupPanel.activeSelf)
        {
            popupPanel.SetActive(false);
        }
        else
        {
            popupPanel.SetActive(true);
            popupTextbox.text = "Are you sure you want to reset?";
            //popupTextbox2.text = "";
            popupImages.SetActive(false);
            //print("ResetGameInputGrace");
            yield return new WaitForSeconds(0.5f);
            //popupTextbox2.text = "Enter (Y) / Backspace (N)\nInput a key now!";
            popupImages.SetActive(true);
            isResetCheck = true;
        }
    }

    private void UpdateHappiness(int val)
    {
        happiness += val;

        if (happiness < 0)
            isGameOver = true;

        if (happiness > maxHappiness)
        {
            happiness = maxHappiness;
            StartCoroutine(AlertPopUp(true, alertTextbox, "MAX", 2));
        }
        else if (val != 0)
            StartCoroutine(AlertPopUp(Mathf.Sign(val) > 0, alertTextbox, (Mathf.Sign(val) > 0) ? ("+" + val) : val.ToString(), 2));

        happinessTextbox.text = happiness.ToString();
    }

    void SetPanelActive(GameObject panel)
    {
        //sacrificedPanel.GetComponent<RectTransform>().localPosition = (Vector2)sacrificedPanel.GetComponent<RectTransform>().localPosition == Vector2.zero ? new Vector2(10000, 10000) : Vector2.zero;
        panel.SetActive(!panel.activeSelf);
    }

    void SetPanelActive(GameObject panel, bool val)
    {
        panel.SetActive(val);

        //if (!val)
        //    sacrificedPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10000, 10000);
        //else
        //    sacrificedPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    IEnumerator AlertPopUp(bool isGood, TMP_Text tmpText, string str, float timeTaken)
    {
        tmpText.text = str;
        audioSource.pitch = 1;
        audioSource.PlayOneShot(isGood ? goodAlert : badAlert, 0.7f);
        yield return new WaitForSeconds(timeTaken);
        tmpText.text = "";
    }

    public void UpdateSacrificedCharTextbox()
    {
        sacrificedCharTextbox.text = "";
        //string str = "";

        foreach (var c in sacrificedChar)
            sacrificedCharTextbox.text += Char.ToUpper(c) + " ";
            //str += Char.ToUpper(c) + " ";

        if (sacrificedSortToggle.isOn)
            //str = String.Concat(str.OrderBy(c => c));
            sacrificedCharTextbox.text = String.Join(" ", sacrificedCharTextbox.text.Split(' ').OrderBy(c => c));
    }
}
