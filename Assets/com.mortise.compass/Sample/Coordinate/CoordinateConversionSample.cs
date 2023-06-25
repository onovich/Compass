using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class CoordinateConversionSample : MonoBehaviour {

        [Header("输出")] public Vector2Int index;
        [Header("网格数据输入")] public CompassSampleSO compassSampleSO;

        void Update() {

            var pos = transform.position;
            var mpu = compassSampleSO.tm.MPU;
            var offset = compassSampleSO.tm.LocalOffset;
            var map = GetComponent<NavAgentSample>().Map;
            this.index = MathUtil.Pos2Index(pos, mpu, offset, map);

        }

    }

}