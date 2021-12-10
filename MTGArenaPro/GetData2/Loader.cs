using UnityEngine;

namespace GetData2
{
    public class Loader
    {
        private static GameObject gameObject;

        public static void Load()
        {
            //Debug.Log("TEST!!!!! Load()");

            if (GameObject.Find("MTGAProDataGetter") == null)
            {
                gameObject = new GameObject("MTGAProDataGetter");
                gameObject.AddComponent<MTGAProGetData>();
                Object.DontDestroyOnLoad(gameObject);
                gameObject.SetActive(true);

                //Debug.Log("TEST!!!!! Load() completed");
            }
            else
            {
                Debug.Log("[MTGA.Pro Logger] Logger is already in place, no need to embed it again!");
            }
        }

        public static void Unload()
        {
            Object.Destroy(gameObject);
        }
    }
}