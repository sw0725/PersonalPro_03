using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public AnimationCurve curve;
    public float maxExpend = 100.0f;
    public float recorveryDuration = 0.5f;
    public WeaponType type = WeaponType.Bat;

    const float recorveryWaitTime = 0.1f;
    const float defaultEapend = 10.0f;

    float divPreCompute;
    float current = 0.0f;

    RectTransform[] crossRects;

    readonly Vector2[] direction = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    private void Awake()
    {
        crossRects = new RectTransform[transform.childCount-1];
        for (int i = 0; i < transform.childCount-1; i++)
        {
            crossRects[i] = transform.GetChild(i+1) as RectTransform;
        }

        divPreCompute = 1 / recorveryDuration;
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;

        WeaponBase[] weapon = player.transform.GetChild(1).GetComponentsInChildren<WeaponBase>(true);
        foreach (WeaponBase weaponItem in weapon) 
        {
            if (weaponItem.weaponType == type) 
            {
                weaponItem.onFire += Expend;
                gameObject.SetActive(false);
            }
        }
    }

    public void Expend(float amount)
    {
        current = Mathf.Min(current + amount, maxExpend);
        for (int i = 0; i < crossRects.Length; i++)
        {
            crossRects[i].anchoredPosition = (current + defaultEapend) * direction[i];
        }

        StopAllCoroutines();
        StartCoroutine(DelayRecovery(recorveryWaitTime));
    }

    IEnumerator DelayRecovery(float wait)
    {
        yield return new WaitForSeconds(wait);

        float startExpend = current;
        float curveProcess = 0.0f;

        while (curveProcess < 1)
        {
            curveProcess += Time.deltaTime * divPreCompute;
            current = curve.Evaluate(curveProcess) * startExpend;

            for (int i = 0; i < crossRects.Length; i++)
            {
                crossRects[i].anchoredPosition = current * direction[i];
            }
            yield return null;
        }
        for (int i = 0; i < crossRects.Length; i++)
        {
            crossRects[i].anchoredPosition = Vector2.zero;
        }

        current = 0;
    }
}
