using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
  private int m_score = 0;
  private int m_lines;
  public int m_level = 1;

  public int m_linesPerLevel = 3;
  public Text m_linesText;
  public Text m_levelText;
  public Text m_scoreText;

  public bool m_didLevelUp = false;

  public const int m_minLines = 1;
  public const int m_maxLines = 4;

  // Start is called before the first frame update
  void Start()
  {
    Reset ();
  }

  public void ScoreLines( int n )
  {
    m_didLevelUp = false;
    n = Mathf.Clamp ( n , m_minLines , m_maxLines );                                          // the number of lines = score multiply  
    switch ( n )
    {
      case 1:
        m_score += 40 * m_level;
        break;
      case 2:
        m_score += 100 * m_level;
        break;
      case 3:
        m_score += 300 * m_level;
        break;
      case 4:
        m_score += 1200 * m_level;
        break;
    }
    m_lines -= n;
    if(m_lines<=0)
    {
      LevelUp ();
    }
    UpdateUIText ();
  }
  public void Reset()
  {
    m_level = 1;
    m_lines = m_linesPerLevel * m_level;
    UpdateUIText ();
  }

  private void UpdateUIText()
  {
    if ( m_linesText )
    {
      m_linesText.text = m_lines.ToString ();
    }
    if ( m_levelText )
    {
      m_levelText.text = m_level.ToString ();
    }
    if ( m_scoreText )
    {
      m_scoreText.text = PadZero ( m_score , 5 );
    }
  }
  private string PadZero( int n , int padDigits )
  {
    string nStr = n.ToString ();
    while ( nStr.Length < padDigits )
    {
      nStr = "0" + nStr;
    }
    return nStr;
  }
  public void LevelUp()
  {
    m_level++;
    m_lines = m_linesPerLevel * m_level;
    m_didLevelUp = true;
  }




}// class
