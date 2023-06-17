using UnityEngine;
using MortiseFrame.Modifier.Toaster.Generic;

namespace MortiseFrame.Compass.Sample {

    [CreateAssetMenu(fileName = "ToasterSampleSO", menuName = "MortiseFrame/Compass/Sample/ToasterSampleSO")]
    public class CompassSampleSO : ScriptableObject {

        [SerializeField] public ToasterGridTM tm;

    }

}
