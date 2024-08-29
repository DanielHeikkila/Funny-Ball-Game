using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class main : MonoBehaviour
{
    #region Fields
    public GameObject Player;
    public GameObject DeathWall;
    public Camera Camera;
    public GameObject[] Circles;
    public GameObject[] Stars;
    public GameObject Blade;
    public GameObject Triangle;
    public int n;
    public Material O;
    public Material G;
    public Material B;
    public Material P;
    public float score;
    public bool gameOver;
    public ParticleSystem death;
    public ParticleSystem star;
    private AudioSource boingClip;
    private AudioSource deathClip;   
    private AudioSource starClip;
    private AudioSource colorClip;
    public AudioClip boing;
    public AudioClip deathC;
    public AudioClip starC;
    public AudioClip colorC;
    public TextMeshProUGUI promptText;
    public Image overlayImage;
    public TextMeshProUGUI scoreText;
    public bool increasing;
    #endregion

    #region Start
    // Start is called before the first frame update
    void Start()
    {
        increasing = true;
        boingClip = GetComponent<AudioSource>();
        boingClip.clip = boing;
        score = 0;
        gameOver = false;
        //Assign starting color
        ChangeColor(Player);
        overlayImage.color = new Color(0, 0, 0, 0);
        InvokeRepeating("MoveObjects", 0.01f, 0.01f);
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        n ++;
        //This is jumping mechanics
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //Restart the game if lcicked after death or win
            if (gameOver == true)
            {
                score = -1;
                gameOver = false;
                Application.LoadLevel(Application.loadedLevel);
            }
            Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector3(0,850,0);
            boingClip.Play();
        }
        DeathWall.transform.position = new Vector3(0, Camera.transform.position.y - 523.2f, 0);  

        //Camera follows player UP only
        if (Player.transform.position.y > Camera.transform.position.y)
        {
            Camera.transform.position = new Vector3(0, Player.transform.position.y, -10);
        }

    }
    #endregion

    #region Triggers
    public void OnTriggerEnter2D(Collider2D other)
    {
        //This is the death trigger, it works by material
        if (this.gameObject.GetComponent<Renderer>().material.color != other.gameObject.GetComponent<Renderer>().material.color && other.gameObject.CompareTag("Respawn") || this.gameObject.GetComponent<Renderer>().material.color != other.gameObject.GetComponent<Renderer>().material.color && other.gameObject.CompareTag("OtherWay"))
        {
            //I honestly could not explain my thought process for attaching the death state specifically to the spinning triangle
            Triangle.GetComponent<main>().gameOver = true;
            Destroy(Player);
            //Death animation
            Instantiate(death, this.gameObject.transform.position, Quaternion.identity);
            //Sound
            deathClip = Triangle.GetComponent<AudioSource>();
            deathClip.clip = Triangle.GetComponent<main>().deathC;
            deathClip.Play();
            //Death screen
            overlayImage.color = new Color(0, 0, 0, 0.5f);
            promptText.text = "GAME OVER \r\nClick to restart";
        }
        //Star collection trigger
        else if (other.gameObject.CompareTag("Star"))
        {
            score += 0.5f;
            //Star pick up animation
            Instantiate(star, other.gameObject.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            //Star pick up sound, it is tied to the spinny blade because I have deadlines
            starClip = Blade.GetComponent<AudioSource>();
            starClip.clip = Blade.GetComponent<main>().starC;
            starClip.Play();
            Invoke("Destroy(star)", 2);
            scoreText.text = "0" + score;
        }
        //Color switch trigger
        else if (other.gameObject.CompareTag("ColorSwitcher"))
        {
            ChangeColor(Player);
            Destroy(other.gameObject);
            //Color switch sound, yet again I tied it to a random object, do not ask why, I am in a hurry
            colorClip = DeathWall.GetComponent<AudioSource>();
            colorClip.clip = DeathWall.GetComponent<main>().colorC;
            colorClip.Play();
        }
        //Win trigger
        else if (other.gameObject.CompareTag("Finish"))
        {
            //Yet again, I honestly could not explain my thought process for attaching the death state specifically to the spinning triangle
            Triangle.GetComponent<main>().gameOver = true;
            gameOver = true;
            Destroy(Player);
            //Player blows up on win
            Instantiate(death, this.gameObject.transform.position, Quaternion.identity);
            //Win screen
            overlayImage.color = new Color(0, 0, 0, 0.5f);
            promptText.text = "YOU WIN!!! \r\nClick to restart";
        }
    }
    #endregion

    #region Color
    void ChangeColor(GameObject target)//This randomizes color
    {
        SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
        if (targetRenderer != null)
        {
            float f = Random.value;
            if (f < 0.25 && targetRenderer.material != G)
            {
                targetRenderer.material = G;
            }
            else if (f < 0.5 && targetRenderer.material != O)
            {
                targetRenderer.material = O;
            }
            //Here it makes sure its never pink before the triangle because triangle has no pink side
            else if (f < 0.75 && score != 2 && targetRenderer.material != P)
            {
                targetRenderer.material = P;
            }
            else if (targetRenderer.material != B)
            {
                targetRenderer.material = B;
            }
        }
    }
    #endregion

    #region Moving parts
    public void MoveObjects()
    {
        //Circles spin
        foreach (GameObject Circle in Circles)
            Circle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, n / 5));
        //Blade spin
        Blade.transform.rotation = Quaternion.Euler(new Vector3(0, 0, n / -10));
        //Guess what spins...
        Triangle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, n / -5));

        //Stars pulsate
        foreach (GameObject star in Stars)
        {
            try
            {
                if (star.gameObject.transform.localScale.x <= 13)
                {
                    increasing = true;
                }
                else if (star.gameObject.transform.localScale.x >= 20)
                {
                    increasing = false;
                }

                if (!increasing)
                {
                    star.gameObject.transform.localScale = new Vector3(star.gameObject.transform.localScale.x - 0.1f, star.gameObject.transform.localScale.y - 0.1f, 1);
                }
                else
                {
                    star.gameObject.transform.localScale = new Vector3(star.gameObject.transform.localScale.x + 0.1f, star.gameObject.transform.localScale.y + 0.1f, 1);
                }
            }
            catch
            {

            }
        }
    }
    #endregion

    //NOTES: Should probably separate into several scripts Ex: movement scripts for objects, player + collision script, sounds,
}
