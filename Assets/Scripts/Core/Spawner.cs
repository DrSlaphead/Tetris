using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{

  // our library of shapes, make sure you don't leave any blanks in the Inspector
  public Shape [] m_allShapes;
  public Transform [] m_queuedXforms = new Transform [ 3 ];

  private Shape [] m_queuedShapes = new Shape [ 3 ];
  private float m_queueScale = 0.5f;

  private void Awake()
  {
    InitQueue ();
  }



  // returns a random shape from our library of shapes
  Shape GetRandomShape()
  {
    int i = Random.Range ( 0 , m_allShapes.Length );
    if ( m_allShapes [ i ] )
    {
      return m_allShapes [ i ];
    }
    else
    {
      Debug.LogWarning ( "WARNING! Invalid shape in spawner!" );
      return null;
    }

  }
  // instantiates a shape at the spawner's position
  public Shape SpawnShape()
  {
    Shape shape = null;
    //    shape = Instantiate ( GetRandomShape () , transform.position , Quaternion.identity ) as Shape;
    shape = GetQueuedShape ();
    shape.transform.position = transform.position;
    shape.transform.localScale = Vector3.one;
    if ( shape )
    {
      return shape;
    }
    else
    {
      Debug.LogWarning ( "WARNING! Invalid shape in spawner!" );
      return null;
    }
  }
  private void InitQueue()
  {
    for ( int i = 0 ; i < m_queuedShapes.Length ; i++ )
    {
      m_queuedShapes [ i ] = null;
    }
    FillQueue ();
  }
  private void FillQueue()
  {
    for ( int i = 0 ; i < m_queuedShapes.Length ; i++ )
    {
      if ( !m_queuedShapes [ i ] )
      {
        m_queuedShapes [ i ] = Instantiate ( GetRandomShape () , transform.position , Quaternion.identity ) as Shape;
        m_queuedShapes [ i ].transform.position = m_queuedXforms [ i ].position+ m_queuedShapes [ i ].m_queueOffset;
        m_queuedShapes [ i ].transform.localScale = new Vector3 ( m_queueScale , m_queueScale , m_queueScale );
      }
    }
  }

  private Shape GetQueuedShape()
  {
    Shape firstShape = null;

    if ( m_queuedShapes [ 0 ] )
    {
      firstShape = m_queuedShapes [ 0 ];
    }
    for ( int i = 1 ; i < m_queuedShapes.Length ; i++ )
    {
      m_queuedShapes [ i - 1 ] = m_queuedShapes [ i ];                                        // shift shapes up one  
      m_queuedShapes [ i - 1 ].transform.position = m_queuedXforms [ i - 1 ].position + m_queuedShapes [ i ].m_queueOffset;
    }
    m_queuedShapes [ m_queuedShapes.Length - 1 ] = null;
    FillQueue ();
    return firstShape;
  }

}//class
