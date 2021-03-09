using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(LoadIntroLevel());
    }

    

    IEnumerator LoadIntroLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.Instance.LoadScene("Intro");
    }
}
