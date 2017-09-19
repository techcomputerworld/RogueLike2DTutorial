using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{

    public class Loader : MonoBehaviour {

        public GameObject gameManager;
	
        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                Instantiate(gameManager);
            }
        }
    }
}