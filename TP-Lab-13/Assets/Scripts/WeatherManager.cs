using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class WeatherManager : MonoBehaviour
{
    public enum City
    {
        Orlando,
        Sydney,
        London,
        Moscow,
        Stockholm
    }
    [SerializeField] public City city;
    private string xmlApi;

    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"network problem: {request.error}");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"response error: {request.responseCode}");
            }
            else
            {
                callback(request.downloadHandler.text);
            }
        }
    }
    void Start()
    {
        xmlApi = "http://api.openweathermap.org/data/2.5/weather?q=" + city + ",us&mode=xml&appid=3deb980fa62762c977d52625281e64ce";
        StartCoroutine(GetWeatherXML(OnXMLDataLoaded));
        
    }

    public class WeatherData
    {
        XmlSerializer serializer = new XmlSerializer(typeof(WeatherData));

    }
public void OnXMLDataLoaded(string data)
    {
        Debug.Log(data);
        XDocument doc = XDocument.Parse(data);

        foreach (XElement current in doc.Descendants("current"))
        {
            string temperature = current.Element("temperature")?.Attribute("value")?.Value;
            string humidity = current.Element("humidity")?.Attribute("value")?.Value;
            string pressure = current.Element("pressure")?.Attribute("value")?.Value;
            string windSpeed = current.Element("wind")?.Element("speed")?.Attribute("value")?.Value;
            string windDirection = current.Element("wind")?.Element("direction")?.Attribute("name")?.Value;
            string clouds = current.Element("clouds")?.Attribute("name")?.Value;
            string weather = current.Element("weather")?.Attribute("number")?.Value;
            string weatherIcon = current.Element("weather")?.Attribute("icon")?.Value;
            Debug.Log($"Temperature: {temperature} K");
            Debug.Log($"Humidity: {humidity} %");
            Debug.Log($"Pressure: {pressure} hPa");
            Debug.Log($"Wind Speed: {windSpeed} m/s");
            Debug.Log($"Wind Direction: {windDirection}");
            Debug.Log($"Clouds: {clouds}");
            Debug.Log($"Weather Number: {weather}");
            Debug.Log($"Weather Icon: {weatherIcon}");
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApi, callback);
    }

}

