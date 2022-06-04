using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadScreen : MonoBehaviour
{
    public static LoadScreen instance;

    [SerializeField] private GameObject loadScreenContentHolder;
    public bool sceneIsLoading {get; private set;}

    [SerializeField] private TMP_Text ellipsesTextBox;

    private float statAnimationNumberDuration = 0.5f;
    private int ellipsesCount = 0;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
            return;
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        if(sceneIsLoading){
            loadScreenContentHolder.SetActive(true);
            StartCoroutine(EllipsesAnimationRoutine());
        }
    }

    public void LoadSceneWithLoadScreen( string sceneToLoad )
    {
        sceneIsLoading = true;

        // We already faded to black so now get rid of that for the load screen
        FindObjectOfType<ScreenFade>().SetTransparent();

        // Activate the load screen
        loadScreenContentHolder.SetActive(true);
        InGameUIManager.instance.gameObject.SetActive(false);

        // Load the scene
        SceneManager.LoadScene(sceneToLoad);
    }

    public void RemoveLoadScreen()
    {
        sceneIsLoading = false;
        loadScreenContentHolder.SetActive(false);
        InGameUIManager.instance.gameObject.SetActive(true);
    }

    private IEnumerator EllipsesAnimationRoutine()
    {
        while(sceneIsLoading){
            yield return new WaitForSecondsRealtime(statAnimationNumberDuration);

            ellipsesCount++;

            if(ellipsesCount == 0)
                ellipsesTextBox.text = "";
            else if(ellipsesCount == 1)
                ellipsesTextBox.text = ".";
            else if(ellipsesCount == 2)
                ellipsesTextBox.text = ". .";
            else if(ellipsesCount == 3){
                ellipsesTextBox.text = ". . .";
                ellipsesCount = -1;
            }
        }
    }
}
