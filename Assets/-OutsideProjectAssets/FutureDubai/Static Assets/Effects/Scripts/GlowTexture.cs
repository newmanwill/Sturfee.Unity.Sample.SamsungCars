using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GlowTexture : MonoBehaviour {
	public Texture2D glowTexture;

	void Awake() {
		GetComponent<Renderer>().sharedMaterial.SetTexture("_GlowTex", glowTexture);
	}
}
