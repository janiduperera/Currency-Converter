using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System;
using pingak9; // Native Pop Up Plugin
using System.Linq;

public class UIController : Singleton<UIController>
{

    #region Start
    public void InitiateCurrencyData()
    {
        Transform m_ScrollRectContainer = CountryScrollParentRect.Find("Scroll View").gameObject.GetComponent<ScrollRect>().content;
        GameObject m_ScrollItemObj;

        if (m_ScrollRectContainer.childCount == 0)
        {
            foreach (KeyValuePair<string, string[]> pair in DataClass.Instance.CountryCodeDictionary)
            {
                if (DataClass.Instance.CurrencyRatesDictionary.ContainsKey(pair.Value[0]))
                {
                    m_ScrollItemObj = Instantiate(ScrollItemPrefab);
                    string m_CountryNameAndRateAndCurrencyCode = pair.Key + ":" + DataClass.Instance.CurrencyRatesDictionary[pair.Value[0]] + ":" + pair.Value[0];
                    m_ScrollItemObj.name = m_CountryNameAndRateAndCurrencyCode;
                    m_ScrollItemObj.GetComponent<Button>().onClick.AddListener(delegate { OnScrollItemClicked(m_CountryNameAndRateAndCurrencyCode); });
                    m_ScrollItemObj.transform.Find("CountryTxt").gameObject.GetComponent<Text>().text = pair.Key;
                    m_ScrollItemObj.transform.Find("CurrencyTxt").gameObject.GetComponent<Text>().text = pair.Value[0];
                    m_ScrollItemObj.transform.Find("FlagImg").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("CountryFlags/" + pair.Value[1].ToLower());

                    m_ScrollItemObj.transform.SetParent(m_ScrollRectContainer);
                    m_ScrollItemObj.transform.localScale = Vector3.one;

                    if (pair.Key == "France") // Set the initial ToConvert Section
                    {
                        SetToConvertCurrency(m_CountryNameAndRateAndCurrencyCode, true);
                        Set_Exchange_ToConvert(m_CountryNameAndRateAndCurrencyCode, true);
                    }
                    else if (pair.Key == "United Kingdom") // Set the initial Result Section 
                    {
                        SetResultCurrency(m_CountryNameAndRateAndCurrencyCode, true);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < m_ScrollRectContainer.childCount; i++)
            {
                string m_Country_Rate_CurrencyCode = m_ScrollRectContainer.GetChild(i).name;
                m_ScrollRectContainer.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                m_Country_Rate_CurrencyCode = m_Country_Rate_CurrencyCode.Split(':')[0] + ":" + DataClass.Instance.CurrencyRatesDictionary[m_Country_Rate_CurrencyCode.Split(':')[2]] + ":" + m_Country_Rate_CurrencyCode.Split(':')[2];
                m_ScrollRectContainer.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { OnScrollItemClicked(m_Country_Rate_CurrencyCode); });

                if (ToCountryTxt.text == m_Country_Rate_CurrencyCode.Split(':')[0]) // Update To Convert Country Section
                {
                    m_ToCurrencyCode = m_Country_Rate_CurrencyCode.Split(':')[2];
                    m_ToExchangeRate = Convert.ToDouble(m_Country_Rate_CurrencyCode.Split(':')[1]);
                }

                if (ResultCountryTxt.text == m_Country_Rate_CurrencyCode.Split(':')[0]) // Update Result Country Section
                {
                    m_ResultCurrencyCode = m_Country_Rate_CurrencyCode.Split(':')[2];
                    m_ResultExchangeRate = Convert.ToDouble(m_Country_Rate_CurrencyCode.Split(':')[1]);
                }

                if(Ex_FromCountry.text == m_Country_Rate_CurrencyCode.Split(':')[0]) // Update Exchange Rate Panel From Country Section
                {
                    m_Ex_ToCurrencyCode = m_Country_Rate_CurrencyCode.Split(':')[2];
                    m_Ex_ToExchangeRate = Convert.ToDouble(m_Country_Rate_CurrencyCode.Split(':')[1]);
                }
            }
        }

        SetResultCurrencyAmount();
        Set_ExchangeRatesResult_CurrencyAmounts();
    }
    #endregion Start

    #region Tab Buttons
    private Color m_TabActiveColor = Color.black;
    private Color m_TabInActiveColor = new Color(150f/255f, 150f/255f, 150f/255f);
    public Text[] TabButtonTxts;
    public GameObject[] Panels;

    public void OnTabButtonClick(int _index)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == _index)
            {
                Panels[i].SetActive(true);
                TabButtonTxts[i].color = m_TabActiveColor;
            }
            else
            {
                Panels[i].SetActive(false);
                TabButtonTxts[i].color = m_TabInActiveColor;
            }
        }
    }
    #endregion Tab Buttons

    #region Convert Panel
    public Image ToConvertFlagImg, ResultFlagImg;
    public Text ToCurrencyTxt, ToCountryTxt, ToConvertSymbolTxt, ResultCurrencyTxt, ResultCountryTxt, ResultSymbolTxt, DateTxt;
    public InputField ToConvertInputField;
    public Text ResultTxt;
    public Transform CountryScrollParentRect;
    public GameObject ScrollItemPrefab;
    public GameObject ScrollDissmissBtn;
    private bool m_IsConvertButtonClick = false;
    private float[] m_ScrollRectHeightArray = new float[] { 634f, 480f };

    public GameObject ExchangeRateSection;
    private string m_ToCurrencyCode, m_ResultCurrencyCode;
    private double m_ToExchangeRate, m_ResultExchangeRate;

    private void SetToConvertCurrency(string _countryNameAndRateAndCurrencyCode, bool _atStart = false)
    {
        try
        {
            RegionInfo m_RegionInfo = new RegionInfo(DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1]);
            ToConvertSymbolTxt.text = m_RegionInfo.CurrencySymbol;
        }
        catch(Exception ex)
        {
            ToConvertSymbolTxt.text = "";
            Debug.Log(ex.Message);
        }

        ToCountryTxt.text = _countryNameAndRateAndCurrencyCode.Split(':')[0];
        ToCurrencyTxt.text = DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][0];
        ToConvertFlagImg.sprite = Resources.Load<Sprite>("CountryFlags/" + DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1].ToLower());

        m_ToCurrencyCode = _countryNameAndRateAndCurrencyCode.Split(':')[2];
        m_ToExchangeRate = Convert.ToDouble(_countryNameAndRateAndCurrencyCode.Split(':')[1]);

        if (!_atStart)
        {
            SetResultCurrencyAmount();
        }
    }

    private void SetResultCurrency(string _countryNameAndRateAndCurrencyCode, bool _atStart = false)
    {
        try
        {
            RegionInfo m_RegionInfo = new RegionInfo(DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1]);
            ResultSymbolTxt.text = m_RegionInfo.CurrencySymbol;
        }
        catch (Exception ex)
        {
            ResultSymbolTxt.text = "";
            Debug.Log(ex.Message);
        }


        ResultCountryTxt.text = _countryNameAndRateAndCurrencyCode.Split(':')[0];
        ResultCurrencyTxt.text = DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][0];
        ResultFlagImg.sprite = Resources.Load<Sprite>("CountryFlags/" + DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1].ToLower());

        m_ResultCurrencyCode = _countryNameAndRateAndCurrencyCode.Split(':')[2];
        m_ResultExchangeRate = Convert.ToDouble(_countryNameAndRateAndCurrencyCode.Split(':')[1]);

        if (!_atStart)
        {
            SetResultCurrencyAmount();
        }
    }

    private void BringInCountryScroll(bool _IsCalledFromExchangePanel = false)
    {
        ScrollDissmissBtn.SetActive(true);

        if(!_IsCalledFromExchangePanel)
        {
            CountryScrollParentRect.SetParent(Panels[0].transform);
            ScrollDissmissBtn.gameObject.transform.SetParent(Panels[0].transform);
            ScrollDissmissBtn.GetComponent<RectTransform>().SetAsLastSibling();
            if (m_IsConvertButtonClick)
            {
                CountryScrollParentRect.Find("Scroll View").GetComponent<RectTransform>().sizeDelta = new Vector2(CountryScrollParentRect.Find("Scroll View").GetComponent<RectTransform>().sizeDelta.x, m_ScrollRectHeightArray[0]);
            }
            else
            {
                CountryScrollParentRect.Find("Scroll View").GetComponent<RectTransform>().sizeDelta = new Vector2(CountryScrollParentRect.Find("Scroll View").GetComponent<RectTransform>().sizeDelta.x, m_ScrollRectHeightArray[1]);
            }
        }
        else
        {
            CountryScrollParentRect.SetParent(Panels[1].transform);
            ScrollDissmissBtn.gameObject.transform.SetParent(Panels[1].transform);
            ScrollDissmissBtn.GetComponent<RectTransform>().SetAsLastSibling();
            CountryScrollParentRect.Find("Scroll View").GetComponent<RectTransform>().sizeDelta = new Vector2(CountryScrollParentRect.Find("Scroll View").GetComponent<RectTransform>().sizeDelta.x, 710);
        }

        CountryScrollParentRect.SetAsLastSibling();
    }

    private void DismissCountryScroll()
    {
        CountryScrollParentRect.SetAsFirstSibling();
        ScrollDissmissBtn.SetActive(false);
        CountryScrollParentRect.Find("Scroll View").gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }

    public void OnScrollItemClicked(string _countryNameAndRateAndCurrencyCode)
    {
        DismissCountryScroll();

        if (CountryScrollParentRect.parent.gameObject == Panels[0])
        {
            if (m_IsConvertButtonClick)
            {
                SetToConvertCurrency(_countryNameAndRateAndCurrencyCode);
            }
            else
            {
                SetResultCurrency(_countryNameAndRateAndCurrencyCode);
            }
        }
        else
        {
            Set_Exchange_ToConvert(_countryNameAndRateAndCurrencyCode);
        }
    }

    public void OnScrollDismissButtonClicked()
    {
        DismissCountryScroll();
    }

    public void ToConvertButtonClick()
    {
        m_IsConvertButtonClick = true;
        CountryScrollParentRect.position = new Vector2(CountryScrollParentRect.position.x, ToConvertFlagImg.gameObject.transform.parent.position.y);
        BringInCountryScroll();
    }

    public void ResultButtonClick()
    {
        m_IsConvertButtonClick = false;
        CountryScrollParentRect.position = new Vector2(CountryScrollParentRect.position.x, ResultFlagImg.gameObject.transform.parent.position.y);
        BringInCountryScroll();
    }

    public void OnInputFieldEndTyping(string _currencyAmount)
    {
        CurrencyController.Instance.GetCurrencyData("latest");
    }

    private void SetResultCurrencyAmount()
    {
        ResultTxt.text = DataClass.Instance.ConvertedCurrency(Convert.ToDouble(ToConvertInputField.text), m_ToExchangeRate, m_ResultExchangeRate);

        SetExchangeRate();
    }

    public void SetDate(double _date)
    {
        DateTxt.text = DataClass.Instance.ConvertFromUnixTimestamp(_date);
    }

    public void SetExchangeRate()
    {
        ExchangeRateSection.SetActive(true);
        ExchangeRateSection.transform.Find("ExchangeRateTxt").gameObject.GetComponent<Text>().text = "1 <size=15>" + m_ToCurrencyCode + "</size>  =>  " + DataClass.Instance.FormatCurrencyToDecimal(m_ResultExchangeRate / m_ToExchangeRate, 4) + " <size=15>" + m_ResultCurrencyCode + "</size>";
    }
    #endregion Convert Panel

    #region Exchange Rate Panel
    public InputField Ex_FromInputField;
    public Image Ex_FromFlag;
    public Text Ex_FromCountry, Ex_FromCurrency, Ex_FromSymbol;
    private string m_Ex_ToCurrencyCode;
    private double m_Ex_ToExchangeRate;
    public Transform Ex_FavoriteContent;
    public GameObject Ex_FavoriteScrollItemPrefab;
    public GameObject Ex_AddMoreScrollItemPrefab;

    private void Set_Exchange_ToConvert(string _countryNameAndRateAndCurrencyCode, bool _atStart = false)
    {
        try
        {
            RegionInfo m_RegionInfo = new RegionInfo(DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1]);
            Ex_FromSymbol.text = m_RegionInfo.CurrencySymbol;
        }
        catch (Exception ex)
        {
            Ex_FromSymbol.text = "";
            Debug.Log(ex.Message);
        }

        Ex_FromCountry.text = _countryNameAndRateAndCurrencyCode.Split(':')[0];
        Ex_FromCurrency.text = DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][0];
        Ex_FromFlag.sprite = Resources.Load<Sprite>("CountryFlags/" + DataClass.Instance.CountryCodeDictionary[_countryNameAndRateAndCurrencyCode.Split(':')[0]][1].ToLower());

        m_Ex_ToCurrencyCode = _countryNameAndRateAndCurrencyCode.Split(':')[2];
        m_Ex_ToExchangeRate = Convert.ToDouble(_countryNameAndRateAndCurrencyCode.Split(':')[1]);

        if (!_atStart)
        {
            Set_ExchangeRatesResult_CurrencyAmounts();
        }
    }

    private void Set_ExchangeRatesResult_CurrencyAmounts()
    {
        double m_TempExchangeRate;
        if (Ex_FavoriteContent.childCount == 0)
        {
            GameObject m_FavoriteScrollItem;

            foreach(string _country in DataClass.Instance.DefaultFavoritCountryList)
            {
                m_FavoriteScrollItem = Instantiate(Ex_FavoriteScrollItemPrefab);
                m_TempExchangeRate = Convert.ToDouble(DataClass.Instance.CurrencyRatesDictionary[DataClass.Instance.CountryCodeDictionary[_country][0]]);
                m_FavoriteScrollItem.name = _country + ":" + m_TempExchangeRate + ":" + DataClass.Instance.CountryCodeDictionary[_country][0];
                //m_ScrollItemObj.GetComponent<Button>().onClick.AddListener(delegate { OnScrollItemClicked(m_CountryNameAndRateAndCurrencyCode); });
                m_FavoriteScrollItem.transform.Find("Button").Find("CountryTxt").gameObject.GetComponent<Text>().text = _country;
                m_FavoriteScrollItem.transform.Find("Button").Find("CurrencyTxt").gameObject.GetComponent<Text>().text = DataClass.Instance.CountryCodeDictionary[_country][0];
                m_FavoriteScrollItem.transform.Find("Button").Find("FlagImg").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("CountryFlags/" + DataClass.Instance.CountryCodeDictionary[_country][1].ToLower());

                try
                {
                    RegionInfo m_RegionInfo = new RegionInfo(DataClass.Instance.CountryCodeDictionary[_country][1]);
                    m_FavoriteScrollItem.transform.Find("SymbolTxt").gameObject.GetComponent<Text>().text = m_RegionInfo.CurrencySymbol;
                }
                catch (Exception ex)
                {
                    m_FavoriteScrollItem.transform.Find("SymbolTxt").gameObject.GetComponent<Text>().text = "";
                    Debug.Log(ex.Message);
                }

                m_FavoriteScrollItem.transform.Find("ExchangeRateTxt").gameObject.GetComponent<Text>().text = DataClass.Instance.ConvertedCurrency(Convert.ToDouble(Ex_FromInputField.text), m_Ex_ToExchangeRate, m_TempExchangeRate);

                m_FavoriteScrollItem.transform.SetParent(Ex_FavoriteContent);
                m_FavoriteScrollItem.transform.localScale = Vector3.one;
            }
        }
        else
        {
            for(int i = 0; i < Ex_FavoriteContent.childCount; i++)
            {
                string m_Country_Rate_CurrencyCode = Ex_FavoriteContent.GetChild(i).name;
                m_TempExchangeRate = Convert.ToDouble(DataClass.Instance.CurrencyRatesDictionary[DataClass.Instance.CountryCodeDictionary[m_Country_Rate_CurrencyCode.Split(':')[0]][0]]);
                Ex_FavoriteContent.GetChild(i).name = m_Country_Rate_CurrencyCode.Split(':')[0] + ":" + m_TempExchangeRate + ":" + DataClass.Instance.CountryCodeDictionary[m_Country_Rate_CurrencyCode.Split(':')[0]][0];
                Ex_FavoriteContent.GetChild(i).Find("ExchangeRateTxt").gameObject.GetComponent<Text>().text = DataClass.Instance.ConvertedCurrency(Convert.ToDouble(Ex_FromInputField.text), m_Ex_ToExchangeRate, m_TempExchangeRate);
            }
        }
    }

    public void Ex_FromButtonClick()
    {
        m_IsConvertButtonClick = true;
        CountryScrollParentRect.position = new Vector2(CountryScrollParentRect.position.x, Ex_FromFlag.gameObject.transform.parent.position.y);
        BringInCountryScroll(true);
    }

    public void OnExchangeInputFieldEnd(string _currencyAmount)
    {
        CurrencyController.Instance.GetCurrencyData("latest");
    }
    #endregion Exchange Rate Panel

    #region Loading Panel
    public GameObject LoadingPanel;
    public void BringInLoadingPanel()
    {
        LoadingPanel.SetActive(true);
    }

    public void DismissLoadingPanel()
    {
        LoadingPanel.SetActive(false);
    }
    #endregion Loading Panel

    #region Graph
    public GameObject GraphObj;
    private RectTransform[] m_Points = new RectTransform[7];
    private RectTransform[] m_Lines = new RectTransform[6];
    private Text[] m_DaysTxt = new Text[7];
    private float m_GraphStartX = -200;
    private float m_GraphEndX = 200;
    private float m_GraphStartY = -120;
    private float m_GraphEndY = 120;
    private float m_MaxExchangeValue;

    /// <summary>
    /// Shows the graph.
    /// </summary>
    /// <param name="_isTesting">Is used because my current subscription does not support the Time series API call <c>true</c> is testing.</param>
    private void ShowGraph(bool _isTesting = true)
    {
        GraphObj.SetActive(true);
        if (m_Points[0] == null)
        {
            int m_TempIndex = 0;
            for(int i = 2; i < 9; i++)
            {
                m_Points[m_TempIndex] = GraphObj.transform.GetChild(i).GetComponent<RectTransform>();
                m_TempIndex++;
            }

            m_TempIndex = 0;
            for (int i = 9; i < 15; i++)
            {
                m_Lines[m_TempIndex] = GraphObj.transform.GetChild(i).GetComponent<RectTransform>();
                m_TempIndex++;
            }

            m_TempIndex = 0;
            for (int i = 15; i < 22; i++)
            {
                m_DaysTxt[m_TempIndex] = GraphObj.transform.GetChild(i).GetComponent<Text>();
                m_TempIndex++;
            }
        }

        float[] m_ExchangeRateValues = new float[m_Points.Length];

        if(_isTesting)
        {
            float m_CurrentDateExchangeRate = (float)(m_ResultExchangeRate / m_ToExchangeRate);

            // Since we don't have the API data fluctuates the current exchange rate between less 1, and plus 15
            for (int i = 0; i < m_Points.Length; i++)
            {
                m_ExchangeRateValues[i]= m_CurrentDateExchangeRate + UnityEngine.Random.Range(1f, 15f);
            }
        }
        else // Live data
        {

        }

        m_MaxExchangeValue = m_ExchangeRateValues.Max();


        // Draw Points
        for (int i = 0; i < m_Points.Length; i++)
        {
            DrawPoints(i, m_ExchangeRateValues[i]);
        }

        //Draw Lines
        for(int i = 0; i < m_Lines.Length ; i++)
        {
            DrawLine(i, m_Points[i], m_Points[i + 1]);
        }
    }

    private void DrawPoints(int _pointIndex, float _value)
    {
        float m_XGap = (m_GraphEndX - m_GraphStartX) / 7; // 7 is becuase we have 7 days to draw. 
      
        if(_value < 0)
        {
            _value = 0;
        }

        float m_YPos = m_GraphStartY + (((m_GraphEndY - m_GraphStartY) / m_MaxExchangeValue) * _value);

        m_Points[_pointIndex].transform.localPosition = new Vector2(m_GraphStartX + _pointIndex * m_XGap, m_YPos);

        m_DaysTxt[_pointIndex].text = DataClass.Instance.CurrentDate.AddDays(-6 + _pointIndex).ToString("dd/MM");
    }

    private void DrawLine(int _lineIndex, RectTransform _inPositionFrom, RectTransform _inPositionTo)
    {
        float dist = Vector2.Distance(_inPositionFrom.localPosition, _inPositionTo.localPosition);
        float angle = Vector3.Angle(_inPositionTo.localPosition - _inPositionFrom.localPosition, Vector2.right);

        if (_inPositionFrom.localPosition.y > _inPositionTo.localPosition.y)
        {
            angle *= -1;
        }

        // m_Lines[_lineIndex].GetComponent<Image>().color = HexToColor(tradingColour);
        m_Lines[_lineIndex].localPosition = _inPositionFrom.localPosition;
        m_Lines[_lineIndex].sizeDelta = new Vector2(dist, 3);
        m_Lines[_lineIndex].localEulerAngles = new Vector3(0f, 0f, angle);

    }

    public void ShowGraphButton(Transform _arrowTransform)
    {
        //If true, the graph will be shown
        if(Math.Abs(_arrowTransform.localScale.y - -1) < Mathf.Epsilon)  
        {
            _arrowTransform.localScale = new Vector3(1, 1, 1);
            _arrowTransform.parent.Find("Text").gameObject.GetComponent<Text>().text = "Hide past week";
            ShowGraph();
        }
        else 
        {
            _arrowTransform.localScale = new Vector3(1, -1, 1);
            _arrowTransform.parent.Find("Text").gameObject.GetComponent<Text>().text = "Show past week";
            GraphObj.SetActive(false);
        }
    }
    #endregion Graph

    #region Native Pop Ups Implementation 
    public void BringInPopUpPanel(string _heading, string _message)
    {
        NativeDialog.OpenDialog(_heading, _message, "Ok",
            () => {
                Debug.Log("OK Button pressed");
            });
    }
    #endregion Native Pop Ups Implementation
}
