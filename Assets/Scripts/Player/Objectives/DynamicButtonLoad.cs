using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonLoad : MonoBehaviour
{
    [SerializeField] TextDialogue initialDialogue;

    [SerializeField][Tooltip("Built to be a Panel w/ a Layout Group Component")] GameObject buttonContainer;
    [SerializeField][Tooltip("Requires Button Component On Parent And TMP_Text on Child")] GameObject buttonPrefab;
    //[SerializeField][Tooltip("Requires the TMP_Text Component")] GameObject textPrefab;

    // Used To Delete TextBoxes After Selection is Made
    List<GameObject> currentActiveButtons = new List<GameObject>();

    [System.Serializable]
    public struct TextOptions
    {
        [Header("TextOptions")]
        public string text;
        public int optionId;
    }

    [System.Serializable]
    public struct TextDialogue
    {
        [Header("TextDialogue")]
        public string chatName;
        //public string dialogue;
        public List<TextOptions> options;
    }

    public void GenerateTextButtons(TextDialogue textInfo)
    {
        RemoveActiveDialogue();

        //GameObject newText = Instantiate(textPrefab, buttonContainer.transform);
        //newText.name = textInfo.chatName;
        //newText.GetComponent<TMP_Text>().text = textInfo.dialogue;

        //currentActiveButtons.Add(newText);

        foreach (var info in textInfo.options)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer.transform);
            newButton.name = textInfo.chatName + "Option: " + info.optionId;
            newButton.GetComponentInChildren<TMP_Text>().text = info.text;
            newButton.GetComponent<Button>().onClick.AddListener(() => OptionSelected(info.optionId));

            currentActiveButtons.Add(newButton);
        }
    }

    public void OptionSelected(int selection)
    {
        print(selection);
    }

    public void RemoveActiveDialogue()
    {
        for (int i = 0; i < currentActiveButtons.Count;)
        {
            GameObject go = currentActiveButtons[i];
            currentActiveButtons.Remove(go);
            Destroy(go);
        }
    }

    void Start()
    {
        GenerateTextButtons(initialDialogue);
    }
}
