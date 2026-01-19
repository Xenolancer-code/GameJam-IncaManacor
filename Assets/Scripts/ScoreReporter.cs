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
        public int kills;
    }

    // Mètode públic per enviar la puntuació
    public void SubmitScore(string playerName, float timeSeconds, int kills)
    {
        // Empaquetem les dades en l'objecte que serialitzarem a JSON
        var payload = new Score
        {
            player = playerName,
            timeSurvived = timeSeconds,
            kills = kills,
        };

        // Llancem la corutina que fa la petició HTTP
        StartCoroutine(PostScoreCoroutine(payload));
    }

    // Corutina que construeix i envia la petició POST
    private IEnumerator PostScoreCoroutine(Score payload)
    {
        // Assegurem que no quedin dobles '/' entre base i path
        string url = apiBaseUrl.TrimEnd('/') + submitPath;

        // Serialitzem l'objecte a JSON
        string json = JsonUtility.ToJson(payload);

        // UnityWebRequest.Put crea un request amb cos; canviem el mètode a POST
        using (var req = UnityWebRequest.Put(url, json))
        {
            // Forcem POST perquè la nostra API espera aquest mètode
            req.method = UnityWebRequest.kHttpVerbPOST;
            // Indiquem el tipus de contingut del cos
            req.SetRequestHeader("Content-Type", "application/json");
            // Temps màxim d'espera (segons)
            req.timeout = 10; // segundos

            // Enviem la petició i esperem la resposta
            yield return req.SendWebRequest();

            // Considerem èxit qualsevol codi 2xx
            bool isHttpSuccess = req.responseCode >= 200 && req.responseCode < 300;
            if (req.result == UnityWebRequest.Result.Success || isHttpSuccess)
            {
                // Resposta correcta
                Debug.Log("Puntuacio enviada OK: " + req.downloadHandler.text);
            }
            else
            {
                // Informació d'error per depuració
                Debug.LogWarning(
                    $"Error enviant puntuacio: Result={req.result}, Code={req.responseCode}, Error={req.error}\n{req.downloadHandler.text}"
                );
                // Aquí pots implementar reintents o desar-ho offline per enviar-ho després
            }
        }
    }
}