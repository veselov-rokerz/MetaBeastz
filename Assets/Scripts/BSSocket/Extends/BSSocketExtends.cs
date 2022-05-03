using System.Text;
using UnityEngine;

namespace Assets.Scripts.BSSocket.Extends
{
    public static class BSSocketExtends
    {
        public static string ToJson<T>(this T source)
        {
            return JsonUtility.ToJson(source);
        }

        public static byte[] ToByte(this string source)
        {
            return Encoding.UTF8.GetBytes(source);
        }

        public static T ToObject<T>(this string source)
        {
            return JsonUtility.FromJson<T>(source);
        }
    }
}