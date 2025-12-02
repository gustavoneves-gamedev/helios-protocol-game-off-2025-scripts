using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        
    }

    public GameObject GetObject(GameObject prefab, Transform target)
    {
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
            //Verifica no if se este gameObject já possui um Pool correspondente e, caso não tenha, cria um para ele
        }

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);//verifica se esta chave possui algum valor disponível.
                                    //caso todos os objetos deste tipo estejam em uso, cria um novo e adiciona à chave
                                    //já criada (chave prefab

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        //Pega um valor dentro da chave [prefab]. Na real, vai pegar um clone do respectivo prefab

        objectToGet.transform.parent = null;
        objectToGet.transform.position = target.position;

        objectToGet.SetActive(true);
        
        //Retira ele do parent (gameObject que contém os Pools)

        return objectToGet;


    }

    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn));
        //Chama a Corountine e define um delay para ela
    }


    //Criado para dar a opção de retornar ao Pool após alguns segundos
    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        //causa um delay na coroutine (que é este método DelayReturn)

        ReturnToPool(objectToReturn);
    }


    //Este método devolve o objeto criado ao pool dele!
    private void ReturnToPool(GameObject objectToReturn)
    {
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;
        //Aqui eu estou pegando o original prefab dele para encontrar a chave correta dentro do Dicitonary

        objectToReturn.SetActive(false);
        //Estou desativando este cara antes de levar ele de volta para dentro do GameObject que contém os Pools
        objectToReturn.transform.parent = transform;
        //Estou definindo o pai dele como sendo o GameObject que contém os Pools

        poolDictionary[originalPrefab].Enqueue(objectToReturn);
        //Estou colocando ele de volta no valor associado à chave que peguei na primeira linha deste método

    }



    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();//estou criando uma chave chamada prefab e adicionando o valor
                                                         //Queue<GameObject>() a ela
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
        //Aqui eu estou adicionando uma quantidade de clones baseada no poolSize. Vide CreateNewObject para mais detalhes

    }



    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        //Estou criando um clone do objeto desejado e inserindo ele como child deste gameObject para evitar poluir
        //a Hierarquia

        newObject.AddComponent<PooledObject>().originalPrefab = prefab;
        //Aqui estou adicionando o componente PooledObject e definindo a variável originalPrefab dentro deste novo
        //componente como sendo o prefab passado para esta função. Isso é importante porque quando eu uso o instatiate
        //em cima, eu criei um clone dele, não sendo mais o prefab em si.

        newObject.SetActive(false);
        //Aqui estou apenas definindo o objeto como inativo para não trigar alguma colisão ou ficar flutuando no mapa

        poolDictionary[prefab].Enqueue(newObject);
        //Aqui eu adiciona uma linha ao Dictionary(que é parecido com o Map da Unreal). Como esse Dictionary é composto por
        //uma chave do tipo GameObject associada a um valor também do tipo GameObject, estou pegando a chave do tipo
        //do prefab passado (IPC!! -> Esta chave foi criada no InitializeNewPool!!!) e atribuindo o valor do
        //Clone (newObject) a ela


    }


}
