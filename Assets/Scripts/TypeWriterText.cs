using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TypeWriterText : MonoBehaviour
{
    Text txt;
    string fullText;
    [Tooltip("How fast the typewriter produces text")]
    [SerializeField]
    float textspeed = .02f;
    [Tooltip("This is here to let people know when the typewriter is done")]
    public bool isDone = true;



    public void ChangeText(string _text, float _delay = 0f)
    {
        isDone = false;
        txt = GetComponent<Text>();
        StopCoroutine(PlayText()); //stop Coroutime if exist
        fullText = _text;
        txt.text = ""; //clean text
        Invoke("Start_PlayText", _delay); //Invoke effect
    }

    void Start_PlayText()
    {
        StartCoroutine(PlayText());
    }
    IEnumerator PlayText()
    {
        foreach (char c in fullText)
        {
            txt.text += c;
            yield return new WaitForSeconds(textspeed);
        }
        isDone = true;
    }
}
