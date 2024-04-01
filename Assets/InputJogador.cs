using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputJogador : MonoBehaviour
{
    private TMPro.TMP_InputField textField;
    private TMPro.TMP_Dropdown dropdown;
    void Start()
    {
        textField = GetComponentInChildren<TMPro.TMP_InputField>();
        dropdown = GetComponent<TMPro.TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownChange);

        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData(""));
        foreach (var item in System.IO.Directory.GetFiles(Relatorio.getFolderRelatorio()))
        {
            string[] partesAddr = item.Replace('\\','/').Split('/');
            string nome = partesAddr[partesAddr.Length - 1].Split('.')[0];
            
            dropdown.options.Add(new TMP_Dropdown.OptionData(nome));
        }
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    private void OnDropdownChange(int index)
    {
        textField.text = dropdown.options[index].text;
        print(dropdown.options[index].text);
    }

    public string valor()
    {
        return textField.text;
    }
}
