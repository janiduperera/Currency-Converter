using System.Collections;
using MiniJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class APIConnection : Singleton<APIConnection>
{
    #region API Communication
    private string m_Date;

    /// <summary>
    /// This method is used to begin sending request process.
    /// </summary>
    /// <param name="_url">URL.</param>
    /// <param name="_callbackOnSuccess">Callback on success.</param>
    /// <param name="_callbackOnFail">Callback on fail.</param>
    /// <typeparam name="IDictionary">The 1st type parameter.</typeparam>
    private void SendAPIRequest<IDictionary>(string _url, UnityAction<IDictionary> _callbackOnSuccess, UnityAction<string> _callbackOnFail)
    {
        //StartCoroutine(RequestCoroutine(_url, _callbackOnSuccess, _callbackOnFail));

        //TODO: Remove 
        _callbackOnSuccess?.Invoke((IDictionary)Json.Deserialize(Test.Instance.jsonTxt.text));
    }

    /// <summary>
    /// Coroutine that handles communication with the server.
    /// </summary>
    /// <returns>The coroutine.</returns>
    /// <param name="_url">API url.</param>
    /// <param name="_callbackOnSuccess">Callback on success.</param>
    /// <param name="_callbackOnFail">Callback on fail.</param>
    /// <typeparam name="IDictionary">The 1st type parameter.</typeparam>
    private IEnumerator RequestCoroutine<IDictionary>(string _url, UnityAction<IDictionary> _callbackOnSuccess, UnityAction<string> _callbackOnFail)
    {
        UnityWebRequest m_WWWWebRequest = UnityWebRequest.Get(_url);
        yield return m_WWWWebRequest.SendWebRequest();

        if (m_WWWWebRequest.isNetworkError || m_WWWWebRequest.isHttpError)
        {
            Debug.LogError(m_WWWWebRequest.error);
            _callbackOnFail?.Invoke(m_WWWWebRequest.error);
        }
        else
        {
            Debug.Log(m_WWWWebRequest.downloadHandler.text);
            _callbackOnSuccess?.Invoke((IDictionary)Json.Deserialize(m_WWWWebRequest.downloadHandler.text));
            if(m_Date == "latest") // We update all the latest currency data in to our local file to be used for offline conversions. 
            {
                DataClass.Instance.WriteToLocalJsonFile(m_WWWWebRequest.downloadHandler.text);
            }
        }
    }
    #endregion API Communication

    #region API Calling
    /// <summary>
    /// Call the API to get the Currency Data.
    /// </summary>
    /// <param name="_Date">Date.</param>
    /// <param name="callbackOnSuccess">Callback on success.</param>
    /// <param name="callbackOnFail">Callback on fail.</param>
    public void GetCurrencyData(string _Date, UnityAction<IDictionary> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        m_Date = _Date;
        SendAPIRequest(DataClass.Instance.GetAPI_ENDPOINT+ _Date+ "?access_key=" + DataClass.Instance.GetAPI_KEY, callbackOnSuccess, callbackOnFail);
    }

    /// <summary>
    /// Gets the past week data. 
    /// </summary>
    /// <param name="_startDate">Start date.</param>
    /// <param name="_endDate">End date.</param>
    /// <param name="callbackOnSuccess">Callback on success.</param>
    /// <param name="callbackOnFail">Callback on fail.</param>
    public void GetPastWeekData(string _startDate, string _endDate, string _fromCurrencyCode, string _toCurrencyCode, UnityAction<IDictionary> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        string m_URL = DataClass.Instance.GetAPI_ENDPOINT + "timeseries?access_key=" + DataClass.Instance.GetAPI_KEY + "&base="+ _fromCurrencyCode + "&start_date=" + _startDate + "&end_date=" + _endDate + "&symbols=" + _toCurrencyCode + "";
        SendAPIRequest(m_URL, callbackOnSuccess, callbackOnFail);
    }
    #endregion API Calling
}
