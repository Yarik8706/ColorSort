using System;
using System.Net;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using YG;

public class TranslateText : MonoBehaviour
{
    [SerializeField] private YandexGame yandexGame;
    private TMP_Text _text;

    public string ruText;
    public string enText;
    public string trText;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        YandexGame.SwitchLangEvent += SwitchLanguage;
        SwitchLanguage(YandexGame.savesData.language);
    }
    
    public void TranslateTR()
    {
        trText = TranslateGoogle("tr", ruText);
    }

    private void SwitchLanguage(string obj)
    {
        SetText(obj);
    }

    private void SetText(string language)
    {
        _text.text = language switch
        {
            "ru" => ruText,
            "en" => enText,
            "tr" => trText,
            _ => ruText
        };
    }

    [ContextMenu(nameof(TranslateSomeText))]
    public void TranslateSomeText()
    {
        _text = GetComponent<TMP_Text>();
        ruText = _text.text;
        enText = TranslateGoogle("en", ruText);
        trText = TranslateGoogle("tr", ruText);
    }

    private string TranslateGoogle(string translationTo, string text)
    {
        var url = String.Format(
            "http://translate.google." + yandexGame.infoYG.domainAutoLocalization +
            "/translate_a/single?client=gtx&dt=t&sl={0}&tl={1}&q={2}",
            "auto", translationTo, WebUtility.UrlEncode(text));
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
        while (!www.isDone)
        {
        }

        string response = www.downloadHandler.text;

        try
        {
            JArray jsonArray = JArray.Parse(response);
            response = jsonArray[0][0][0].ToString();
        }
        catch
        {
            response = "process error";
            StopAllCoroutines();

            Debug.LogError(
                "(ru���������) ������� �� ��������! ��������, �� ������ ������� ����� ��������. � ����� ������, API Google Translate ��������� ������ � �������� �� ��������� �����.  ����������, ���������� �����. �� ���������� ����� ������� �����, ����� Google �� �������� ���� �������� �� ����" +
                "\n" +
                "(enMessage) The process is not completed! Most likely, you made too many requests. In this case, the Google Translate API blocks access to the translation for a while.  Please try again later. Do not translate the text too often, so that Google does not consider your actions as spam");
        }

        return response;
    }
}