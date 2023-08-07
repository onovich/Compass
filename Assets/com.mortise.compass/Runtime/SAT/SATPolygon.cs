using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass {
    public class SATPolygon {
        public Vector2[] points;

        public SATPolygon(Vector2[] points) {
            this.points = points;
        }

        /// <summary>
        /// 计算多边形在给定轴上的投影
        /// </summary>
        public void ProjectOntoAxis(Vector2 axis, out float min, out float max) {
            float dotProduct = axis.x * points[0].x + axis.y * points[0].y;
            min = dotProduct;
            max = dotProduct;

            for (int i = 1; i < points.Length; i++) {
                dotProduct = axis.x * points[i].x + axis.y * points[i].y;
                if (dotProduct < min)
                    min = dotProduct;
                else if (dotProduct > max)
                    max = dotProduct;
            }
        }

        /// <summary>
        /// 检测与另一个多边形是否相交
        /// </summary>
        public bool IsIntersecting(SATPolygon other) {
            for (int i = 0; i < points.Length; i++) {
                int nextIndex = (i + 1) % points.Length;
                Vector2 edge = new Vector2() { x = points[nextIndex].x - points[i].x, y = points[nextIndex].y - points[i].y };

                Vector2 normal = new Vector2() { x = -edge.y, y = edge.x };
                float minA, maxA;
                ProjectOntoAxis(normal, out minA, out maxA);

                float minB, maxB;
                other.ProjectOntoAxis(normal, out minB, out maxB);

                if (maxA < minB || maxB < minA)
                    return false;
            }

            return true;
        }
    }
}
