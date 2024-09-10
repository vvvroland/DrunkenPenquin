using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Allows me to use text
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 400.0f;
    public float jumpforce = 12.0f;
    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;
    public Text scoreText;
    private int score;
    public AudioClip jumpsound;
    public AudioClip diamondsound;
    public AudioClip trianglesound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        // score = 0;
        PlayerPrefs.SetInt("CurrentScore", 0);
        score = PlayerPrefs.GetInt("CurrentScore");
        scoreText.text = "Score: " + score.ToString();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //side to side arrow key input
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(moveX, _body.velocity.y);
        _body.velocity = movement;
        //jumping
        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        //checking for collisions of objects. Checking corners.
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        bool grounded = false;
        if (hit != null)
        {
            grounded = true;
        }
        if (grounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            _body.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            audioSource.clip = jumpsound;
            audioSource.Play();
        }
        //Passing speed to animator to trigger animations
        _anim.SetFloat("speed", Mathf.Abs(moveX));
        //Prevent moonwalk
        if (!Mathf.Approximately(moveX, 0))
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX) * .5f, .5f, .5f);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            score += 10;
            PlayerPrefs.SetInt("CurrentScore", score);
            scoreText.text = "Score: " + score.ToString();
            audioSource.clip = diamondsound;
            audioSource.Play();
        }
        if (other.tag == "NextLevel")
        {
            audioSource.clip = trianglesound;
            audioSource.Play();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

}
