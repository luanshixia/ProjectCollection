using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.Geometry3D
{
    public class Matrix4
    {
        //
        // Matrix4 class is based on the same class in THREE.js
        //

        public double n11;
        public double n12;
        public double n13;
        public double n14;
        public double n21;
        public double n22;
        public double n23;
        public double n24;
        public double n31;
        public double n32;
        public double n33;
        public double n34;
        public double n41;
        public double n42;
        public double n43;
        public double n44;

        public Matrix4()
        {
        }

        public Matrix4(double n11, double n12, double n13, double n14, double n21, double n22, double n23, double n24, double n31, double n32, double n33, double n34, double n41, double n42, double n43, double n44)
        {
            this.n11 = n11; this.n12 = n12; this.n13 = n13; this.n14 = n14;
            this.n21 = n21; this.n22 = n22; this.n23 = n23; this.n24 = n24;
            this.n31 = n31; this.n32 = n32; this.n33 = n33; this.n34 = n34;
            this.n41 = n41; this.n42 = n42; this.n43 = n43; this.n44 = n44;
        }

        //    THREE.Matrix4 = function ( n11, n12, n13, n14, n21, n22, n23, n24, n31, n32, n33, n34, n41, n42, n43, n44 ) {

        //    this.set(

        //        ( n11 !== undefined ) ? n11 : 1, n12 || 0, n13 || 0, n14 || 0,
        //        n21 || 0, ( n22 !== undefined ) ? n22 : 1, n23 || 0, n24 || 0,
        //        n31 || 0, n32 || 0, ( n33 !== undefined ) ? n33 : 1, n34 || 0,
        //        n41 || 0, n42 || 0, n43 || 0, ( n44 !== undefined ) ? n44 : 1

        //    );

        //    this.flat = new Array( 16 );
        //    this.m33 = new THREE.Matrix3();

        //};

        //THREE.Matrix4.prototype = {

        //    constructor: THREE.Matrix4,

        //    set: function ( n11, n12, n13, n14, n21, n22, n23, n24, n31, n32, n33, n34, n41, n42, n43, n44 ) {

        //        this.n11 = n11; this.n12 = n12; this.n13 = n13; this.n14 = n14;
        //        this.n21 = n21; this.n22 = n22; this.n23 = n23; this.n24 = n24;
        //        this.n31 = n31; this.n32 = n32; this.n33 = n33; this.n34 = n34;
        //        this.n41 = n41; this.n42 = n42; this.n43 = n43; this.n44 = n44;

        //        return this;

        //    },

        private static Matrix4 _identity = new Matrix4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
        public static Matrix4 Identity { get { return _identity; } }

        //copy: function ( m ) {

        //    this.set(

        //        m.n11, m.n12, m.n13, m.n14,
        //        m.n21, m.n22, m.n23, m.n24,
        //        m.n31, m.n32, m.n33, m.n34,
        //        m.n41, m.n42, m.n43, m.n44

        //    );

        //    return this;

        //},

        public static Matrix4 LookAt(Point3D eye, Point3D center, Vector3D up)
        {
            var z = (eye - center).Normalize();
            if (z.Abs == 0)
            {
                z.z = 1;
            }

            var x = up.Cross(z).Normalize();
            if (x.Abs == 0)
            {
                z.x += 0.0001;
                x = up.Cross(z).Normalize();
            }

            var y = z.Cross(x).Normalize();

            Matrix4 m = new Matrix4();

            m.n11 = x.x; m.n12 = y.x; m.n13 = z.x;
            m.n21 = x.y; m.n22 = y.y; m.n23 = z.y;
            m.n31 = x.z; m.n32 = y.z; m.n33 = z.z;

            return m;
        }

        public static Matrix4 Multiply(Matrix4 a, Matrix4 b)
        {
            Matrix4 m = new Matrix4();

            double a11 = a.n11, a12 = a.n12, a13 = a.n13, a14 = a.n14,
            a21 = a.n21, a22 = a.n22, a23 = a.n23, a24 = a.n24,
            a31 = a.n31, a32 = a.n32, a33 = a.n33, a34 = a.n34,
            a41 = a.n41, a42 = a.n42, a43 = a.n43, a44 = a.n44,

            b11 = b.n11, b12 = b.n12, b13 = b.n13, b14 = b.n14,
            b21 = b.n21, b22 = b.n22, b23 = b.n23, b24 = b.n24,
            b31 = b.n31, b32 = b.n32, b33 = b.n33, b34 = b.n34,
            b41 = b.n41, b42 = b.n42, b43 = b.n43, b44 = b.n44;

            m.n11 = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            m.n12 = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            m.n13 = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            m.n14 = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            m.n21 = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            m.n22 = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            m.n23 = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            m.n24 = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            m.n31 = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            m.n32 = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            m.n33 = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            m.n34 = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            m.n41 = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            m.n42 = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            m.n43 = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            m.n44 = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return m;
        }

        public Matrix4 Multiply(Matrix4 m)
        {
            return Multiply(this, m);
        }

        //multiplyToArray: function ( a, b, r ) {

        //    this.multiply( a, b );

        //    r[ 0 ] = this.n11; r[ 1 ] = this.n21; r[ 2 ] = this.n31; r[ 3 ] = this.n41;
        //    r[ 4 ] = this.n12; r[ 5 ] = this.n22; r[ 6 ] = this.n32; r[ 7 ] = this.n42;
        //    r[ 8 ]  = this.n13; r[ 9 ]  = this.n23; r[ 10 ] = this.n33; r[ 11 ] = this.n43;
        //    r[ 12 ] = this.n14; r[ 13 ] = this.n24; r[ 14 ] = this.n34; r[ 15 ] = this.n44;

        //    return this;

        //},

        public Matrix4 MultiplyScalar(double s)
        {
            Matrix4 m = new Matrix4();

            m.n11 = this.n11 * s; m.n12 = this.n12 * s; m.n13 = this.n13 * s; m.n14 = this.n14 * s;
            m.n21 = this.n21 * s; m.n22 = this.n22 * s; m.n23 = this.n23 * s; m.n24 = this.n24 * s;
            m.n31 = this.n31 * s; m.n32 = this.n32 * s; m.n33 = this.n33 * s; m.n34 = this.n34 * s;
            m.n41 = this.n41 * s; m.n42 = this.n42 * s; m.n43 = this.n43 * s; m.n44 = this.n44 * s;

            return m;
        }

        public Vector3D MultiplyVector3(Vector3D v)
        {
            double vx = v.x, vy = v.y, vz = v.z,
            d = 1 / (this.n41 * vx + this.n42 * vy + this.n43 * vz + this.n44);

            Vector3D u = new Vector3D();

            u.x = (this.n11 * vx + this.n12 * vy + this.n13 * vz + this.n14) * d;
            u.y = (this.n21 * vx + this.n22 * vy + this.n23 * vz + this.n24) * d;
            u.z = (this.n31 * vx + this.n32 * vy + this.n33 * vz + this.n34) * d;

            return u;
        }

        public Point3D MultiplyVector3(Point3D v)
        {
            double vx = v.x, vy = v.y, vz = v.z,
            d = 1 / (this.n41 * vx + this.n42 * vy + this.n43 * vz + this.n44);

            Point3D u = new Point3D();

            u.x = (this.n11 * vx + this.n12 * vy + this.n13 * vz + this.n14) * d;
            u.y = (this.n21 * vx + this.n22 * vy + this.n23 * vz + this.n24) * d;
            u.z = (this.n31 * vx + this.n32 * vy + this.n33 * vz + this.n34) * d;

            return u;
        }

        //public Geometry.Computational.Vector3 MultiplyVector3(Geometry.Computational.Vector3 v)
        //{
        //    double vx = v.X, vy = v.Y, vz = v.Z,
        //    d = 1 / (this.n41 * vx + this.n42 * vy + this.n43 * vz + this.n44);

        //    Geometry.Computational.Vector3 u = new Geometry.Computational.Vector3();

        //    u.X = (float)((this.n11 * vx + this.n12 * vy + this.n13 * vz + this.n14) * d);
        //    u.Y = (float)((this.n21 * vx + this.n22 * vy + this.n23 * vz + this.n24) * d);
        //    u.Z = (float)((this.n31 * vx + this.n32 * vy + this.n33 * vz + this.n34) * d);

        //    return u;
        //}

        //multiplyVector4: function ( v ) {

        //    var vx = v.x, vy = v.y, vz = v.z, vw = v.w;

        //    v.x = this.n11 * vx + this.n12 * vy + this.n13 * vz + this.n14 * vw;
        //    v.y = this.n21 * vx + this.n22 * vy + this.n23 * vz + this.n24 * vw;
        //    v.z = this.n31 * vx + this.n32 * vy + this.n33 * vz + this.n34 * vw;
        //    v.w = this.n41 * vx + this.n42 * vy + this.n43 * vz + this.n44 * vw;

        //    return v;

        //},

        //rotateAxis: function ( v ) {

        //    var vx = v.x, vy = v.y, vz = v.z;

        //    v.x = vx * this.n11 + vy * this.n12 + vz * this.n13;
        //    v.y = vx * this.n21 + vy * this.n22 + vz * this.n23;
        //    v.z = vx * this.n31 + vy * this.n32 + vz * this.n33;

        //    v.normalize();

        //    return v;

        //},

        //crossVector: function ( a ) {

        //    var v = new THREE.Vector4();

        //    v.x = this.n11 * a.x + this.n12 * a.y + this.n13 * a.z + this.n14 * a.w;
        //    v.y = this.n21 * a.x + this.n22 * a.y + this.n23 * a.z + this.n24 * a.w;
        //    v.z = this.n31 * a.x + this.n32 * a.y + this.n33 * a.z + this.n34 * a.w;

        //    v.w = ( a.w ) ? this.n41 * a.x + this.n42 * a.y + this.n43 * a.z + this.n44 * a.w : 1;

        //    return v;

        //},

        public double Determinant()
        {
            //TODO: make this more efficient
            //( based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm )

            return (
                n14 * n23 * n32 * n41 -
                n13 * n24 * n32 * n41 -
                n14 * n22 * n33 * n41 +
                n12 * n24 * n33 * n41 +

                n13 * n22 * n34 * n41 -
                n12 * n23 * n34 * n41 -
                n14 * n23 * n31 * n42 +
                n13 * n24 * n31 * n42 +

                n14 * n21 * n33 * n42 -
                n11 * n24 * n33 * n42 -
                n13 * n21 * n34 * n42 +
                n11 * n23 * n34 * n42 +

                n14 * n22 * n31 * n43 -
                n12 * n24 * n31 * n43 -
                n14 * n21 * n32 * n43 +
                n11 * n24 * n32 * n43 +

                n12 * n21 * n34 * n43 -
                n11 * n22 * n34 * n43 -
                n13 * n22 * n31 * n44 +
                n12 * n23 * n31 * n44 +

                n13 * n21 * n32 * n44 -
                n11 * n23 * n32 * n44 -
                n12 * n21 * n33 * n44 +
                n11 * n22 * n33 * n44
            );
        }

        public Matrix4 Transpose()
        {
            return new Matrix4(
                n11, n21, n31, n41,
                n12, n22, n32, n42,
                n13, n23, n33, n43,
                n14, n24, n34, n44
            );
        }

        //clone: function () {

        //    var m = new THREE.Matrix4();

        //    m.n11 = this.n11; m.n12 = this.n12; m.n13 = this.n13; m.n14 = this.n14;
        //    m.n21 = this.n21; m.n22 = this.n22; m.n23 = this.n23; m.n24 = this.n24;
        //    m.n31 = this.n31; m.n32 = this.n32; m.n33 = this.n33; m.n34 = this.n34;
        //    m.n41 = this.n41; m.n42 = this.n42; m.n43 = this.n43; m.n44 = this.n44;

        //    return m;

        //},

        //flatten: function () {

        //    this.flat[ 0 ] = this.n11; this.flat[ 1 ] = this.n21; this.flat[ 2 ] = this.n31; this.flat[ 3 ] = this.n41;
        //    this.flat[ 4 ] = this.n12; this.flat[ 5 ] = this.n22; this.flat[ 6 ] = this.n32; this.flat[ 7 ] = this.n42;
        //    this.flat[ 8 ]  = this.n13; this.flat[ 9 ]  = this.n23; this.flat[ 10 ] = this.n33; this.flat[ 11 ] = this.n43;
        //    this.flat[ 12 ] = this.n14; this.flat[ 13 ] = this.n24; this.flat[ 14 ] = this.n34; this.flat[ 15 ] = this.n44;

        //    return this.flat;

        //},

        //flattenToArray: function ( flat ) {

        //    flat[ 0 ] = this.n11; flat[ 1 ] = this.n21; flat[ 2 ] = this.n31; flat[ 3 ] = this.n41;
        //    flat[ 4 ] = this.n12; flat[ 5 ] = this.n22; flat[ 6 ] = this.n32; flat[ 7 ] = this.n42;
        //    flat[ 8 ]  = this.n13; flat[ 9 ]  = this.n23; flat[ 10 ] = this.n33; flat[ 11 ] = this.n43;
        //    flat[ 12 ] = this.n14; flat[ 13 ] = this.n24; flat[ 14 ] = this.n34; flat[ 15 ] = this.n44;

        //    return flat;

        //},

        //flattenToArrayOffset: function( flat, offset ) {

        //    flat[ offset ] = this.n11;
        //    flat[ offset + 1 ] = this.n21;
        //    flat[ offset + 2 ] = this.n31;
        //    flat[ offset + 3 ] = this.n41;

        //    flat[ offset + 4 ] = this.n12;
        //    flat[ offset + 5 ] = this.n22;
        //    flat[ offset + 6 ] = this.n32;
        //    flat[ offset + 7 ] = this.n42;

        //    flat[ offset + 8 ]  = this.n13;
        //    flat[ offset + 9 ]  = this.n23;
        //    flat[ offset + 10 ] = this.n33;
        //    flat[ offset + 11 ] = this.n43;

        //    flat[ offset + 12 ] = this.n14;
        //    flat[ offset + 13 ] = this.n24;
        //    flat[ offset + 14 ] = this.n34;
        //    flat[ offset + 15 ] = this.n44;

        //    return flat;

        //},

        public static Matrix4 Translation(double x, double y, double z)
        {
            return new Matrix4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
            );
        }

        public static Matrix4 Scale(double x, double y, double z)
        {
            return new Matrix4(
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationX(double theta)
        {
            double c = Math.Cos(theta), s = Math.Sin(theta);

            return new Matrix4(
                1, 0, 0, 0,
                0, c, -s, 0,
                0, s, c, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationY(double theta)
        {
            double c = Math.Cos(theta), s = Math.Sin(theta);

            return new Matrix4(
                 c, 0, s, 0,
                 0, 1, 0, 0,
                -s, 0, c, 0,
                 0, 0, 0, 1
            );
        }

        public static Matrix4 RotationZ(double theta)
        {
            double c = Math.Cos(theta), s = Math.Sin(theta);

            return new Matrix4(
                c, -s, 0, 0,
                s, c, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationAxis(Vector3D axis, double angle)
        {
            // Based on http://www.gamedev.net/reference/articles/article1199.asp

            double c = Math.Cos(angle),
            s = Math.Sin(angle),
            t = 1 - c,
            x = axis.x, y = axis.y, z = axis.z,
            tx = t * x, ty = t * y;

            return new Matrix4(
                tx * x + c, tx * y - s * z, tx * z + s * y, 0,
                tx * y + s * z, ty * y + c, ty * z - s * x, 0,
                tx * z - s * y, ty * z + s * x, t * z * z + c, 0,
                0, 0, 0, 1
            );
        }

        //setPosition: function ( v ) {

        //    this.n14 = v.x;
        //    this.n24 = v.y;
        //    this.n34 = v.z;

        //    return this;

        //},

        //getPosition: function () {

        //    return THREE.Matrix4.__v1.set( this.n14, this.n24, this.n34 );

        //},

        //getColumnX: function () {

        //    return THREE.Matrix4.__v1.set( this.n11, this.n21, this.n31 );

        //},

        //getColumnY: function () {

        //    return THREE.Matrix4.__v1.set( this.n12, this.n22, this.n32 );

        //},

        //getColumnZ: function() {

        //    return THREE.Matrix4.__v1.set( this.n13, this.n23, this.n33 );

        //},

        public Matrix4 Inverse()
        {
            // based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm

            Matrix4 m = new Matrix4();

            m.n11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
            m.n12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
            m.n13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
            m.n14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;
            m.n21 = n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44;
            m.n22 = n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44;
            m.n23 = n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44;
            m.n24 = n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34;
            m.n31 = n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44;
            m.n32 = n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44;
            m.n33 = n13 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44;
            m.n34 = n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34;
            m.n41 = n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43;
            m.n42 = n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43;
            m.n43 = n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43;
            m.n44 = n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33;
            m = m.MultiplyScalar(1 / m.Determinant());

            return m;
        }

        //setRotationFromEuler: function( v, order ) {

        //    var x = v.x, y = v.y, z = v.z,
        //    a = Math.cos( x ), b = Math.sin( x ),
        //    c = Math.cos( y ), d = Math.sin( y ),
        //    e = Math.cos( z ), f = Math.sin( z );

        //    switch ( order ) {

        //        case 'YXZ':

        //            var ce = c * e, cf = c * f, de = d * e, df = d * f;

        //            this.n11 = ce + df * b;
        //            this.n12 = de * b - cf;
        //            this.n13 = a * d;

        //            this.n21 = a * f;
        //            this.n22 = a * e;
        //            this.n23 = - b;

        //            this.n31 = cf * b - de;
        //            this.n32 = df + ce * b;
        //            this.n33 = a * c;
        //            break;

        //        case 'ZXY':

        //            var ce = c * e, cf = c * f, de = d * e, df = d * f;

        //            this.n11 = ce - df * b;
        //            this.n12 = - a * f;
        //            this.n13 = de + cf * b;

        //            this.n21 = cf + de * b;
        //            this.n22 = a * e;
        //            this.n23 = df - ce * b;

        //            this.n31 = - a * d;
        //            this.n32 = b;
        //            this.n33 = a * c;
        //            break;

        //        case 'ZYX':

        //            var ae = a * e, af = a * f, be = b * e, bf = b * f;

        //            this.n11 = c * e;
        //            this.n12 = be * d - af;
        //            this.n13 = ae * d + bf;

        //            this.n21 = c * f;
        //            this.n22 = bf * d + ae;
        //            this.n23 = af * d - be;

        //            this.n31 = - d;
        //            this.n32 = b * c;
        //            this.n33 = a * c;
        //            break;

        //        case 'YZX':

        //            var ac = a * c, ad = a * d, bc = b * c, bd = b * d;

        //            this.n11 = c * e;
        //            this.n12 = bd - ac * f;
        //            this.n13 = bc * f + ad;

        //            this.n21 = f;
        //            this.n22 = a * e;
        //            this.n23 = - b * e;

        //            this.n31 = - d * e;
        //            this.n32 = ad * f + bc;
        //            this.n33 = ac - bd * f;
        //            break;

        //        case 'XZY':

        //            var ac = a * c, ad = a * d, bc = b * c, bd = b * d;

        //            this.n11 = c * e;
        //            this.n12 = - f;
        //            this.n13 = d * e;

        //            this.n21 = ac * f + bd;
        //            this.n22 = a * e;
        //            this.n23 = ad * f - bc;

        //            this.n31 = bc * f - ad;
        //            this.n32 = b * e;
        //            this.n33 = bd * f + ac;
        //            break;

        //        default: // 'XYZ'

        //            var ae = a * e, af = a * f, be = b * e, bf = b * f;

        //            this.n11 = c * e;
        //            this.n12 = - c * f;
        //            this.n13 = d;

        //            this.n21 = af + be * d;
        //            this.n22 = ae - bf * d;
        //            this.n23 = - b * c;

        //            this.n31 = bf - ae * d;
        //            this.n32 = be + af * d;
        //            this.n33 = a * c;
        //            break;

        //    }

        //    return this;

        //},


        //setRotationFromQuaternion: function( q ) {

        //    var x = q.x, y = q.y, z = q.z, w = q.w,
        //    x2 = x + x, y2 = y + y, z2 = z + z,
        //    xx = x * x2, xy = x * y2, xz = x * z2,
        //    yy = y * y2, yz = y * z2, zz = z * z2,
        //    wx = w * x2, wy = w * y2, wz = w * z2;

        //    this.n11 = 1 - ( yy + zz );
        //    this.n12 = xy - wz;
        //    this.n13 = xz + wy;

        //    this.n21 = xy + wz;
        //    this.n22 = 1 - ( xx + zz );
        //    this.n23 = yz - wx;

        //    this.n31 = xz - wy;
        //    this.n32 = yz + wx;
        //    this.n33 = 1 - ( xx + yy );

        //    return this;

        //},

        //    scale: function ( v ) {

        //        var x = v.x, y = v.y, z = v.z;

        //        this.n11 *= x; this.n12 *= y; this.n13 *= z;
        //        this.n21 *= x; this.n22 *= y; this.n23 *= z;
        //        this.n31 *= x; this.n32 *= y; this.n33 *= z;
        //        this.n41 *= x; this.n42 *= y; this.n43 *= z;

        //        return this;

        //    },

        //    compose: function ( translation, rotation, scale ) {

        //        var mRotation = THREE.Matrix4.__m1;
        //        var mScale = THREE.Matrix4.__m2;

        //        mRotation.identity();
        //        mRotation.setRotationFromQuaternion( rotation );

        //        mScale.setScale( scale.x, scale.y, scale.z );

        //        this.multiply( mRotation, mScale );

        //        this.n14 = translation.x;
        //        this.n24 = translation.y;
        //        this.n34 = translation.z;

        //        return this;

        //    },

        //    decompose: function ( translation, rotation, scale ) {

        //        // grab the axis vectors

        //        var x = THREE.Matrix4.__v1;
        //        var y = THREE.Matrix4.__v2;
        //        var z = THREE.Matrix4.__v3;

        //        x.set( this.n11, this.n21, this.n31 );
        //        y.set( this.n12, this.n22, this.n32 );
        //        z.set( this.n13, this.n23, this.n33 );

        //        translation = ( translation instanceof THREE.Vector3 ) ? translation : new THREE.Vector3();
        //        rotation = ( rotation instanceof THREE.Quaternion ) ? rotation : new THREE.Quaternion();
        //        scale = ( scale instanceof THREE.Vector3 ) ? scale : new THREE.Vector3();

        //        scale.x = x.length();
        //        scale.y = y.length();
        //        scale.z = z.length();

        //        translation.x = this.n14;
        //        translation.y = this.n24;
        //        translation.z = this.n34;

        //        // scale the rotation part

        //        var matrix = THREE.Matrix4.__m1;

        //        matrix.copy( this );

        //        matrix.n11 /= scale.x;
        //        matrix.n21 /= scale.x;
        //        matrix.n31 /= scale.x;

        //        matrix.n12 /= scale.y;
        //        matrix.n22 /= scale.y;
        //        matrix.n32 /= scale.y;

        //        matrix.n13 /= scale.z;
        //        matrix.n23 /= scale.z;
        //        matrix.n33 /= scale.z;

        //        rotation.setFromRotationMatrix( matrix );

        //        return [ translation, rotation, scale ];

        //    },

        //    extractPosition: function ( m ) {

        //        this.n14 = m.n14;
        //        this.n24 = m.n24;
        //        this.n34 = m.n34;

        //        return this;

        //    },

        public Matrix4 ExtractRotation()
        {
            Matrix4 m = new Matrix4();

            var scaleX = 1 / new Vector3D(n11, n21, n31).Abs;
            var scaleY = 1 / new Vector3D(n12, n22, n32).Abs;
            var scaleZ = 1 / new Vector3D(n13, n23, n33).Abs;

            m.n11 = n11 * scaleX;
            m.n21 = n21 * scaleX;
            m.n31 = n31 * scaleX;

            m.n12 = n12 * scaleY;
            m.n22 = n22 * scaleY;
            m.n32 = n32 * scaleY;

            m.n13 = n13 * scaleZ;
            m.n23 = n23 * scaleZ;
            m.n33 = n33 * scaleZ;

            m.n44 = 1;

            return m;
        }

        //THREE.Matrix4.makeInvert3x3 = function ( m1 ) {

        //    // input:  THREE.Matrix4, output: THREE.Matrix3
        //    // ( based on http://code.google.com/p/webgl-mjs/ )

        //    var m33 = m1.m33, m33m = m33.m,
        //    a11 =   m1.n33 * m1.n22 - m1.n32 * m1.n23,
        //    a21 = - m1.n33 * m1.n21 + m1.n31 * m1.n23,
        //    a31 =   m1.n32 * m1.n21 - m1.n31 * m1.n22,
        //    a12 = - m1.n33 * m1.n12 + m1.n32 * m1.n13,
        //    a22 =   m1.n33 * m1.n11 - m1.n31 * m1.n13,
        //    a32 = - m1.n32 * m1.n11 + m1.n31 * m1.n12,
        //    a13 =   m1.n23 * m1.n12 - m1.n22 * m1.n13,
        //    a23 = - m1.n23 * m1.n11 + m1.n21 * m1.n13,
        //    a33 =   m1.n22 * m1.n11 - m1.n21 * m1.n12,

        //    det = m1.n11 * a11 + m1.n21 * a12 + m1.n31 * a13,

        //    idet;

        //    // no inverse

        //    if ( det === 0 ) {

        //        console.error( 'THREE.Matrix4.makeInvert3x3: Matrix not invertible.' );

        //    }

        //    idet = 1.0 / det;

        //    m33m[ 0 ] = idet * a11; m33m[ 1 ] = idet * a21; m33m[ 2 ] = idet * a31;
        //    m33m[ 3 ] = idet * a12; m33m[ 4 ] = idet * a22; m33m[ 5 ] = idet * a32;
        //    m33m[ 6 ] = idet * a13; m33m[ 7 ] = idet * a23; m33m[ 8 ] = idet * a33;

        //    return m33;

        //}

        public static Matrix4 Frustum(double left, double right, double top, double bottom, double near, double far)
        {
            double x, y, a, b, c, d;

            Matrix4 m = new Matrix4();

            x = 2 * near / (right - left);
            y = 2 * near / (top - bottom);

            a = (right + left) / (right - left);
            b = (top + bottom) / (top - bottom);
            c = -(far + near) / (far - near);
            d = -2 * far * near / (far - near);

            m.n11 = x; m.n12 = 0; m.n13 = a; m.n14 = 0;
            m.n21 = 0; m.n22 = y; m.n23 = b; m.n24 = 0;
            m.n31 = 0; m.n32 = 0; m.n33 = c; m.n34 = d;
            m.n41 = 0; m.n42 = 0; m.n43 = -1; m.n44 = 0;

            return m;
        }

        public static Matrix4 Perspective(double fov, double aspect, double near, double far)
        {
            double ymax, ymin, xmin, xmax;

            ymax = near * Math.Tan(fov * Math.PI / 360);
            ymin = -ymax;
            xmin = ymin * aspect;
            xmax = ymax * aspect;

            return Frustum(xmin, xmax, ymin, ymax, near, far);
        }

        public static Matrix4 Ortho(double left, double right, double top, double bottom, double near, double far)
        {
            double x, y, z, w, h, p;

            Matrix4 m = new Matrix4();

            w = right - left;
            h = top - bottom;
            p = far - near;

            x = (right + left) / w;
            y = (top + bottom) / h;
            z = (far + near) / p;

            m.n11 = 2 / w; m.n12 = 0; m.n13 = 0; m.n14 = -x;
            m.n21 = 0; m.n22 = 2 / h; m.n23 = 0; m.n24 = -y;
            m.n31 = 0; m.n32 = 0; m.n33 = -2 / p; m.n34 = -z;
            m.n41 = 0; m.n42 = 0; m.n43 = 0; m.n44 = 1;

            return m;
        }

        //THREE.Matrix4.__v1 = new THREE.Vector3();
        //THREE.Matrix4.__v2 = new THREE.Vector3();
        //THREE.Matrix4.__v3 = new THREE.Vector3();

        //THREE.Matrix4.__m1 = new THREE.Matrix4();
        //THREE.Matrix4.__m2 = new THREE.Matrix4();
    }
}
