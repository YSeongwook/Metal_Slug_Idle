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
        OnClickManualGPGSSignIn,
        OnClickEmailSignIn,
    }

    public enum DataEvents
    {
        OnUserDataSave,
        OnUserDataLoad,
        OnUserDataReset,
    }

    public enum GoogleEvents
    {
        GPGSSignIn,
        ManualGPGSSignIn,
    }

    public enum FirebaseEvents
    {
        FirebaseInitialized,
        FirebaseDatabaseInitialized,
        FirebaseSignIn,
        EmailSignIn,
    }

    public class EnumTypes : MonoBehaviour { }
}