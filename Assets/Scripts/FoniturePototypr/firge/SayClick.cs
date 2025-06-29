using TMPro;
using UnityEngine;

public class SayClick : MonoBehaviour
{
    public Firge firgeScript;         // 拖入 frige 脚本
    public GameObject dialogueUI;     // 拖入 dialogue 对象（UI）
    public TextMeshProUGUI dialogueContent;
    public RectTransform selection1;  // 拖入 selection1 的 RectTransform
    public RectTransform selection2;  // 拖入 selection2 的 RectTransform

    private int currentIndex = 0;

    private void OnMouseDown()
    {
        if (firgeScript != null)
        {
            firgeScript.PauseAngerGrowth(); // 暂停怒气值
        }

        if (dialogueUI != null)
        {
            currentIndex = Random.Range(0, firgeScript.dialogueContentList.Count);
            dialogueUI.SetActive(true);     // 显示对话框

            dialogueContent.text = firgeScript.dialogueContentList[currentIndex].c1;
            // 50% 概率交换两个按钮的位置
            if (Random.value < 0.5f && selection1 != null && selection2 != null)
            {
                Vector3 tempPos = selection1.anchoredPosition;
                selection1.anchoredPosition = selection2.anchoredPosition;
                selection2.anchoredPosition = tempPos;

                Debug.Log("按钮位置已交换");
            }
            else
            {
                Debug.Log("按钮位置未交换");
            }
        }
        gameObject.SetActive(false);
    }
}
