using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour {
	// the sprite that we use if we are set to true
	public Sprite m_iconTrue;

	// the sprite that we use if we are set to false
	public Sprite m_iconFalse;

	// default state of the icon toggle
	public bool m_defaultIconState = true;

	// the UI.Image component
	Image m_image;


	// Use this for initialization
	void Start () {
		m_image = GetComponent<Image>();
		m_image.sprite = (m_defaultIconState) ? m_iconTrue : m_iconFalse;
	}

	public void ToggleIcon(bool state)
	{
		if (!m_image || !m_iconTrue || !m_iconFalse)
		{
			Debug.LogWarning("ICONTOGGLE Undefined iconTrue and/or iconFalse!"); 
			return;
		}

		m_image.sprite = (state) ? m_iconTrue : m_iconFalse;
	}
		
}
