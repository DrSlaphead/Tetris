using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

  // reference to our board
  Board m_gameBoard;

  // reference to our spawner 
  Spawner m_spawner;

  SoundManager m_soundManager;

  ScoreManager m_scoreManager;

  // currently active shape
  Shape m_activeShape;
  // ghost for visualization
  Ghost m_ghost;

  public float m_dropInterval = 0.1f;
  private float m_dropIntervalModded;
  float m_timeToDrop;

  float m_timeToNextKeyLeftRight;

  [Range ( 0.02f , 1f )]
  public float m_keyRepeatRateLeftRight = 0.25f;

  float m_timeToNextKeyDown;

  [Range ( 0.01f , 0.5f )]
  public float m_keyRepeatRateDown = 0.01f;

  float m_timeToNextKeyRotate;

  [Range ( 0.02f , 1f )]
  public float m_keyRepeatRateRotate = 0.25f;

  public GameObject m_gameOverPanel;

  bool m_gameOver = false;

  public IconToggle m_rotIconToggle;

  bool m_clockwise = true;

  public bool m_isPaused = false;

  public GameObject m_pausePanel;



  // Use this for initialization
  void Start()
  {

    // find spawner and board with GameObject.FindWithTag plus GetComponent; make sure you tag your objects correctly
    //m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
    //m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

    // find spawner and board with generic version of GameObject.FindObjectOfType, slower but less typing
    m_gameBoard = GameObject.FindObjectOfType<Board> ();
    m_spawner = GameObject.FindObjectOfType<Spawner> ();
    m_soundManager = GameObject.FindObjectOfType<SoundManager> ();
    m_scoreManager = GameObject.FindObjectOfType<ScoreManager> ();
    m_ghost = GameObject.FindObjectOfType<Ghost> ();

    m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
    m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
    m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

    if ( !m_gameBoard )
    {
      Debug.LogWarning ( "WARNING!  There is no game board defined!" );
    }

    if ( !m_soundManager )
    {
      Debug.LogWarning ( "WARNING!  There is no sound manager defined!" );
    }
    if ( !m_scoreManager )
    {
      Debug.LogError ( "ERROR! no score manager" );
    }
    if ( !m_spawner )
    {
      Debug.LogWarning ( "WARNING!  There is no spawner defined!" );
    }
    else
    {
      m_spawner.transform.position = Vectorf.Round ( m_spawner.transform.position );

      if ( !m_activeShape )
      {
        m_activeShape = m_spawner.SpawnShape ();
      }
    }

    if ( m_gameOverPanel )
    {
      m_gameOverPanel.SetActive ( false );
    }

    if ( m_pausePanel )
    {
      m_pausePanel.SetActive ( false );
    }
    m_dropIntervalModded = m_dropInterval;
  }

  // Update is called once per frame
  private void Update()
  {
    // if we are missing a spawner or game board or active shape, then we don't do anything
    if ( !m_spawner || !m_gameBoard || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager )
    {
      return;
    }

    PlayerInput ();
  }

  private void LateUpdate()
  {
    if(m_ghost)
    {
      m_ghost.DrawGhost ( m_activeShape , m_gameBoard );
    }
  }

  void PlayerInput()
  {
    // example of NOT using the Input Manager
    //if (Input.GetKey ("right") && (Time.time > m_timeToNextKey) || Input.GetKeyDown (KeyCode.RightArrow)) 

    if (( Input.GetButton ( "MoveRight" ) && ( Time.time > m_timeToNextKeyLeftRight )) || Input.GetButtonDown ( "MoveRight" ) )
    {
      m_activeShape.MoveRight ();
      m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

      if ( !m_gameBoard.IsValidPosition ( m_activeShape ) )
      {
        m_activeShape.MoveLeft ();
        PlaySound ( m_soundManager.m_errorSound , 0.5f );
      }
      else
      {
        PlaySound ( m_soundManager.m_moveSound , 0.5f );

      }

    }
    else if (( Input.GetButton ( "MoveLeft" ) && ( Time.time > m_timeToNextKeyLeftRight )) || Input.GetButtonDown ( "MoveLeft" ) )
    {
      m_activeShape.MoveLeft ();
      m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

      if ( !m_gameBoard.IsValidPosition ( m_activeShape ) )
      {
        m_activeShape.MoveRight ();
        PlaySound ( m_soundManager.m_errorSound , 0.5f );
      }
      else
      {
        PlaySound ( m_soundManager.m_moveSound , 0.5f );

      }

    }
    else if ( Input.GetButtonDown ( "Rotate" ) && ( Time.time > m_timeToNextKeyRotate ) )
    {
      //m_activeShape.RotateRight();
      m_activeShape.RotateClockwise ( m_clockwise );

      m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

      if ( !m_gameBoard.IsValidPosition ( m_activeShape ) )
      {
        //m_activeShape.RotateLeft();
        m_activeShape.RotateClockwise ( !m_clockwise );

        PlaySound ( m_soundManager.m_errorSound , 0.5f );
      }
      else
      {
        PlaySound ( m_soundManager.m_moveSound , 0.5f );

      }

    }

    else if (( Input.GetButton ( "MoveDown" ) && ( Time.time > m_timeToNextKeyDown )) || ( Time.time > m_timeToDrop ) )
    {
      m_timeToDrop = Time.time + m_dropIntervalModded;
      m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

      m_activeShape.MoveDown ();

      if ( !m_gameBoard.IsValidPosition ( m_activeShape ) )
      {
        if ( m_gameBoard.IsOverLimit ( m_activeShape ) )
        {
          GameOver ();
        }
        else
        {
          LandShape ();
        }
      }

    }
    else if ( Input.GetButtonDown ( "ToggleRot" ) )
    {
      ToggleRotDirection ();
    }
    else if ( Input.GetButtonDown ( "Pause" ) )
    {
      TogglePause ();
    }
  }

  // shape lands
  void LandShape()
  {
    // move the shape up, store it in the Board's grid array
    m_activeShape.MoveUp ();
    m_gameBoard.StoreShapeInGrid ( m_activeShape );
    if(m_ghost)
    {
      m_ghost.Reset ();                                                                       // kill the last ghost object  
    }

    // spawn a new shape
    m_activeShape = m_spawner.SpawnShape ();

    // set all of the timeToNextKey variables to current time, so no input delay for the next spawned shape
    m_timeToNextKeyLeftRight = Time.time;
    m_timeToNextKeyDown = Time.time;
    m_timeToNextKeyRotate = Time.time;

    // remove completed rows from the board if we have any 
    m_gameBoard.ClearAllRows ();


    PlaySound ( m_soundManager.m_dropSound );

    if ( m_gameBoard.m_completedRows > 0 )
    {
      m_scoreManager.ScoreLines ( m_gameBoard.m_completedRows );
      if ( m_scoreManager.m_didLevelUp )
      {
        PlaySound ( m_soundManager.m_levelUpVocalClip );
        m_dropIntervalModded =Mathf.Clamp( m_dropInterval - ( ( ( float ) m_scoreManager.m_level - 1 ) * 0.05f ),0.05f,1f);
      }
      else
      {
        if ( m_gameBoard.m_completedRows > 1 )
        {
          AudioClip randomVocal = m_soundManager.GetRandomClip ( m_soundManager.m_vocalClips );
          PlaySound ( randomVocal );
        }
      }
      PlaySound ( m_soundManager.m_clearRowSound );
    }


  }

  // triggered when we are over the board's limit
  void GameOver()
  {
    // move the shape one row up
    m_activeShape.MoveUp ();

    // turn on the Game Over Panel
    if ( m_gameOverPanel )
    {
      m_gameOverPanel.SetActive ( true );
    }
    // play the failure sound effect
    PlaySound ( m_soundManager.m_gameOverSound , 5f );

    // play "game over" vocal
    PlaySound ( m_soundManager.m_gameOverVocalClip , 5f );

    // set the game over condition to true
    m_gameOver = true;
  }

  // reload the level
  public void Restart()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene ( SceneManager.GetActiveScene().name );
//    Application.LoadLevel ( Application.loadedLevel );
  }

  // plays a sound with an option volume multiplier
  void PlaySound( AudioClip clip , float volMultiplier = 1.0f )
  {
    if ( m_soundManager.m_fxEnabled && clip )
    {
      AudioSource.PlayClipAtPoint ( clip , Camera.main.transform.position , Mathf.Clamp ( m_soundManager.m_fxVolume * volMultiplier , 0.05f , 1f ) );
    }
  }

  public void ToggleRotDirection()
  {
    m_clockwise = !m_clockwise;
    if ( m_rotIconToggle )
    {
      m_rotIconToggle.ToggleIcon ( m_clockwise );
    }

  }

  public void TogglePause()
  {
    m_isPaused = !m_isPaused;

    if ( m_pausePanel )
    {
      m_pausePanel.SetActive ( m_isPaused );

      if ( m_soundManager )
      {
        m_soundManager.m_musicSource.volume = ( m_isPaused ) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;

      }

      Time.timeScale = ( m_isPaused ) ? 0 : 1;
    }


  }






}
