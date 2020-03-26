using UnityEngine;
using System.Collections;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;

namespace Completed
{

	public class Player : MovingObject
	{
				
					
		public int wallDamage = 1;
		public int pointsPerFood = 10;
		public int pointsPerSoda = 20;
		public float restartLevelDelay = 1f;						
		
		private Animator animator;					
		private int food;

		public Text foodText;
		public AudioClip moveSound1;
		public AudioClip moveSound2;
		public AudioClip eatSound1;
		public AudioClip eatSound2;
		public AudioClip drinkSound1;
		public AudioClip drinkSound2;
		public AudioClip gameOverSound;



		
		protected override void Start ()
		{
			
			animator = GetComponent<Animator>();			
			food = GameManager.instance.playerFoodPoints;			
			foodText.text = "Food: " + food;			
			base.Start ();
		}
		
		
	
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerFoodPoints = food;
		}
		
		
		private void Update ()
		{
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;
			
			int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;		//Used to store the vertical move direction.
			
			//Check if we are running either in the Unity editor or in a standalone build.

			
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}

			if(horizontal != 0 || vertical != 0)
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
				AttemptMove<Wall> (horizontal, vertical);
			}
		}
		
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
		
			food--;					
			foodText.text = "Food: " + food;			
			base.AttemptMove <T> (xDir, yDir);
			
			
			RaycastHit2D hit;		
					
			CheckIfGameOver ();			
			GameManager.instance.playersTurn = false;

			if (Move(xDir, yDir, out hit))
			{
				SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
			}
		}
		
		
		
		protected override void OnCantMove <T> (T component)
		{
			
			Wall hitWall = component as Wall;			
			hitWall.DamageWall (wallDamage);			
			animator.SetTrigger ("playerChop");
		}
		
		
	
		private void OnTriggerEnter2D (Collider2D other)
		{
			
			if(other.tag == "Exit")
			{
				
				Invoke ("Restart", restartLevelDelay);				
				enabled = false;
			}			
			else if(other.tag == "Food")
			{				
				food += pointsPerFood;					
				foodText.text = "+" + pointsPerFood + " food: " + food;
				SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);				
				other.gameObject.SetActive (false);
			}						
			else if(other.tag == "Soda")
			{
			
				food += pointsPerSoda;				
				foodText.text = "+" + pointsPerSoda + " food: " + food;
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				other.gameObject.SetActive (false);
			}
		}
		
		
		private void Restart ()
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		public void LoseFood (int loss)
		{
			animator.SetTrigger ("playerHit");
			food -= loss;
			foodText.text = "-"+ loss + " food: " + food;
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			
			if (food <= 0) 
			{
				SoundManager.instance.PlaySingle (gameOverSound);
				SoundManager.instance.musicSource.Stop();			
				GameManager.instance.GameOver ();
			}
		}
	}
}

