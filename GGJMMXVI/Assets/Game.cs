using UnityEngine;
using UE = UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class Game : MonoBehaviour {
	[Serializable]
	private class Sound {
		public AudioClip[] clips;
		public bool cycle;
		public float volume = 1.0f;

		private int cycleIndex;

		public AudioClip GetClip() {
			if( clips == null || clips.Length == 0 ) {
				return null;
			}

			int index = 0;
			if( cycle ) {
				index = cycleIndex;
				cycleIndex++;
				if( cycleIndex == clips.Length ) {
					cycleIndex = 0;
				}
			} else {
				index = UE.Random.Range( 0, clips.Length );
			}

			return clips[ index ];
		}
	};

	[SerializeField]
	private Button buttonPrefab;
	[SerializeField]
	private Toggle togglePrefab;
	[SerializeField]
	private Text labelKcal;
	[SerializeField]
	private Text labelMain;
	[SerializeField]
	private Text labelFood;
	[SerializeField]
	private Transform navButtonsHolder;
	[SerializeField]
	private Transform foodButtonsHolder;

	[SerializeField]
	private Sound soundTick;
	[SerializeField]
	private Sound soundTock;

	[SerializeField]
	private Music musicAlarm;
	[SerializeField]
	private Music musicMelancholy;
	[SerializeField]
	private Music musicPanic;
	[SerializeField]
	private Music musicAngst;

	private Button[] navButtons;
	private Toggle[] foodButtons;
	private Dictionary<string, int> tagToIndex;

	private int kcal;
	private int selectedButton;
	private int dinnerIndex;
	private int angstIndex;
	private bool pageIsSingleChoice;

	private AudioSource soundSource;

	private void OnEnable() {
		dinnerIndex = -1;
		angstIndex = -1;

		soundSource = GetComponent<AudioSource>();
		if( soundSource == null ) {
			soundSource = gameObject.AddComponent<AudioSource>();
		}
		soundSource.spatialBlend = 0.0f;

		tagToIndex = new Dictionary<string,int>();
		for( int i = 0; i < pages.Length; ++i ) {
			Page page = pages[ i ];
			if( !string.IsNullOrEmpty( (string)page.tag ) ) {
				UE.Debug.LogFormat( "Tagging {0} as {1}", i, (string)page.tag );
				tagToIndex[ (string)page.tag ] = i;
			}
		}
		labelMain.text = "";
		labelFood.text = "";
		labelKcal.text = "000\nkcal";
		UpdateKcalColor();
		selectedButton = -1;
		kcal = 0;
		pageIsSingleChoice = false;
		navButtons = new Button[ 3 ];
		for( int i = 0; i < navButtons.Length; ++i ) {
			Button button = Instantiate( buttonPrefab );
			if( i == 0 ) {
				SetButtonAlpha( button, 0.0f );
			}
			button.transform.SetParent( navButtonsHolder );
			navButtons[ i ] = button;
			int index = i;
			button.onClick.AddListener( () => {
				selectedButton = index;
			} );
			button.gameObject.SetActive( false );
		}
		foodButtons = new Toggle[ 5 ];
		for( int i = 0; i < foodButtons.Length; ++i ) {
			Toggle toggle = Instantiate( togglePrefab );
			toggle.transform.SetParent( foodButtonsHolder );
			foodButtons[ i ] = toggle;
			int index = i;
			toggle.onValueChanged.AddListener( ( bool value ) => {
				if( !pageIsSingleChoice ) {
					return;
				}
				if( value ) {
					for( int z = 0; z < foodButtons.Length; ++z ) {
						if( index == z ) {
							continue;
						}
						foodButtons[ z ].isOn = false;
					}
				}
			} );
			toggle.gameObject.SetActive( false );
		}
		StartCoroutine( Run( pageIndex: 0 ) );
		StartCoroutine( "BlinkKcal" );
		StartCoroutine( FadeInButton( navButtons[ 0 ], 2.0f, 1.0f ) );
	}

	private void OnDisable() {
		StopAllCoroutines();
		for( int i = 0; i < navButtons.Length; ++i ) {
			Destroy( navButtons[ i ].gameObject );
		}
		for( int i = 0; i < foodButtons.Length; ++i ) {
			foodButtons[ i ].onValueChanged.RemoveAllListeners();
			Destroy( foodButtons[ i ].gameObject );
		}
		tagToIndex.Clear();
	}

	private IEnumerator FadeInButton( Button button, float delay, float duration ) {
		yield return new WaitForSeconds( delay );
		
		float timer = duration;
		while( timer > 0.0f ) {
			yield return null;
			timer -= Time.deltaTime;
			SetButtonAlpha( button, 1.0f - ( timer / duration ) );
		}
	}

	private IEnumerator BlinkKcal() {
		float t = 0.5f;
		while( true ) {
			yield return null;
			t -= Time.deltaTime;
			if( t < 0.0f ) {
				labelKcal.enabled = !labelKcal.enabled;
				t = 0.5f;
			}
		}
	}

	private IEnumerator Run( int pageIndex ) {
		if( pageIndex == 0 ) {
			musicAlarm.targetVolume = 1.0f;
		} else if( pageIndex == 1 ) {
			StopCoroutine( "BlinkKcal" );
			labelKcal.enabled = true;
			musicAlarm.SetVolume( 0.0f );
			musicMelancholy.targetVolume = 1.0f;
		}
		selectedButton = -1;
		pageIsSingleChoice = false;
		int nextPage = pageIndex + 1;
		Page page = pages[ pageIndex ];

		if( "AfterFlushNapkin" == (string)page.tag ) {
			dinnerIndex = -1;
			musicPanic.targetVolume = 0.0f;
		} else if( "KitchenCake" == (string)page.tag ) {
			angstIndex = 0;
		}

		if( "Throw1" == (string)page.tag ) {
			angstIndex = -1;
			musicAngst.targetVolume = 0.7f;
		} else if( "Throw2" == (string)page.tag ) {
			musicAngst.targetVolume = 0.4f;
		} else if( "Throw3" == (string)page.tag ) {
			musicAngst.targetVolume = 0.0f;
		}

		if( angstIndex >= 0 ) {
			++angstIndex;
			float volume = ( (float)angstIndex ) / 7.0f;
			musicAngst.targetVolume = volume;
		}

		if( page is NormalPage ) {
			var normalPage = (NormalPage)page;
			Set( normalPage.mainText, normalPage.buttons );
			int forced = kcal < 500 ? -1 : Array.FindIndex( normalPage.buttons, buttonInfo => buttonInfo.forceIfFat );
			while( true ) {
				yield return StartCoroutine( WaitForButton() );
				if( forced != -1 && selectedButton != forced ) {
					selectedButton = -1;
					Set( "You can't do that yet.", normalPage.buttons );
				} else {
					break;
				}

			}
		} else if( page is FoodPage ) {
			var foodPage = (FoodPage)page;
			pageIsSingleChoice = foodPage.singleChoice;
			SetFood( foodPage.mainText, foodPage.buttonText, foodPage.food );

			while( true ) {
				selectedButton = -1;
				yield return StartCoroutine( WaitForButton() );
				FoodInfo[] selected = GetSelectedFood( foodPage );
				if( selected.Length != 0 ) {
					break;
				}
			}
			labelFood.text = "";
		} else if( page is ThrowPage ) {
			var throwPage = (ThrowPage)page;
			Set( throwPage.mainText );
			int kcalToAdd = throwPage.aThird ? ( kcal / 3 ) : kcal;
			yield return StartCoroutine( AddKcal( -kcalToAdd ) );
			Set( throwPage.mainText, throwPage.button );
			yield return StartCoroutine( WaitForButton() );
		}

		if( page is NormalPage ) {
			var normalPage = (NormalPage)page;
			ButtonInfo info = normalPage.buttons[ selectedButton ];
			yield return StartCoroutine( AddKcal( info.kcal ) );
			if( !string.IsNullOrEmpty( info.pageTag ) ) {
				UE.Debug.Assert( tagToIndex.ContainsKey( (string)info.pageTag ), "Tag \"{0}\" not found", info.pageTag );
				nextPage = tagToIndex[ (string)info.pageTag ];
			}
		} else if( page is FoodPage ) {
			var foodPage = (FoodPage)page;
			string foodPageTag = (string)foodPage.tag;
			if( foodPageTag != "Dinner" && foodPageTag != "FridgeCake" ) {
				FoodInfo[] food = GetSelectedFood( foodPage );
				StartCoroutine( SetFoodButtons( none: true ) );
				yield return StartCoroutine( EatFood( foodPage.eatMainText, food, foodPage.postEatButtonText ) );
			}

			if( foodPageTag == "Dinner" ) {
				dinnerIndex = 0;
				musicMelancholy.targetVolume = 0.0f;
			}
		} else if( page is ThrowPage ) {
			var throwPage = (ThrowPage)page;
			ButtonInfo info = throwPage.button;
			if( !string.IsNullOrEmpty( info.pageTag ) ) {
				UE.Debug.Assert( tagToIndex.ContainsKey( (string)info.pageTag ), "Tag \"{0}\" not found", info.pageTag );
				nextPage = tagToIndex[ (string)info.pageTag ];
			}
		}

		if( dinnerIndex >= 0 ) {
			++dinnerIndex;
			float volume = ( (float)dinnerIndex ) / 5.0f;
			musicPanic.targetVolume = volume;
		}

		if( nextPage >= pages.Length ) {
			StartCoroutine( FadeOutAndGotoCredits() );
		} else {
			Set( "" );
			SetFood( "", "" );
			SetFoodButtons( none: true );
			StartCoroutine( Run( nextPage ) );
		}
	}

	private IEnumerator FadeOutAndGotoCredits() {
		UE.EventSystems.EventSystem.current.enabled = false;
		yield return StartCoroutine( WaitOrClick( 3.0f, alpha => {
			SetButtonAlpha( navButtons[ 0 ], 1.0f - alpha );
		} ) );
		yield return StartCoroutine( WaitOrClick( 3.0f, alpha => {
			Color c = labelMain.color;
			c.a = 1.0f - alpha;
			labelMain.color = c;
		} ) );
		yield return StartCoroutine( WaitOrClick( 3.0f, alpha => {
			Color c = labelKcal.color;
			c.a = 1.0f - alpha;
			labelKcal.color = c;
		} ) );
		yield return new WaitForSeconds( 2.0f );
		Application.LoadLevel( "Credits" );
	}

	private FoodInfo[] GetSelectedFood( FoodPage page ) {
		var selected = new List<FoodInfo>();
		for( int i = 0; i < foodButtons.Length; ++i ) {
			if( foodButtons[ i ].isOn ) {
				selected.Add( page.food[ i ] );
			}
		}
		return selected.ToArray();
	}

	private IEnumerator EatFood( string mainText, FoodInfo[] food, string buttonText ) {
		Set( mainText );
		for( int i = 0; i < food.Length; ++i ) {
			yield return new WaitForSeconds( 1.0f );
			yield return StartCoroutine( AddKcal( food[ i ].kcal ) );
		}
		Set( mainText, new ButtonInfo( buttonText ) );
		selectedButton = -1;
		yield return StartCoroutine( WaitForButton() );
	}

	private void Set( string mainText, params ButtonInfo[] buttons ) {
		StartCoroutine( SetText( mainText ) );
		StartCoroutine( SetNavButtons( buttons ) );
	}

	private void SetFood( string mainText, string navButton, params FoodInfo[] food ) {
		labelFood.text = mainText;
		StartCoroutine( SetFoodButtons( food.Length == 0, food ) );
		Set( "", new ButtonInfo( navButton ) );
	}

	private IEnumerator SetNavButtons( params ButtonInfo[] args ) {
		for( int i = 0; i < navButtons.Length; ++i ) {
			Button button = navButtons[ i ];
			if( i >= args.Length ) {
				button.gameObject.SetActive( false );
			} else {
				button.gameObject.SetActive( true );
				ButtonInfo info = args[ i ];
				Text buttonText = button.GetComponentInChildren<Text>();
				if( info.kcal != 0 ) {
					buttonText.text = string.Format( "{0} [{1} kcal]", info.text, info.kcal );
				} else {
					buttonText.text = info.text;
				}
			}
		}
		yield break;
	}

	private IEnumerator SetFoodButtons( params FoodInfo[] args ) {
		yield return StartCoroutine( SetFoodButtons( false, args ) );
	}

	private IEnumerator SetFoodButtons( bool none, params FoodInfo[] args ) {
		for( int i = 0; i < foodButtons.Length; ++i ) {
			Toggle toggle = foodButtons[ i ];
			toggle.isOn = false;
			if( none || i >= args.Length ) {
				toggle.gameObject.SetActive( false );
			} else {
				toggle.gameObject.SetActive( true );
				toggle.GetComponentInChildren<Text>().text = string.Format( "{0} [{1} kcal]", args[ i ].name, args[ i ].kcal );
			}
		}
		yield break;
	}

	private IEnumerator AddKcal( int value ) {
		if( value == 0 ) {
			yield break;
		}
		kcal += value;
		labelKcal.text = string.Format( "{0:000}\nkcal", kcal );
		float volume = Mathf.Lerp( 0.2f, 1.0f, ( (float)Mathf.Max( kcal, 0 ) ) / 500.0f );
		Sound sound = value > 0 ? soundTick : soundTock;
		PlaySound( sound, volume );
		UpdateKcalColor();
		yield break;
	}

	private void UpdateKcalColor() {
		Color color = kcal < 250 ? Color.green : kcal < 500 ? Color.yellow : Color.red;
		labelKcal.color = color;
	}

	private IEnumerator SetText( string value ) {
		labelMain.text = value;
		yield break;
	}

	private IEnumerator WaitForButton() {
		while( selectedButton == -1 ) {
			yield return null;
		}
	}

	private void PlaySound( Sound sound, float volume ) {
		AudioClip clip = sound == null ? null : sound.GetClip();
		if( clip == null ) {
			return;
		}
		soundSource.PlayOneShot( clip, sound.volume * volume );
	}

	private void SetButtonAlpha( Button button, float alpha ) {
		ColorBlock cb = button.colors;
		Color c = cb.normalColor;
		c.a = alpha;
		cb.normalColor = c;
		button.colors = cb;
		Text label = button.GetComponentInChildren<Text>();
		c = label.color;
		c.a = alpha;
		label.color = c;
	}

	private IEnumerator WaitOrClick( float duration, Action<float> action = null ) {
		float timer = duration;
		while( timer > 0.0f ) {
			yield return null;
			timer -= Time.deltaTime;
			if( Input.GetMouseButtonUp( 0 ) ) {
				if( action != null ) {
					action( 1.0f );
				}
				break;
			} else {
				if( action != null ) {
					action( 1.0f - timer / duration );
				}
			}
		}
	}
}
