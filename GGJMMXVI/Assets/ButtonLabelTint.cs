using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonLabelTint : MonoBehaviour {
	[SerializeField]
	private Button button;
	[SerializeField]
	private Text label;

	private void Update() {
		if( button != null && label != null ) {
			label.color = button.image.color;
		}
	}
}
