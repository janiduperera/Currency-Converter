using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataClass : Singleton<DataClass>
{
    protected DataClass() { }

    #region Server Config
    private const string API_KEY = "b6ac216dd6e4c8b6bae4266b507ec5af";
    public string GetAPI_KEY
    {
        get { return API_KEY; }
    }

    private const string API_ENDPOINT = "http://data.fixer.io/api/";
    public string GetAPI_ENDPOINT
    {
        get { return API_ENDPOINT; }
    }
    #endregion Server Config

    #region Country Codes
    private Dictionary<string, string[]> m_CountryCodeDictionary = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> CountryCodeDictionary
    {
        get { return m_CountryCodeDictionary; }
    }

    public void SetCountryCodesDictionary(string _key, string[] _value)
    {
        m_CountryCodeDictionary.Add(_key, _value);
    }

    private List<string> m_DefaultFavoritCountryList = new List<string> { "United States", "United Kingdom", "India", "Australia", "Canada", "Singapore", "Japan", "China" };
    public List<string> DefaultFavoritCountryList
    {
        get { return m_DefaultFavoritCountryList; }
    }
    public void AddToDefaultFavoritCountries(string _country)
    {
        m_DefaultFavoritCountryList.Add(_country);
    }
    #endregion Country Codes

    #region Currency Rates
    private DateTime m_CurrentDate;
    public DateTime CurrentDate
    {
        get { return m_CurrentDate; }
    }

    private string m_LatestCurrency; // This is for offline currency data
    public string LatestCurrency
    {
        get { return m_LatestCurrency; }
    }

    private Dictionary<string, object> m_CurrencyRatesDictionary;
    public Dictionary<string, object> CurrencyRatesDictionary
    {
        get { return m_CurrencyRatesDictionary; }
        set { m_CurrencyRatesDictionary = value; }
    }
    #endregion Currency Rates

    #region Calculations
    public string FormatCurrencyToDecimal(double _currencyVal)
    {
        return string.Format("{0:N2}", (Math.Truncate(_currencyVal * 100) / 100));
    }

    public string FormatCurrencyToDecimal(double _currencyVal, int _decimalPlaces)
    {
        return (Math.Truncate(_currencyVal * Math.Pow(10, _decimalPlaces)) / Math.Pow(10, _decimalPlaces))+"";
    }

    public string ConvertedCurrency(double _enteredAmount, double _toExchangeRate, double _resultExchangeRate)
    {
        double m_EnteredEuroAmout = (1.0 / _toExchangeRate) * _enteredAmount; // Covert the amount to base Currency first

        return FormatCurrencyToDecimal(m_EnteredEuroAmout * _resultExchangeRate);
    }

    public string ConvertFromUnixTimestamp(double _timestamp)
    {
        m_CurrentDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        m_CurrentDate = m_CurrentDate.AddSeconds(_timestamp).ToLocalTime();
        return m_CurrentDate.ToString("MMMM dd, yyyy hh:mm tt");
    }
    #endregion Calculations

    #region Write to local Json data file
    public void WriteToLocalJsonFile(string _json)
    {
        FileStream m_Fs = new FileStream(UnityEngine.Application.persistentDataPath + "/latest.json", FileMode.Create);

        using (StreamWriter writer = new StreamWriter(m_Fs))
        {
            writer.Write(_json);
        }

        m_LatestCurrency = _json;
    }
    #endregion Write to local Json data file
}
