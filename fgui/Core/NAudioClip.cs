using System;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class NAudioClip
    {
        public static Action<AudioStream> CustomDestroyMethod;

        /// <summary>
        /// 
        /// </summary>
        public DestroyMethod destroyMethod;

        /// <summary>
        /// 
        /// </summary>
        public AudioStream nativeClip;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioClip"></param>
        public NAudioClip(AudioStream audioClip)
        {
            nativeClip = audioClip;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unload()
        {
            if (nativeClip == null)
                return;
            nativeClip = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioClip"></param>
        public void Reload(AudioStream audioClip)
        {
            if (nativeClip != null && nativeClip != audioClip)
                Unload();

            nativeClip = audioClip;
        }

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeOnLoad()
        {
            CustomDestroyMethod = null;
        }
#endif
    }
}
