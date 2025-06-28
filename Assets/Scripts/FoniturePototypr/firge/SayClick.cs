using UnityEngine;

public class SayClick : MonoBehaviour
{
    public firge firgeScript;         // ���� frige �ű�
    public GameObject dialogueUI;     // ���� dialogue ����UI��
    public RectTransform selection1;  // ���� selection1 �� RectTransform
    public RectTransform selection2;  // ���� selection2 �� RectTransform

    private void OnMouseDown()
    {
        if (firgeScript != null)
        {
            firgeScript.PauseAngerGrowth(); // ��ͣŭ��ֵ
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);     // ��ʾ�Ի���

            // 50% ���ʽ���������ť��λ��
            if (Random.value < 0.5f && selection1 != null && selection2 != null)
            {
                Vector3 tempPos = selection1.anchoredPosition;
                selection1.anchoredPosition = selection2.anchoredPosition;
                selection2.anchoredPosition = tempPos;

                Debug.Log("��ťλ���ѽ���");
            }
            else
            {
                Debug.Log("��ťλ��δ����");
            }
        }
    }
}
