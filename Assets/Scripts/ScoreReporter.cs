using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreReporter : MonoBehaviour
{
    // URL base del servidor (p. ex. http://127.0.0.1:3000)
    [SerializeField] private string apiBaseUrl = "http://127.0.0.1:3000";
    // Ruta concreta del recurs on fem el POST
    [SerializeField] private string classificationPath = "/api/classification";
    [SerializeField] private string ratingPath = "/api/rateGame";
    [SerializeField] private string verifyPath = "/api/verify";

    [Serializable]
    private class Score
    {
        // Nom del jugador/a
        public string name;
        // Temps sobreviscut (en segons)
        public int  puntuacion;
        // Nombre de salts
        public string api_token;
    }

    [Serializable]
    public class ScoreEntry
    {
        public string name;
        public int puntuacion;
    }

    [Serializable]
    private class ScoreList
    {
        public ScoreEntry[] data;
    }
    
    [Serializable]
    private class Rating
    {
        public string name;
        public string api_token;
        public string email;
        public int general;
        public int jugabilitat;
        public int dificultat;
        public int grafics;
        public int concordancia;
    }
    [Serializable]
    private class User
    {
        public string name;
        public string api_token;
        public string email;
    }
    
    //----------------------- M?tode p?blic per enviar la puntuaci?
    public void SubmitScore(string playerName, int  puntuaciones, string tokken)
    {
        // Empaquetem les dades en l'objecte que serialitzarem a JSON
        var payload = new Score
        {
            api_token = tokken,
            name = playerName,
            puntuacion = puntuaciones
        };

        // Llancem la corutina que fa la petici? HTTP
        StartCoroutine(PostScoreCoroutine(payload));
    }

    public void GetClassification(string token, int top, System.Action<ScoreEntry[]> onSuccess)
    {
        StartCoroutine(GetClassificationCoroutine(token, top, onSuccess));
    }

    private IEnumerator GetClassificationCoroutine(string token,int top, System.Action<ScoreEntry[]> onSuccess)
    {
        string url = apiBaseUrl.TrimEnd('/') + $"/api/classification/{token}/{top}";
        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();
            
            bool isHttpSuccess = req.responseCode >= 200 && req.responseCode < 300;
            if (req.result == UnityWebRequest.Result.Success || isHttpSuccess)
            {
                ScoreList result = JsonUtility.FromJson<ScoreList>(req.downloadHandler.text);
                onSuccess?.Invoke(result.data);
            }
            else
            {
                Debug.LogWarning($"Error GET Clasificación: {req.result} {req.responseCode} {req.error}" );
            }
        }
    }
    //------------------------- Metodos para enviar raiting
    
    public void SubmitRating(string playerName,string mail, string tokken, int rGeneral,int rJugabilitat, int rDificultat, int rGrafics, int rConcordancia)
    {
        var payload = new Rating
        {
            api_token = tokken,
            email = mail,
            name = playerName,
            general = rGeneral,
            jugabilitat = rJugabilitat,
            dificultat = rDificultat,
            grafics = rGrafics,
            concordancia = rConcordancia
        };

        StartCoroutine(PostRatingCoroutine(payload));
    }

    private IEnumerator PostRatingCoroutine(Rating payload)
    {
       string url = apiBaseUrl.TrimEnd('/') +  ratingPath;
       
       string json = JsonUtility.ToJson(payload);

       using (var req = UnityWebRequest.Put(url, json))
       {
           req.method = UnityWebRequest.kHttpVerbPOST;
           req.SetRequestHeader("Content-Type", "application/json");
           req.timeout = 10;
           
           Debug.Log(req.uri);
           Debug.Log(req.url);
           Debug.Log(url);
           
           yield return req.SendWebRequest();
           
           bool isHttpSuccess = req.responseCode >= 200 && req.responseCode < 300;
           if (req.result == UnityWebRequest.Result.Success || isHttpSuccess)
           {
               Debug.Log("Puntuacio enviada OK: " + req.downloadHandler.text);
           }
           else
           {
               Debug.LogWarning(
                   $"Error enviant puntuacio: Result={req.result}, Code={req.responseCode}, Error={req.error}\n{req.downloadHandler.text}"
               );
           }
       }
    }
    
    
    // ------------------Corutina que construeix i envia la petici? POST
    private IEnumerator PostScoreCoroutine(Score payload)
    {
        // Assegurem que no quedin dobles '/' entre base i path
        string url = apiBaseUrl.TrimEnd('/') + classificationPath;

        // Serialitzem l'objecte a JSON
        string json = JsonUtility.ToJson(payload);
        // UnityWebRequest.Put crea un request amb cos; canviem el m?tode a POST
        using (var req = UnityWebRequest.Put(url, json))
        {
            // Forcem POST perqu? la nostra API espera aquest m?tode
            req.method = UnityWebRequest.kHttpVerbPOST;
            // Indiquem el tipus de contingut del cos
            req.SetRequestHeader("Content-Type", "application/json");
            // Temps m?xim d'espera (segons)
            req.timeout = 10; // segundos

            Debug.Log(req.uri);
            Debug.Log(req.url);
            Debug.Log(url);
            
            
            // Enviem la petici? i esperem la resposta
            yield return req.SendWebRequest();

            // Considerem ?xit qualsevol codi 2xx
            bool isHttpSuccess = req.responseCode >= 200 && req.responseCode < 300;
            if (req.result == UnityWebRequest.Result.Success || isHttpSuccess)
            {
                // Resposta correcta
                Debug.Log("Puntuacio enviada OK: " + req.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning(
                    $"Error enviant puntuacio: Result={req.result}, Code={req.responseCode}, Error={req.error}\n{req.downloadHandler.text}"
                );
                // Aqu? pots implementar reintents o desar-ho offline per enviar-ho despr?s
            }
        }
    }
    //------------------------- Metodos para CRER USARIO
    public void SubmitUser(string playerName, string  email, string tokken)
    {
        // Empaquetem les dades en l'objecte que serialitzarem a JSON
        var payload = new User
        {
            api_token = tokken,
            name = playerName,
            email = email
        };

        // Llancem la corutina que fa la petici? HTTP
        StartCoroutine(PostUserCoroutine(payload));
    }
    private IEnumerator PostUserCoroutine(User payload)
    {
        // Assegurem que no quedin dobles '/' entre base i path
        string url = apiBaseUrl.TrimEnd('/') + verifyPath;

        // Serialitzem l'objecte a JSON
        string json = JsonUtility.ToJson(payload);
        // UnityWebRequest.Put crea un request amb cos; canviem el m?tode a POST
        using (var req = UnityWebRequest.Put(url, json))
        {
            // Forcem POST perqu? la nostra API espera aquest m?tode
            req.method = UnityWebRequest.kHttpVerbPOST;
            // Indiquem el tipus de contingut del cos
            req.SetRequestHeader("Content-Type", "application/json");
            // Temps m?xim d'espera (segons)
            req.timeout = 10; // segundos

            Debug.Log(req.uri);
            Debug.Log(req.url);
            Debug.Log(url);
            
            
            // Enviem la petici? i esperem la resposta
            yield return req.SendWebRequest();

            // Considerem ?xit qualsevol codi 2xx
            bool isHttpSuccess = req.responseCode >= 200 && req.responseCode < 300;
            if (req.result == UnityWebRequest.Result.Success || isHttpSuccess)
            {
                // Resposta correcta
                Debug.Log("User creado con exito: " + req.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning(
                    $"Error enviant puntuacio: Result={req.result}, Code={req.responseCode}, Error={req.error}\n{req.downloadHandler.text}"
                );
                // Aqu? pots implementar reintents o desar-ho offline per enviar-ho despr?s
            }
        }
    }
}