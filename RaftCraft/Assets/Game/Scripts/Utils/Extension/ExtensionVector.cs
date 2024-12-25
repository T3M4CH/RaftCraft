using UnityEngine;

namespace Game.Scripts.Extension
{
    public static class ExtensionVector
    {
        public static float Angle(this Vector2 direction)
        {
            float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return rotZ;
        }

        public static float Angle(this Vector3 direction)
        {
            float rotZ = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            return rotZ;
        }

        public static float Direction(this Vector2 start, Vector2 target)
        {
            Vector2 direction = start - target;
            float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return rotZ;
        }

        public static Vector2 Cross(Vector2 A, Vector2 B, Vector2 C, Vector2 D) 
        {
            Vector2 dot = Vector2.zero;

            float n;
            if (B.y - A.y != 0) 
            { 
                float q = (B.x - A.x) / (A.y - B.y);   
                float sn = (C.x - D.x) + (C.y - D.y) * q; 
                float fn = (C.x - A.x) + (C.y - A.y) * q;
                n = fn / sn;
            }
            else 
                n = (C.y - A.y) / (C.y - D.y);

            dot.x = C.x + (D.x - C.x) * n;
            dot.y = C.y + (D.y - C.y) * n;

            return dot;
        }
    
        public static Vector3 Cross(Vector3 A, Vector3 B, Vector3 C, Vector3 D) 
        {
            Vector3 dot = Vector3.zero;

            float n;
            if (B.z - A.z != 0) 
            { 
                float q = (B.x - A.x) / (A.z - B.z);   
                float sn = (C.x - D.x) + (C.z - D.z) * q; 
                float fn = (C.x - A.x) + (C.z - A.z) * q;
                n = fn / sn;
            }
            else 
                n = (C.z - A.z) / (C.z - D.z);

            dot.x = C.x + (D.x - C.x) * n;
            dot.z = C.z + (D.z - C.z) * n;

            return dot;
        }
    
        public static bool AreLineSegmentsIntersectingDotProduct(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isIntersecting = IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2);

            return isIntersecting;
        }

        private static bool IsPointsOnDifferentSides(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isOnDifferentSides = false;
	
            Vector3 lineDir = p2 - p1;

            Vector3 lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);
	
            float dot1 = Vector3.Dot(lineNormal, p3 - p1);
            float dot2 = Vector3.Dot(lineNormal, p4 - p1);

            if (dot1 * dot2 < 0f)
            {
                isOnDifferentSides = true;
            }

            return isOnDifferentSides;
        }
    
        public static void CalcDampedSimpleHarmonicMotionFast(ref float position, ref float velocity, 
            float equilibriumPosition, float deltaTime, float angularFrequency, float dampingRatio)
        {
            var x = position - equilibriumPosition;
            velocity += (-dampingRatio * velocity) - (angularFrequency * x);
            position += velocity * deltaTime;
        }

        public static void CalcDampedSimpleHarmonicMotionFast(ref Vector2 position, ref Vector2 velocity,
            Vector2 equilibriumPosition, float deltaTime, float angularFrequency, float dampingRatio)
        {
            var x = position - equilibriumPosition;
            velocity += (-dampingRatio * velocity) - (angularFrequency * x);
            position += velocity * deltaTime;
        }

        public static void CalcDampedSimpleHarmonicMotionFast(ref Vector3 position, ref Vector3 velocity,
            Vector3 equilibriumPosition, float deltaTime, float angularFrequency, float dampingRatio)
        {
            var x = position - equilibriumPosition;
            velocity += (-dampingRatio * velocity) - (angularFrequency * x);
            position += velocity * deltaTime;
        }
    }
}
