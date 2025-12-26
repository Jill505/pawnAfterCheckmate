namespace AnkrhaDebugLogSystem
{
    using UnityEngine;

    public class AK_DebugManager : MonoBehaviour
    {
        public void throwError(string error)
        {
            switch (error)
            {
                case "":
                    break;

                default:
                    Debug.Log("AK Error - ¥¼ª¾ªº¿ù»~");
                    break;
            }
        }
    }
}