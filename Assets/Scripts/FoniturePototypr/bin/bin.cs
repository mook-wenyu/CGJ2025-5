using System.Collections;
using UnityEngine;

public class Bin : Furniture
{
    public GameObject lid;
    public Sprite lidOpen;
    public Sprite lidClose;

    private SpriteButton lidBtn;

    protected override void StartFurniture()
    {
        base.StartFurniture();
        lid.SetActive(true);
        lidBtn = lid.GetComponent<SpriteButton>();
        lidBtn.SetSpriteRenderer(lid.GetComponent<SpriteRenderer>());
        lidBtn.SetSprite(lidClose);
        lidBtn.SetAlpha(1f);
    }

    /// <summary>
    /// 愤怒值更新
    /// </summary>
    protected override void AngerTick()
    {
        currentAnger++;
        if (currentAnger >= furniture.stageCrazy && status != FurnitureStatus.Crazy)
        {
            status = FurnitureStatus.Crazy;
            SwitchStatus(status);

            lidBtn.SetSprite(lidOpen);
            lidBtn.SetAlpha(0.001f);
            lidBtn.onClick.RemoveAllListeners();

            Debug.Log("进入状态3：垃圾桶失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            Debug.Log("进入状态2：垃圾桶黑化");
            return;
        }

    }

    protected override IEnumerator SwitchToSpecial(float delay)
    {
        yield return StartCoroutine(base.SwitchToSpecial(delay));

        lidBtn.SetSprite(lidOpen);
        lidBtn.SetAlpha(0.01f);
        lidBtn.onClick.AddListener(OnClicked);
        Debug.Log("进入状态1：垃圾桶进入特殊状态");
    }

    protected override void SwitchToNormal()
    {
        base.SwitchToNormal();

        lidBtn.SetSprite(lidClose);
        lidBtn.SetAlpha(1f);
        lidBtn.onClick.RemoveAllListeners();
        Debug.Log("进入状态0：垃圾桶恢复正常");
    }

    protected override void SwitchStatus(FurnitureStatus newStatus)
    {
        animator.SetInteger("bin_status", (int)newStatus);
        base.SwitchStatus(newStatus);
    }
}
