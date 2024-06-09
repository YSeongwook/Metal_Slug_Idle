using UnityEngine;

namespace EnumTypes
{
    public enum LookDirection { Up, Down, Left, Right }
    public enum PlayerState { Stand, Move, Run, Hold, Cook }
    public enum Layers
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Reserved1,
        Water,
        UI,
        Reserved2,
        Reserved3,
        Player,
        World,
    }

    public enum UIEvents
    {
        OnClickSignInGoogle,
        OnClickStart,
    }

    public enum DataEvents
    {
        OnUserDataSave,
        OnUserDataLoad,
        OnUserDataReset,
    }

    public enum FirebaseEvents
    {
        FirebaseInitialized,
        FirebaseLoggedIn,
    }

    public class EnumTypes : MonoBehaviour { }
}