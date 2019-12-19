using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class CurrencyController : Singleton<CurrencyController>
{
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("CurrencyConverter", 0) == 0) // If this is the first time the app is used, We are saving the local Currency Json file to persistant data path. 
        {
            TextAsset m_tempTextAsset = Resources.Load("latest") as TextAsset;
            DataClass.Instance.WriteToLocalJsonFile(m_tempTextAsset.text);
            PlayerPrefs.SetInt("CurrencyConverter", 1);
        }

        UIController.Instance.OnTabButtonClick(0);
        InitiateCountries();
        GetCurrencyData("latest");
    }

    private void OnApplicationPause(bool _pause)
    {
        if(_pause)
        {
            DataClass.Instance.SaveFavoriteCurrency();
        }
        else
        {
            DataClass.Instance.ReadFavoriteCurrency();
        }
    }

    private void OnApplicationQuit()
    {
        DataClass.Instance.SaveFavoriteCurrency();
    }

    #region API Calling

    //---- Latest Data
    public void GetCurrencyData(string _date)
    {
        UIController.Instance.BringInLoadingPanel();
        APIConnection.Instance.GetCurrencyData(_date, APICallSucceed, APICallFailed);
    }

    private void APICallSucceed(IDictionary _currencyData)
    {
        if (_currencyData["success"]+"" == "True")
        {
            SetCurrencyRates(_currencyData);
        }
        else
        {
            Dictionary<string, object> m_ErrorObj = _currencyData["error"] as Dictionary<string, object>;
            UIController.Instance.BringInPopUpPanel("Error", m_ErrorObj["info"] + "");

            // Use the offline data
            SetCurrencyRates((IDictionary)Json.Deserialize(DataClass.Instance.LatestCurrency));
        }

        UIController.Instance.DismissLoadingPanel();
    }

   
    private void APICallFailed(string _error)
    {
        UIController.Instance.BringInPopUpPanel("Error", _error);

        // Use the offline data
        SetCurrencyRates((IDictionary)Json.Deserialize(DataClass.Instance.LatestCurrency));

        UIController.Instance.DismissLoadingPanel();
    }

    //---- Latest Data

    
    // ---- Past Week Data
    public void GetCurrencyDataForPastWeek(string _startDate, string _endDate, string _fromCurrencyCode, string _toCurrencyCode)
    {
        //UIController.Instance.BringInLoadingPanel();
        //APIConnection.Instance.GetPastWeekData(_startDate, _endDate, _fromCurrencyCode, _toCurrencyCode, PastWeekAPICallSuccess, PastWeekAPICallFail);

        //TODO : This API is currently not supported for my subscription. Therefore, DrawTest graph for now. 
        //When the API supported, Erase the DrawTest graph funtion and Uncomment the above

    }

    private void PastWeekAPICallSuccess(IDictionary _currencyData)
    {
        if (_currencyData["success"] + "" == "True")
        {
        }
        else
        {

        }

        UIController.Instance.DismissLoadingPanel();
    }


    private void PastWeekAPICallFail(string _error)
    {
        UIController.Instance.DismissLoadingPanel();
    }

    // ---- Past Week Data

    private void SetCurrencyRates(IDictionary _currencyData)
    {
        DataClass.Instance.CurrencyRatesDictionary = _currencyData["rates"] as Dictionary<string, object>;
        UIController.Instance.SetDate(System.Convert.ToDouble(_currencyData["timestamp"]));
        UIController.Instance.InitiateCurrencyData();
    }
    #endregion API Calling

    #region Countries
    private void InitiateCountries()
    {
        // Read two character country codes from the file. 
        TextAsset m_TextFileContryCode = Resources.Load("ISO 3166 country codes") as TextAsset;
        string[] m_CountryCodesLines = m_TextFileContryCode.text.Split('\n');
        Dictionary<string, string> m_TempCountryCodeDic = new Dictionary<string, string>();

        foreach (string _line in m_CountryCodesLines)
        {
            string[] m_SplitCountryCode = _line.Split('|');
            m_TempCountryCodeDic.Add(m_SplitCountryCode[0].Trim(), m_SplitCountryCode[1].Trim());
        }

        // Read currency code from the file. 
        TextAsset m_TextFileCurrency = Resources.Load("ISO 4217 currency symbol") as TextAsset;

        string[] m_CurrencyLines = m_TextFileCurrency.text.Split('\n');

        foreach (string _line in m_CurrencyLines)
        {
            string[] m_SplitCurrencyLines = _line.Split('|');

            string m_CountryCode = "";
            if(m_TempCountryCodeDic.TryGetValue(m_SplitCurrencyLines[0].Trim(), out m_CountryCode))
            {
                // Set Key = Country, Value = Currency Code & Country Code
                DataClass.Instance.SetCountryCodesDictionary(m_SplitCurrencyLines[0].Trim(), new string[] { m_SplitCurrencyLines[1].Trim(), m_CountryCode });
            }
            else
            {
                // Set Key = Country, Value = Currency Code & Country Code
                DataClass.Instance.SetCountryCodesDictionary(m_SplitCurrencyLines[0].Trim(), new string[] { m_SplitCurrencyLines[1].Trim(), "no-flag" });
            }
        }
    }
    #endregion Countries
}
