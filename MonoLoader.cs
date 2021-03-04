using UnityEngine;

namespace Duranda
{
    public class MonoLoader
    {
        public static void Init()
        {
            Load = new GameObject();
            Component = Load.AddComponent<Duranda>();
            Object.DontDestroyOnLoad(Load);
        }

        public static void Dispose()
        {
            Component.Console.WriteLine("Unloading... Bye!");
            Object.Destroy(Load);
        }

        private static GameObject Load;
        private static Duranda Component;
    }
}