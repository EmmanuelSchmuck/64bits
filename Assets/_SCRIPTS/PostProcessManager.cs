using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
    public class PostProcessManager : MonoBehaviour
    {

        public PostProcessingProfile mainPP;
        public PostProcessingProfile waterPP;
        public float waterPPChromaticAberration;
        public PostProcessingBehaviour ppBehaviour;

        // Use this for initialization
        void Start()
        {
            ppBehaviour.profile = mainPP;

            ChromaticAberrationModel.Settings chromaSettings = waterPP.chromaticAberration.settings;
            chromaSettings.intensity = waterPPChromaticAberration;
            waterPP.chromaticAberration.settings = chromaSettings;
        }

        // Update is called once per frame
        public void OnEnteringWater(){
            Debug.Log("entering water");
            ppBehaviour.profile = waterPP;

        }

        
        public void OnExitingWater(){
            Debug.Log("exiting water");
            ppBehaviour.profile = mainPP;
            
        }
    }
}
