using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SayClick : MonoBehaviour
{
    public Firge firgeScript;         // 拖入 frige 脚本
    public DialogueUI dialogueUI;

    [HideInInspector]
    public int currentIndex = 0;

    public void OnClicked()
    {
        if (GameMgr.IsTimePaused) return;

        if (firgeScript != null)
        {
            firgeScript.PauseAngerGrowth(); // 暂停怒气值
        }

        if (dialogueUI != null)
        {
            AudioMgr.Instance.PlaySound("电冰箱说话");
            currentIndex = Random.Range(0, firgeScript.dialogueContentList.Count);

            dialogueUI.selection1.gameObject.SetActive(true);
            dialogueUI.selection1.GetComponent<Button>().onClick.RemoveAllListeners();
            dialogueUI.selection1.GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioMgr.Instance.PlaySound("电冰箱说话");
                dialogueUI.Say(firgeScript.dialogueContentList[currentIndex].c2);
                dialogueUI.selection1.gameObject.SetActive(false);
                dialogueUI.selection2.gameObject.SetActive(false);
                StartCoroutine(WaitForDialogue());
            });


            dialogueUI.selection2.gameObject.SetActive(true);
            if (dialogueUI.selection2.GetComponent<Button>().onClick.GetPersistentEventCount() == 0)
            {
                dialogueUI.selection2.GetComponent<Button>().onClick.AddListener(firgeScript.ClickClose);
            }

            dialogueUI.gameObject.SetActive(true);     // 显示对话框
            dialogueUI.Say(firgeScript.dialogueContentList[currentIndex].c1);
            // 50% 概率交换两个按钮的位置
            if (Random.value < 0.5f && dialogueUI.selection1 != null && dialogueUI.selection2 != null)
            {
                Vector3 tempPos = dialogueUI.selection1.anchoredPosition;
                dialogueUI.selection1.anchoredPosition = dialogueUI.selection2.anchoredPosition;
                dialogueUI.selection2.anchoredPosition = tempPos;

                Debug.Log("按钮位置已交换");
            }
            else
            {
                Debug.Log("按钮位置未交换");
            }
        }

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    IEnumerator WaitForDialogue()
    {
        yield return new WaitForSeconds(1.5f / GameMgr.timeScale);

        firgeScript.SwitchToNormal();
    }
}
