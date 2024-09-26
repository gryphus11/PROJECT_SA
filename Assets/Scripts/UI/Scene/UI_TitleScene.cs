using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

public class UI_TitleScene : UI_Scene
{
    enum TextMeshPros
    { 
        Title,
        LoadingText,
    }

    enum Sliders
    {
        LoadingBar,
    }

    enum GameObjects
    {
        LoadingRotateBlur,
    }

    enum Buttons
    {
        MoveGameSceneButton,
    }

    Slider _loadingBar;
    TMP_Text _title;
    TMP_Text _loadingText;
    GameObject _loadingRotateBlur;
    Button _moveGameSceneButton;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindTMP(typeof(TextMeshPros));
        BindSlider(typeof(Sliders));
        BindButton(typeof(Buttons));

        _loadingRotateBlur = GetObject((int)GameObjects.LoadingRotateBlur);
        _loadingBar = GetSlider((int)Sliders.LoadingBar);
        _title = GetTMP((int)TextMeshPros.Title);
        _loadingText = GetTMP((int)TextMeshPros.LoadingText);
        _moveGameSceneButton = GetButton((int)Buttons.MoveGameSceneButton);
        _moveGameSceneButton.gameObject.SetActive(false);

        Managers.Resource.LoadAsyncLabel<Object>("Preload", OnResourceLoaded, OnComplete);
        return true;
    }

    private void OnComplete()
    {
        string loadedMessage = "Loading Complete.";
        _loadingText.text = loadedMessage;
        _loadingRotateBlur.SetActive(false);
        _moveGameSceneButton.gameObject.SetActive(true);
        Debug.Log(loadedMessage);
    }

    private void OnResourceLoaded(string resourceName, int current, int total)
    {
        string loadedMessage = $"{resourceName} ({current} / {total}) Loaded";
        _loadingText.text = loadedMessage;
        _loadingBar.value = (float)current / total;
        Debug.Log(loadedMessage);
    }
}
