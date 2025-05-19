using UnityEngine;

namespace EEA.Utils
{
    static public class AccountInfo
    {
        static public string Token => _token;
        static private string _token;
        static public void SetToken(string token)
        {
            _token = token;
        }
    }


}
