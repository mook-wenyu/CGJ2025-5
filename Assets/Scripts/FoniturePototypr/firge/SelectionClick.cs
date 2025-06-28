using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionClick : MonoBehaviour
{
    public firge firgeScript;         // ���� frige �ű�
    public GameObject dialogueUI;     // ���� dialogue ����UI��

    public void OnClickSelection()
    {
        if (firgeScript != null)
        {
            firgeScript.ResetToCalm(); // ���ŭ�� & ״̬����
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // �رնԻ���
        }
    }
}
