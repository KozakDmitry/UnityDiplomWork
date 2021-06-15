using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Image soundButton;
    [SerializeField]
    private GameObject multiMenu;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private List<Sprite> soundSprites;
    private bool soundEnabled;

    public void StartGame()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            soundEnabled = !bool.Parse(PlayerPrefs.GetString("Sound"));
            ChangeSound();
        }
        multiMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
    public void ChangeSound()
    {
        if (soundEnabled)
        {
            AudioListener.volume = 0;
            soundEnabled = false;
            soundButton.sprite = soundSprites[1];
            PlayerPrefs.SetString("Sound", bool.FalseString);
        }
        else
        {
            AudioListener.volume = 1;
            soundEnabled = true;
            soundButton.sprite = soundSprites[0];
            PlayerPrefs.SetString("Sound", bool.TrueString);
        }
    }

    public void BackToMenu()
    {
        multiMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
