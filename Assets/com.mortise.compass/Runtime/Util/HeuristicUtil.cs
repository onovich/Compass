using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public static class HeuristicUtil {

        public static Func<Node2D, Node2D, float> GetHeuristic(HeuristicType type) {

            switch (type) {

                case HeuristicType.Manhattan:
                    return Manhattan;
                case HeuristicType.Euclidean:
                    return Euclidean;
                case HeuristicType.Chebyshev:
                    return Chebyshev;
                case HeuristicType.Octile:
                    return Octile;
            }

            return Euclidean;

        }

        public static float Manhattan(Node2D current, Node2D neighbor) {
            return Mathf.Abs(current.X - neighbor.X) + Mathf.Abs(current.Y - neighbor.Y);
        }

        public static float Euclidean(Node2D current, Node2D neighbor) {
            var dx = current.X - neighbor.X;
            var dy = current.Y - neighbor.Y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static float Chebyshev(Node2D current, Node2D neighbor) {
            return Mathf.Max(Mathf.Abs(current.X - neighbor.X), Mathf.Abs(current.Y - neighbor.Y));
        }

        public static float Octile(Node2D current, Node2D neighbor) {
            var dx = Mathf.Abs(current.X - neighbor.X);
            var dy = Mathf.Abs(current.Y - neighbor.Y);
            return dx + dy + (Mathf.Sqrt(2) - 2) * Mathf.Min(dx, dy);
        }

        public static float Euclidean_Squared(Node2D current, Node2D neighbor) {
            var dx = current.X - neighbor.X;
            var dy = current.Y - neighbor.Y;
            return dx * dx + dy * dy;
        }

        public static float Chebyshev_Squared(Node2D current, Node2D neighbor) {
            var dx = Mathf.Abs(current.X - neighbor.X);
            var dy = Mathf.Abs(current.Y - neighbor.Y);
            return Mathf.Max(dx, dy);
        }

        public static float Octile_Squared(Node2D current, Node2D neighbor) {
            var dx = Mathf.Abs(current.X - neighbor.X);
            var dy = Mathf.Abs(current.Y - neighbor.Y);
            return dx * dx + dy * dy + (Mathf.Sqrt(2) - 2) * Mathf.Min(dx, dy);
        }

    }

}