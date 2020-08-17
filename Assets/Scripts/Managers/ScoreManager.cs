using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ScoreManager : MonoBehaviour
{
  private int m_score = 0;
  private int m_lines;
  private int m_level = 1;

  public int m_linesPerLevel = 5;


  public const int m_minLines = 1;
  public const int m_maxLines = 4;

  public void ScoreLines( int n )
  {
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
  }
  public void Reset()
  {
    m_level = 1;
    m_lines = m_linesPerLevel * m_level;

  }
  // Start is called before the first frame update
  void Start()
  {
    Reset ();
  }

}// class
