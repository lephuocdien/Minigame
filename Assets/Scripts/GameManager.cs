using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

namespace Completed
{
	using System.Collections.Generic;		
	using UnityEngine.UI;					
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay =0.5f;						
		public float turnDelay = 0.1f;							
		public int playerFoodPoints = 100;	
		
		public static GameManager instance = null;	
		
		[HideInInspector] public bool playersTurn = true;		
		
		private int level = 1;
		private Text levelTextt;		
		//
		private GameObject levelImage;							
		private BoardManager boardScript;						
			
		//enemy
		private List<Enemy> enemies;							
		private bool enemiesMoving;	
		
		private bool doingSetup = true;							
		
		
		
		
		void Awake()
		{
           
            if (instance == null)               
                instance = this;          
            else if (instance != this)               
                Destroy(gameObject);			
		
			DontDestroyOnLoad(gameObject);			
			enemies = new List<Enemy>();
			boardScript = GetComponent<BoardManager>();
			InitGame();
		}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            instance.InitGame();
        }

		
		//Initializes the game for each level.
		void InitGame()
		{

			doingSetup = true;
			//ui
			levelImage = GameObject.Find("LevelImage");
			levelTextt = GameObject.Find("LevelText").GetComponent<Text>();
			levelImage.SetActive(true);
			levelTextt.text = "Day " + level;
			//
			
			Invoke("HideLevelImage", 0.5f);
			enemies.Clear();

			boardScript.SetupScene(level);


		}	
		
		
		void HideLevelImage()
		{		
			levelImage.SetActive(false);
			doingSetup = false;
		}

	
		void Update()
		{
			
			if(playersTurn || enemiesMoving || doingSetup)				
				return;			
			StartCoroutine (MoveEnemies ());
		}
		
		
		public void AddEnemyToList(Enemy script)
		{			
			enemies.Add(script);
		}
		
		

	
		public void GameOver()
		{			
			levelTextt.text = "You survive in :" +level +"days";			
			levelImage.SetActive(true);						
			enabled = false;
		}
		

		IEnumerator MoveEnemies()
		{
			
			enemiesMoving = true;		
			yield return new WaitForSeconds(turnDelay);
			if (enemies.Count == 0) 
			{
				yield return new WaitForSeconds(turnDelay);
			}
			for (int i = 0; i < enemies.Count; i++)
			{				
				enemies[i].MoveEnemy ();	
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			playersTurn = true;
			enemiesMoving = false;
		}
	}
}

