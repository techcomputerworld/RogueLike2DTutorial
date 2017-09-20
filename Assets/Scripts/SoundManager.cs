using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{

    public class SoundManager : MonoBehaviour
    {
        public AudioSource musicSource;
        public AudioSource sfxSource;
        /* En un juego en el que vamos a reproducir el mismo sonido, cada vez que movamos al jugador se va a mover, se movera con un tono mas agudo o mas grave
         * 
         */
        public static SoundManager instance;
        public float lowPitchRange = 0.95f;
        public float highPitchRange = 1.05f;
        private void Awake()
        {
            if (SoundManager.instance == null)
            {
                //instanciamos en la variable estatica SoundManager la clase esta entera 
                SoundManager.instance = this;
            }
            else if(SoundManager.instance != this)
            {
                //destruimos la clase que se instancia en el caso de que ya exista 
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        
        public void PlaySingle(AudioClip clip)
        {
            sfxSource.pitch = 1f;
            sfxSource.clip = clip;
            sfxSource.Play();

            /**/
            
        }

        /* Para poder pasarle los clip o sonidos que queramos mediante un */
        public void RandomizeSfx(params AudioClip[] clips)
        {
            //cada vez que vamos a reproducir suene un tono aleatorio
            int randomIndex = Random.Range(0, clips.Length);
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            sfxSource.pitch = randomPitch;
            sfxSource.clip = clips[randomIndex];
            sfxSource.Play();
        }
    }
}