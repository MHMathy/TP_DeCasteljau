using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrice : MonoBehaviour
{
    public class Matrice3x3
    {
        public float m00;
        public float m01;
        public float m02;
        public float m10;
        public float m11;
        public float m12;
        public float m20;
        public float m21;
        public float m22;

        public Matrice3x3 (
            float m00,
            float m01,
            float m02,
            float m10,
            float m11,
            float m12,
            float m20,
            float m21,
            float m22)
        {
			
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
        }
        
        public Matrice3x3 (Matrice3x3 m)
        {
            m00 = m.m00;
            m10 = m.m10;
            m20 = m.m20;
            m01 = m.m01;
            m11 = m.m11;
            m21 = m.m21;
            m02 = m.m02;
            m12 = m.m12;
            m22 = m.m22;
        }
        public Matrice3x3 ()
        {
            m00 = 0;
            m10 = 0;
            m20 = 0;
            m01 = 0;
            m11 = 0;
            m21 = 0;
            m02 = 0;
            m12 = 0;
            m22 = 0;
        }
        
        public static Matrice3x3 identity {
            get {
                Matrice3x3 matrix = new Matrice3x3 ();
                matrix.m00 = 1;
                matrix.m11 = 1;
                matrix.m22 = 1;
                return matrix;
            }
        }
        
        public static Matrice3x3 MultiplyMatrix3x3 (Matrice3x3 m1, Matrice3x3 m2)
        {
            Matrice3x3 m = new Matrice3x3 ();

            m.m00 = m1.m00 * m2.m00 + m1.m10 * m2.m01 + m1.m20 * m2.m02;
            m.m10 = m1.m00 * m2.m10 + m1.m10 * m2.m11 + m1.m20 * m2.m12;
            m.m20 = m1.m00 * m2.m20 + m1.m10 * m2.m21 + m1.m20 * m2.m22;
            m.m01 = m1.m01 * m2.m00 + m1.m11 * m2.m01 + m1.m21 * m2.m02;
            m.m11 = m1.m01 * m2.m10 + m1.m11 * m2.m11 + m1.m21 * m2.m12;
            m.m21 = m1.m01 * m2.m20 + m1.m11 * m2.m21 + m1.m21 * m2.m22;
            m.m02 = m1.m02 * m2.m00 + m1.m12 * m2.m01 + m1.m22 * m2.m02;
            m.m12 = m1.m02 * m2.m10 + m1.m12 * m2.m11 + m1.m22 * m2.m12;
            m.m22 = m1.m02 * m2.m20 + m1.m12 * m2.m21 + m1.m22 * m2.m22;
            return m;
        }
        public static Vector3 MultiplyVector3 (Matrice3x3 m1, Vector3 vector)
        {
            Vector3 outVector = new Vector3 ();
            outVector.x = m1.m00 * vector.x + m1.m01 * vector.y + m1.m02;
            outVector.y = m1.m10 * vector.x + m1.m11 * vector.y + m1.m12;
            outVector.z = m1.m20 * vector.x + m1.m21 * vector.y + m1.m22;
            return outVector;
        }

        public static Matrice3x3 CreateTranslation (Vector3 translation)
        {
            Matrice3x3 m = new Matrice3x3 ();
            m.m00 = 1;
            m.m11 = 1;
            m.m22 = 1;
            m.m02 = translation.x;
            m.m12 = translation.y;
            m.m22 = translation.z;
            return m;
        }
        
        public static Transform CreateRotationZ (Transform point,float rotation)
        {
            float cos = Mathf.Cos (rotation * Mathf.Deg2Rad);
            float sin = Mathf.Sin (rotation * Mathf.Deg2Rad);

            Matrice3x3 m = new Matrice3x3 ();
            m.m00 = cos;
            m.m01 = -sin;
            m.m10 = sin;
            m.m11 = cos;
            m.m22 = 1;

            point.position = MultiplyVector3(m, point.position);
            return point;
        }
        
        public static Transform CreateRotationY (Transform point,float rotation)
        {
            float cos = Mathf.Cos (rotation * Mathf.Deg2Rad);
            float sin = Mathf.Sin (rotation * Mathf.Deg2Rad);

            Matrice3x3 m = new Matrice3x3 ();
            m.m00 = cos;
            m.m02 = sin;
            m.m20 = -sin;
            m.m22 = cos;
            m.m11 = 1;

            point.position = MultiplyVector3(m, point.position);
            return point;
        }
        
        public static Transform CreateRotationX (Transform point,float rotation)
        {
            float cos = Mathf.Cos (rotation * Mathf.Deg2Rad);
            float sin = Mathf.Sin (rotation * Mathf.Deg2Rad);

            Matrice3x3 m = new Matrice3x3 ();
            m.m11 = cos;
            m.m12 = -sin;
            m.m21 = sin;
            m.m22 = cos;
            m.m00 = 1;

            point.position = MultiplyVector3(m, point.position);
            return point;
        }

        public static Matrice3x3 QuaternionToMatrice3x3(Quaternion quatA)
        {
            float w = quatA.w;
            float x = quatA.x;
            float y = quatA.y;
            float z = quatA.z;

            Matrice3x3 matrice = new Matrice3x3();
            // positions dans la matrice

            matrice.m00 = 1 - (2 * (y * y)) + (2 * (z * z));
            matrice.m01 = (2 * (x * y)) - (2 * (z * w));
            matrice.m02 = (2 * (x * z)) + (2 * (y * w));

            matrice.m10 = (2 * (x * y)) + (2 * (z * w));
            matrice.m11 = 1 - (2 * (x * x)) + (2 * (z * z));
            matrice.m12 = (2 * (y * z)) - (2 * (x * w));

            matrice.m20 = (2 * (x * z)) - (2 * (y * w));
            matrice.m21 = (2 * (y * z)) + (2 * (x * w));
            matrice.m22 = 1 - (2 * (x * x)) + (2 * (y * y));
            
            return matrice;
        }
        
        
        
        
        public static Matrice3x3 operator * (Matrice3x3 m1, Matrice3x3 m2)
        { 
            return Matrice3x3.MultiplyMatrix3x3 (m1, m2); 
        }

        public static Vector3 operator * (Matrice3x3 m, Vector3 v)
        { 
            return Matrice3x3.MultiplyVector3 (m, v); 
        }
    }
    
    
}
