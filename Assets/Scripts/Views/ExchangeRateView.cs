using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeRateView : MonoBehaviour
{
    public GameObject FavoriteScrollItemPrefab;
    public GameObject AddMoreScrollItemPrefab;

    public InputField Ex_FromInputField;
    public Text Ex_FromCountry, Ex_FromCurrency, Ex_FromSymbol;
    public Image Ex_FromFlag;
    private string Ex_FromCurrencyCode;
    private double Ex_FromExchangeRate;

    private void SetUpExchangeRateScroll(string _countryNameAndRateAndCurrencyCode)
    {
        //try
        //{
        //    RegionInfo m_RegionInfo = new RegionInfo(DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1]);
        //    Ex_FromSymbol.text = m_RegionInfo.CurrencySymbol;
        //}
        //catch (Exception ex)
        //{
        //    Ex_FromSymbol.text = "";
        //    Debug.Log(ex.Message);
        //}

        //Ex_FromCountry.text = _countryNameAndRateAndCurrencyCode.Split(':')[0];
        //Ex_FromCurrency.text = DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][0];
        //Ex_FromFlag.sprite = Resources.Load<Sprite>("CountryFlags/" + DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1].ToLower());

        //Ex_FromCurrencyCode = _countryNameAndRateAndCurrencyCode.Split(':')[2];
        //Ex_FromExchangeRate = Convert.ToDouble(_countryNameAndRateAndCurrencyCode.Split(':')[1]);
        //OnEx_InputFieldEndTyping(ToConvertInputField.text);
    }

    public void OnEx_InputFieldEndTyping(string _currencyAmount)
    {

    }
}
