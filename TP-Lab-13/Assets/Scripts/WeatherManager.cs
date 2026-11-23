using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

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

    [SerializeField] private Material[] skyboxes;

    private Dictionary<string, Material> skyboxDict;
    private string weatherIconID;

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
        InitializeDictionary();

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
            weatherIconID = current.Element("weather")?.Attribute("icon")?.Value;
            Debug.Log($"Temperature: {temperature} K");
            Debug.Log($"Humidity: {humidity} %");
            Debug.Log($"Pressure: {pressure} hPa");
            Debug.Log($"Wind Speed: {windSpeed} m/s");
            Debug.Log($"Wind Direction: {windDirection}");
            Debug.Log($"Clouds: {clouds}");
            Debug.Log($"Weather Number: {weather}");
            Debug.Log($"Weather Icon: {weatherIconID}");
        }

        if (skyboxDict.TryGetValue(weatherIconID, out Material skybox)) ChangeSkybox(skybox);
        else Debug.LogWarning($"Key '{weatherIconID}' not found in skybox dictionary.");
    }

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApi, callback);
    }

    // populates dictionary with the icon id string and corresponding skybox material    
    private void InitializeDictionary()
    {
        skyboxDict = new Dictionary<string, Material>
        {
            { "01d", skyboxes[0] },
            { "01n", skyboxes[1] },
            { "02d", skyboxes[2] },
            { "02n", skyboxes[3] },
            { "03d", skyboxes[4] },
            { "03n", skyboxes[5] },
            { "04d", skyboxes[6] },
            { "04n", skyboxes[7] },
            { "09d", skyboxes[8] },
            { "09n", skyboxes[9] },
            { "10d", skyboxes[10] },
            { "10n", skyboxes[11] },
            { "11d", skyboxes[12] },
            { "11n", skyboxes[13] },
            { "13d", skyboxes[14] },
            { "13n", skyboxes[15] },
            { "50d", skyboxes[16] },
            { "50n", skyboxes[17] },
        };
    }

    // changes the skybox material based on the array value provided
    private void ChangeSkybox(Material newSkybox)
    {
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;

            // update lighting & reflections to match new skybox
            DynamicGI.UpdateEnvironment();
        }
    }
}

