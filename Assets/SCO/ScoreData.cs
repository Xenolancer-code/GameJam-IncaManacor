using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Scriptable Objects/ScoreData")]
public class ScoreData : ScriptableObject
{
    public string name = "";
    public string email = "";
    public int general=5;
    public int jugabilitat=5;
    public int dificultat=5;
    public int grafics=5;
    public int concordancia=5;
    public string api_token = "x7bF74UvGCnfoACoQHTXZOkVaPJy6aavaxexzw91B4EZPDZMSbHN70L9IUpT";
}
