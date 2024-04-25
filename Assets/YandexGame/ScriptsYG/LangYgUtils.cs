using UnityEngine.UIElements;
using YG;

namespace DefaultNamespace
{
    public static class LangYgUtils
    {
        public static string Translate(string ru, string en)
        {
            return YandexGame.lang == "ru" ? ru : en;
        }
    }
}