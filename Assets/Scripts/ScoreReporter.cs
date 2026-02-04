using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreReporter : MonoBehaviour
{
    // URL base del servidor (p. ex. http://127.0.0.1:3000)
    [SerializeField] private string apiBaseUrl = "http://127.0.0.1:3000";
    // Ruta concreta del recurs on fem el POST
    [SerializeField] private string submitPath = "/scores";

    [Serializable]
    private class Score
    {
        // Nom del jugador/a
        public string player;
        // Temps sobreviscut (en segons)
        public float timeSurvived;
        // Nombre de salts
        public string token;
    }

    // M?tode p?blic per enviar la puntuaci?
    public void SubmitScore(string playerName, float timeSeconds, string tokken)
    {
        // Empaquetem les dades en l'objecte que serialitzarem a JSON
        var payload = new Score
        {
            player = playerName,
            timeSurvived = timeSeconds,
            token = tokken,
        };

        // Llancem la corutina que fa la petici? HTTP
        StartCoroutine(PostScoreCoroutine(payload));
    }

    // Corutina que construeix i envia la petici? POST
    private IEnumerator PostScoreCoroutine(Score payload)
    {
        // Assegurem que no quedin dobles '/' entre base i path
        string url = apiBaseUrl.TrimEnd('/') + submitPath;

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
                // Informaci? d'error per depuraci?
                Debug.LogWarning(
                    $"Error enviant puntuacio: Result={req.result}, Code={req.responseCode}, Error={req.error}\n{req.downloadHandler.text}"
                );
                // Aqu? pots implementar reintents o desar-ho offline per enviar-ho despr?s
            }
        }
    }
}