using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionClick : MonoBehaviour
{
    public firge firgeScript;         // 拖入 frige 脚本
    public GameObject dialogueUI;     // 拖入 dialogue 对象（UI）

    public void OnClickSelection()
    {
        if (firgeScript != null)
        {
            firgeScript.ResetToCalm(); // 清空怒气 & 状态归零
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 关闭对话框
        }
    }
}
