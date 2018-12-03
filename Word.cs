using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Word : MonoBehaviour {

    public string word;
    public int currChar;
    
    [SerializeField] TMP_Text tmpText;

    public int optionVal;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void SetWord(string str, int val)
    {
        word = str;
        tmpText.text = str;
        currChar = 0;
        optionVal = val;
    }

    public void HighlightChar()
    {
        //print("DisplayWord");
        TMP_TextInfo textInfo = tmpText.textInfo;
        Color32[] newVertexColors;
        Color32 c0 = tmpText.color;

        int characterCount = textInfo.characterCount;

        if (characterCount == 0)
            return;

        int materialIndex = textInfo.characterInfo[currChar].materialReferenceIndex;
        newVertexColors = textInfo.meshInfo[materialIndex].colors32;
        int vertexIndex = textInfo.characterInfo[currChar].vertexIndex;

        if (textInfo.characterInfo[currChar].isVisible)
        {
            c0 = new Color32(255, 0, 255, 255);

            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;

            // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
            // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public char GetChar()
    {
        if (currChar >= word.Length)
            return '\0';

        while(word[currChar] == ' ')
            currChar++;

        return char.ToLower(word[currChar]);
    }

    public void CharTyped()
    {
        //print("LetterTyped");
        HighlightChar();
        currChar++;
        WordCompleted();
    }

    public void WordCompleted()
    {
        if (currChar >= word.Length)
        {
            DialogueManager.Instance.SelectOption(optionVal);
        }
    }
}
