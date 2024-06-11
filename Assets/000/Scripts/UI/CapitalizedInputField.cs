using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magus.UserInterface
{
    public class CapitalizedInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        public void OnValueChanged(string newValue)
        {
            inputField.text = newValue.ToUpper();
        }
    }
}
