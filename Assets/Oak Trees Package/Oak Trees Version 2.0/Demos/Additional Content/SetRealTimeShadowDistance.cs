using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTI.Demo {
	
	public class SetRealTimeShadowDistance : MonoBehaviour {

		public float ShadowDistance = 300.0f;
		private float origShadowDistance;

		void OnEnable() {
			origShadowDistance = QualitySettings.shadowDistance;
			QualitySettings.shadowDistance = ShadowDistance;
		}

		void OnDisable() {
			QualitySettings.shadowDistance = origShadowDistance;
		}

		void Start () {
			SetShadowDistance();
		}
		void OnValidate () {
			SetShadowDistance();
		}

		public void SetShadowDistance() {
			QualitySettings.shadowDistance = ShadowDistance;
		//	Update the Translucent Lighting Fade Range and set it to ShadowDistance. Set the length of the transition to ShadowDistance * 0.2f 
			CTI.CTI_Utils.SetTranslucentLightingFade(ShadowDistance, 0.2f);
		}
	}
}