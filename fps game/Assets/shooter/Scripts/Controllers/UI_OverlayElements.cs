using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_OverlayElements : MonoBehaviour
{
    [SerializeField] private Text magAmmoText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text healthText; //text the displays player health
    [SerializeField] private GameObject interactText; //text for interacting with 
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image savingImage; //image displayed while saving
    [SerializeField] private Image weaponImage; //image of weapom
    [SerializeField] private Image weaponReticleImage; //weapon specific reticle image 

    private void Start()
    {
        magAmmoText = GameObject.Find("AmmoInMag").GetComponent<Text>();
        ammoText = GameObject.Find("AmmoCount").GetComponent<Text>();
        healthText = GameObject.Find("Health").GetComponent<Text>();
        magAmmoText.text = "";
        ammoText.text = "";
        healthText.text = "100";
        interactText = GameObject.Find("InteractText");
        healthBarImage = GameObject.Find("HealthBar").GetComponent<Image>();
        savingImage = GameObject.Find("SavingImage").GetComponent<Image>();
        weaponImage = GameObject.Find("WeaponImage").GetComponent<Image>();
        weaponReticleImage = GameObject.Find("ReticleImage").GetComponent<Image>();

        weaponImage.gameObject.SetActive(false);
        SetInteractTextActive(false);
        savingImage.canvasRenderer.SetAlpha(0.0f);
    }

    public void SetInteractTextActive(bool setActive)
    {
        if (setActive == true)
        {
            if (interactText.activeInHierarchy == false)
                interactText.SetActive(true);
        }
        else // if (setActive == false)
        {
            if (interactText.activeInHierarchy == true)
                interactText.SetActive(false);
        }
    }

    public void ActivateWeaponUI()
    {
        //weaponImage.gameObject.transform.parent.gameObject.SetActive(true);
        weaponImage.gameObject.transform.gameObject.SetActive(true);
        //weaponReticleImage.transform.parent.gameObject.SetActive(true); //default reticle is in its place
    }

    //called from Player_controller
    public void DisplaySavingImage(float timeToFade, int amountOfFades)
    {
        StartCoroutine(FadeImageIE(timeToFade, amountOfFades));
    }

    private IEnumerator FadeImageIE(float timeToFade, int amountOfFades)
    {
        while (amountOfFades > 0)
        {
            savingImage.CrossFadeAlpha(1f, timeToFade, true); //fade in
            yield return new WaitForSecondsRealtime(timeToFade); //use WaitForSecondsRealtime to continue with anim during pause
            savingImage.CrossFadeAlpha(0f, timeToFade, true); //fade out
            yield return new WaitForSecondsRealtime(timeToFade);

            amountOfFades--;
        }
    }

    public void UpdateHealthImage(float amount)
    {
        healthBarImage.fillAmount -= (amount / 100);
    }

    public void SetWeaponImages(Sprite weaponSprite, Sprite reticleTexture)
    {
        weaponImage.sprite = weaponSprite; //activates image of the weapon in the UI
        weaponReticleImage.sprite = reticleTexture; //activates reticle in the UI
    }

    //sets ammo text for UI
    public void SetAmmoText(GunAmmoContainer ammoContainer)
    {
        magAmmoText.text = ammoContainer.magCount.ToString();
        ammoText.text = ammoContainer.ammoCount.ToString();
    }

    //set Health text
    public void SetHealthText(float healthValue)
    {
        healthText.text = healthValue.ToString();
    }
}