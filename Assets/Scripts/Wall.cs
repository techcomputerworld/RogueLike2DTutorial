using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{
    public class Wall : MonoBehaviour
    {

        public Sprite dmgSprite;
        public int hp = 4;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void DamageWall(int loss)
        {
            spriteRenderer.sprite = dmgSprite;
            hp -= loss;
            if (hp <= 0)
            {

                /* es posible que lo hayan hecho para no destruir objetos durante la partida y que asi no se ejecute el recolector de basura, que causaria una perdida
                 * de rendimiento de la misma.
                 * Destroy(gameObject) 
                 */
                gameObject.SetActive(false);
            }
        }
    }
}
