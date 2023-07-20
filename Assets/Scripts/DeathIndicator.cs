using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DeathIndicator : MonoBehaviour
{
    public Transform player;

    public Camera cam;
    public Canvas canvas;
    public Transform self; 
    [SerializeField] Image fishIcon;
    [SerializeField] RectTransform arrow;
    [SerializeField] List<Image> deaths = new List<Image>();
    [SerializeField] Image bigDeath; 

    [SerializeField] float margin;
    [SerializeField] float deathDelayPopUpEnd;

    public bool enableIndicator;
    public bool instaKill;

    bool tracking; 

    Vector3 ogSize;
    float spawnAnimationDelay = 0.2f;

    public void SetFishImage(Sprite image)
    {
        fishIcon.sprite = image;
    }

    private void Start()
    {
        ogSize = self.localScale;
        self.localScale = Vector3.zero;
        DeactivateDeaths();
    }

    private void Enable()
    {
        DeactivateDeaths();
        tracking = true;
        self.gameObject.SetActive(true);
        self.DOScale(ogSize, spawnAnimationDelay);
        StopAllCoroutines();// to stop Deactivation of self
        DeathsTicking();
    }
    private void Disable()
    {
        tracking = false;
        self.DOScale(0, spawnAnimationDelay);
        StartCoroutine(DeactivateWithDelay(self.gameObject, spawnAnimationDelay));
    }
    private void FixedUpdate()
    {
        if (enableIndicator && !tracking)
        {
            Enable();
        }
        else if (!enableIndicator && tracking) {
            Disable();
        }
        if (tracking)
        {
            SnapToPlayerPosition(player);
        }

        if (instaKill)
        {
            instaKill = false;
            InstaKill();
        }
    }
    void InstaKill()
    {
        SnapToPlayerPosition(player);
        if (!enableIndicator)
        {
            bigDeath.transform.localScale = Vector3.zero;
            bigDeath.gameObject.SetActive(false);
            foreach (var img in deaths)
            {
                img.gameObject.SetActive(false);
                img.transform.localScale = Vector3.zero;
            }
        }
        self.gameObject.SetActive(true);
        self.DOScale(ogSize, spawnAnimationDelay);
        BigDeath(deathDelayPopUpEnd);
    }
    void DeathsTicking()
    {
        StartCoroutine(ActivateWithDelay(deaths[0].gameObject, 0.6f,true));
        StartCoroutine(ActivateWithDelay(deaths[1].gameObject, 0.6f + 0.6f,true));
        StartCoroutine(ActivateWithDelay(deaths[2].gameObject, 0.6f + 0.6f + 0.6f,true, OnCoroutineFinished));

    }
    void BigDeath(float extraDelay = 0)
    {
        StartCoroutine(ActivateWithDelay(bigDeath.gameObject,0.3f, true));
        StartCoroutine(DisableSelf(0.3f + spawnAnimationDelay + extraDelay));
    }
    void DeactivateDeaths()
    {
        StopAllCoroutines();// to stop activation
        bigDeath.transform.localScale = Vector3.zero;
        bigDeath.gameObject.SetActive(false);
        foreach (var img in deaths)
        {
            img.gameObject.SetActive(false);
            img.transform.localScale = Vector3.zero;
        }
    }

    private void SnapToPlayerPosition(Transform playerPosition)
    {
        if (playerPosition == null) return; 
        Vector3 screenPosition = cam.WorldToScreenPoint(playerPosition.position);
        if (screenPosition.x > cam.scaledPixelWidth - margin) screenPosition.x = cam.scaledPixelWidth - margin;
        if (screenPosition.y > cam.scaledPixelHeight - margin) screenPosition.y = cam.scaledPixelHeight - margin;
        if (screenPosition.x < margin) screenPosition.x = margin;
        if (screenPosition.y < margin) screenPosition.y = margin;
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, cam, out localPosition);
        self.localPosition = new Vector3(localPosition.x, localPosition.y, 0);


        Vector3 bruh = cam.WorldToScreenPoint(playerPosition.position) - screenPosition;
        arrow.right = bruh;
    }

    IEnumerator DeactivateWithDelay(GameObject Object, float delay)
    {
        yield return new WaitForSeconds(delay);
        Object.SetActive(false);
    }
    IEnumerator ActivateWithDelay(GameObject Object, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Object != self.gameObject) Object.SetActive(true);
    }
    IEnumerator ActivateWithDelay(GameObject Object, float delay, bool animateScale)
    {
        yield return new WaitForSeconds(delay);
        if (Object != self.gameObject) Object.SetActive(true);
        if (animateScale)
        {
            Object.transform.DOScale(1, spawnAnimationDelay);
            //if(Object != bigDeath.gameObject)self.transform.DOScale(1.2f, spawnAnimationDelay * 2f);
            //StartCoroutine(ActivateWithDelay(self.gameObject, spawnAnimationDelay * 2f, true, 0.5f));
        }
    }
    IEnumerator ActivateWithDelay(GameObject Object, float delay, bool animateScale, float animationSpeedMult)
    {
        yield return new WaitForSeconds(delay);
        if(Object != self.gameObject)Object.SetActive(true);
        if (animateScale)
        {
            Object.transform.DOScale(1, spawnAnimationDelay * animationSpeedMult);
        }
    }

    IEnumerator ActivateWithDelay(GameObject Object, float delay, bool animateScale, Action<string> callback)
    {
        yield return new WaitForSeconds(delay);
        if (Object != self.gameObject) Object.SetActive(true);
        if (animateScale)
        {
            Object.transform.DOScale(1, spawnAnimationDelay);
        }
        callback?.Invoke("death");
    }
    private void OnCoroutineFinished(string corotineType)
    {
        if (corotineType == "death")
        {
            BigDeath(deathDelayPopUpEnd);
        }
    }
    IEnumerator DisableSelf(float delay)
    {
        yield return new WaitForSeconds(delay);
        enableIndicator = false;
        if (!tracking) Disable();
    }

}
