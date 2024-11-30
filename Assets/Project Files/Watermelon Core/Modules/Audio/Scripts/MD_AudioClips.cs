using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Audio Clips", menuName = "Data/Core/Audio Clips")]
    public class AudioClips : ScriptableObject
    {
        [BoxGroup("UI", "UI")]
        public AudioClip buttonSound;

        [BoxGroup("Gameplay", "Gameplay")]
        public AudioClip winSound;
        [BoxGroup("Gameplay")]
        public AudioClip loseSound;
        [BoxGroup("Gameplay")]
        public AudioClip coinSound;
        [BoxGroup("Gameplay")]
        public AudioClip powerupSound;
        [BoxGroup("Gameplay")]
        public AudioClip assembleSound;
    }
}

// -----------------
// Audio Controller v 0.4
// -----------------