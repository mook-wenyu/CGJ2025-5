using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffect : MonoSingleton<ClickEffect>
{
    public Transform clickEffectPool;
    public GameObject clickEffectPrefab;

    private int poolSize = 5; // 对象池大小
    private Queue<GameObject> effectPool; // 对象池队列
    private float animationLength; // 缓存动画长度

    void Awake()
    {
        // 初始化对象池
        InitializePool();
    }

    // 初始化对象池
    private void InitializePool()
    {
        effectPool = new Queue<GameObject>();

        // 预先创建特效对象
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(clickEffectPrefab, clickEffectPool);

            // 如果是第一个创建的对象，获取并缓存动画长度
            if (i == 0 && animationLength <= 0)
            {
                Animator animator = effect.GetComponent<Animator>();
                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                    if (clips.Length > 0)
                    {
                        animationLength = clips[0].length;
                    }
                    else
                    {
                        animationLength = 0.5f; // 默认值
                    }
                }
            }

            effect.SetActive(false); // 初始时禁用
            effectPool.Enqueue(effect); // 加入队列
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标点击位置
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // 确保z轴位置为0，使特效在2D平面上

            // 创建点击特效
            CreateClickEffect(mousePosition);
        }
    }

    // 创建点击特效
    private void CreateClickEffect(Vector3 position)
    {
        GameObject effectInstance = GetEffectFromPool();
        if (effectInstance == null) return;

        // 设置位置并激活
        effectInstance.transform.position = position;
        effectInstance.SetActive(true);

        // 动画播放完成后回收到对象池
        StartCoroutine(RecycleEffect(effectInstance));
    }

    // 从对象池获取特效
    private GameObject GetEffectFromPool()
    {
        // 如果池中有对象，从池中取出
        if (effectPool.Count > 0)
        {
            return effectPool.Dequeue();
        }

        // 如果池为空，创建新对象
        GameObject newEffect = Instantiate(clickEffectPrefab, clickEffectPool);
        return newEffect;
    }

    // 回收特效到对象池
    private IEnumerator RecycleEffect(GameObject effect)
    {
        // 等待动画播放完成
        yield return new WaitForSeconds(animationLength > 0 ? animationLength : 0.5f);

        if (effect != null)
        {
            effect.SetActive(false);
            effectPool.Enqueue(effect); // 放回对象池
        }
    }
}
