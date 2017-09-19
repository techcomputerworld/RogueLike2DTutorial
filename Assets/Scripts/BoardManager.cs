using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{
    public class BoardManager : MonoBehaviour
    {

        //area en la que se mueve el personaje
        // numero de columnas
        public int columns = 8;
        // numero de filas
        public int rows = 8;

        public GameObject[] floorTiles, outerWallTiles, wallTiles, foodTiles, enemyTiles;
        public GameObject exit;

        private Transform boardHolder;

        //lista de las posiciones del grid
        private List<Vector2> gridPositions = new List<Vector2>();

        //con este metodo lo que hacemos es delimitar la zona 6x6, donde se van a introducir objetos food, enemy, wall. 
        void InitializeList()
        {
            gridPositions.Clear();
            for (int x = 1; x < columns - 1; x++)
            {
                for (int y = 1; y < rows - 1; y++)
                {
                    gridPositions.Add(new Vector2(x, y));

                }
            }

        }
        Vector2 RandomPosition()
        {
            //Indice con el que vamos a trabajar en la lista
            int randomIndex = Random.Range(0, gridPositions.Count);
            //aqui obtenemos la posicion de la lista gridPositions
            Vector2 randomPosition = gridPositions[randomIndex];
            //eliminamos de la lista ese objeto y asi no apareceran objetos encima de otro 
            gridPositions.RemoveAt(randomIndex);
            return randomPosition;

        }
        /* Le pasamos el array que queremos que nos genere aleatoriamente, un numero minimo de objetos y un maximo, y generara aleatoriamente 
         * entre esos 2 numeros los objetos aleatorios.
        */
        void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max)
        {
            int objectCount = Random.Range(min, max + 1);
            for (int i = 0; i < objectCount; i++)
            {
                // randomPosition devuelve la posicion de la losa
                Vector2 randomPosition = RandomPosition();
                //tileChoice devuelve el GameObject de la losa de forma aleatoria
                GameObject tileChoice = GetRandomInArray(tileArray);
                //instancia la losa tileChoice aleatoria, la posicion aleatoria donde se va a poner y su 
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }
        //dependiendo del numero de Nivel habra mas o menos enemigos
        public void SetupScene(int level)
        {
            BoardSetup();
            InitializeList();
            LayoutObjectAtRandom(wallTiles, 5, 9);
            LayoutObjectAtRandom(foodTiles, 1, 5);
            /* La forma de generar los enemigos no me convence imagino que el razonamiento es que por cada nivel haya mas enemigos pero yo lo he pensado de otra forma
             * poder generarlos tal vez de otra forma como nivel 1 2 enemigos, nivel 2 entre 3 y 4 enemigos algo asi. 
             */
            int enemyCount = (int)Mathf.Log(level, 2);
            // la otra opcion es 
            //int enemyCount = level / 2;
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
            Instantiate(exit, new Vector2(columns - 1, rows - 1), Quaternion.identity);
        }
        private void BoardSetup()
        {
            //Parece interesante poner los objetos cada uno dentro de un objeto padre.
            // no permitir que se instancie mas de unavez que no se porque ocurre 

            boardHolder = new GameObject("Board").transform;
            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    //una forma mas eficiente es meter esto dentro del if el GamewObject este que esta aqui suelto.
                    GameObject toInstantiate;

                    //comprobar si la posicion x e y pertenece al borde para no usar el suelo y usar el muro 
                    if (x == -1 || y == -1 || x == columns || y == rows)
                    {
                        toInstantiate = GetRandomInArray(outerWallTiles);
                    }
                    else
                    {
                        toInstantiate = GetRandomInArray(floorTiles);
                    }
                    //referencia al prefabs que estemos instanciando
                    GameObject instance = Instantiate(toInstantiate, new Vector2(x, y), Quaternion.identity);
                    instance.transform.SetParent(boardHolder);

                }
            }

        }
        //crearemosun metodo para instanciar tanto las losas como los muros en la escena
        GameObject GetRandomInArray(GameObject[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }
}
