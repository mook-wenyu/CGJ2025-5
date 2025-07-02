using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueContentBG;
    public TextMeshProUGUI dialogueContent;
    public RectTransform selection1;
    public RectTransform selection2;

    [HideInInspector]
    public int currentIndex = 0;

    private bool isShow = false;

    public void Say(string content)
    {
        dialogueContent.text = content;
        isShow = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContentBG.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    public void HideDialogue()
    {
        isShow = false;
        gameObject.SetActive(false);
    }

    public void SetIsShow(bool isShow)
    {
        this.isShow = isShow;
    }

    public bool GetIsShow()
    {
        return isShow;
    }
}
