using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silverlight3dApp
{
    public abstract class Camera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public float AspectRatio { get; set; }
        protected GraphicsDevice GraphicsDevice { get; set; }

        public Camera(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
        }

        public virtual void Update()
        {
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), AspectRatio, 0.1f, 1000000.0f);
        }

        //public virtual void OnMouseMove()
        //{
        //}

        //public virtual void OnMouseWheel()
        //{
        //}

        //public virtual void HandleKeys()
        //{
        //}
    }

    public interface IZoomCamera
    {
        float Distance { get; }
        void Zoom(float distChange);
    }

    public interface IWASDMouseCamera
    {
        void Move(float forback, float leftright);
        void RotateLeftRight(float angle);
        void RotateUpDown(float angle);
    }

    public class TargetCamera : Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }

        public TargetCamera(Vector3 Position, Vector3 Target, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Position = Position;
            this.Target = Target;
        }

        public override void Update()
        {
            base.Update();

            Vector3 forward = Target - Position;
            Vector3 side = Vector3.Cross(forward, Vector3.Up);
            Vector3 up = Vector3.Cross(forward, side);
            this.View = Matrix.CreateLookAt(Position, Target, up);
        }
    }

    public class FreeCamera : Camera
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; private set; }
        private Vector3 translation;

        public FreeCamera(Vector3 Position, float Yaw, float Pitch, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Position = Position;
            this.Yaw = Yaw;
            this.Pitch = Pitch;
            translation = Vector3.Zero;
        }

        public void Rotate(float YawChange, float PitchChange)
        {
            this.Yaw += YawChange;
            this.Pitch += PitchChange;
        }

        public void Move(Vector3 Translation)
        {
            this.translation += Translation;
        }

        public override void Update()
        {
            base.Update();

            // Calculate the rotation matrix
            Matrix rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0);

            // Offset the position and reset the translation
            translation = Vector3.Transform(translation, rotation);
            Position += translation;
            translation = Vector3.Zero;

            // Calculate the new target
            Vector3 forward = Vector3.Transform(Vector3.UnitX, rotation);
            Target = Position + forward;

            // Calculate the up vector
            Vector3 up = Vector3.Transform(Vector3.UnitZ, rotation);

            // Calculate the view matrix
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }

    /// <summary>
    /// 自由飞行摄影机
    /// </summary>
    public class FreeFlyCamera : Camera
    {
        public Vector3 Forward { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Position { get; set; }

        public FreeFlyCamera(Vector3 pos, Vector3 forward, Vector3 up, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Position = pos;
            this.Forward = forward;
            this.Up = up;
        }

        public void Move(float forback, float leftright)
        {
            this.Forward.Normalize();
            this.Up.Normalize();
            var left = Vector3.Cross(this.Up, this.Forward);

            Position += forback * this.Forward + leftright * left;
        }

        public void RotateLeftRight(float angle)
        {
            this.Forward = Vector3.Transform(this.Forward, Matrix.CreateFromAxisAngle(this.Up, angle));
        }

        public void RotateUpDown(float angle)
        {
            var left = Vector3.Cross(this.Up, this.Forward);
            var up = Vector3.Transform(this.Up, Matrix.CreateFromAxisAngle(left, angle));
            if (up.Z != 0)
            {
                this.Up = Vector3.Transform(this.Up, Matrix.CreateFromAxisAngle(left, angle));
                this.Forward = Vector3.Transform(this.Forward, Matrix.CreateFromAxisAngle(left, angle));
            }
        }

        public override void Update()
        {
            base.Update();

            View = Matrix.CreateLookAt(Position, Position + Forward, Up);
        }
    }

    /// <summary>
    /// 单轴水平飞行摄影机。机翼所在轴保持水平。
    /// </summary>
    public class OneAxisLevelFlyCamera : Camera
    {
        public Vector3 Forward { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Position { get; set; }

        public OneAxisLevelFlyCamera(Vector3 pos, Vector3 forward, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Position = pos;
            this.Forward = forward;
            this.Up = Vector3.UnitZ;
        }

        public void Move(float forback, float leftright)
        {
            this.Forward.Normalize();
            this.Up.Normalize();
            var left = Vector3.Cross(this.Up, this.Forward);

            Position += forback * this.Forward + leftright * left;
        }

        public void RotateLeftRight(float angle)
        {
            this.Forward = Vector3.Transform(this.Forward, Matrix.CreateFromAxisAngle(this.Up, angle));
        }

        public void RotateUpDown(float angle)
        {
            var left = Vector3.Cross(this.Up, this.Forward);
            var forward = Vector3.Transform(this.Forward, Matrix.CreateFromAxisAngle(left, angle));
            if (forward.X != 0 && forward.Y != 0)
            {
                this.Forward = forward;
            }
        }

        public override void Update()
        {
            base.Update();

            View = Matrix.CreateLookAt(Position, Position + Forward, Up);
        }
    }

    /// <summary>
    /// 双轴水平飞行摄影机。机翼、机身所在轴均保持水平。
    /// </summary>
    public class TwoAxesLevelFlyCamera : Camera
    {
        public Vector3 Forward { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Position { get; set; }

        public TwoAxesLevelFlyCamera(Vector3 pos, Vector3 forward, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Position = pos;
            this.Forward = new Vector3(forward.X, forward.Y, 0);
            this.Up = Vector3.UnitZ;
        }

        public void Move(float forback, float leftright)
        {
            this.Forward.Normalize();
            this.Up.Normalize();
            var left = Vector3.Cross(this.Up, this.Forward);

            Position += forback * this.Forward + leftright * left;
        }

        public void RotateLeftRight(float angle)
        {
            this.Forward = Vector3.Transform(this.Forward, Matrix.CreateFromAxisAngle(this.Up, angle));
        }

        public void RotateUpDown(float angle)
        {
            Position += new Vector3(0, 0, -angle);
        }

        public override void Update()
        {
            base.Update();

            View = Matrix.CreateLookAt(Position, Position + Forward, Up);
        }
    }

    public class FixAxisCamera : Camera, IZoomCamera
    {
        // Rotation around the two axes
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        // Distance between the target and camera
        public float Distance { get; set; }
        private Vector3 _dirA0;
        private Vector3 _dirB0;

        // Distance limits
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }

        // Calculated position and specified target
        public Vector3 Position { get; private set; }
        //public Vector3 Target { get; set; }
        public Vector3 Center { get; set; }

        public FixAxisCamera(Vector3 pos, Vector3 target, Vector3 center, float yaw, float minDist, float maxDist, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            float dist = (pos - target).Length();

            // Lock the distance between the min and max values
            this.Distance = MathHelper.Clamp(dist, minDist, maxDist);

            var dir = pos - target;
            dir.Normalize();
            this._dirA0 = dir;
            this._dirB0 = target - center;

            this.Position = target + this.Distance * this._dirA0;
            //this.Target = target;
            this.Center = center;

            // Lock the x axis rotation between the min and max values
            this.Yaw = yaw;
            this.MinDistance = minDist;
            this.MaxDistance = maxDist;
        }

        public void Zoom(float distChange)
        {
            this.Distance += distChange;
            this.Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Rotate(float yawChange)
        {
            this.Yaw += yawChange;
        }

        public void Translate(Vector3 posChange)
        {
            this.Position += posChange;
        }

        public override void Update()
        {
            base.Update();

            // Calculate rotation matrix from rotation values
            Matrix rotation = Matrix.CreateFromYawPitchRoll(0, 0, Yaw);

            // Calculate position
            Vector3 translation = Distance * _dirA0; //new Vector3(0, 0, Distance);
            translation = Vector3.Transform(translation, rotation);
            var target = Center + Vector3.Transform(_dirB0, rotation);
            Position = target + translation;

            // Calculate the up vector from the rotation matrix
            Vector3 up = Vector3.Transform(Vector3.UnitZ, rotation);
            View = Matrix.CreateLookAt(Position, target, up);
        }
    }

    public class LevelCamera : Camera, IZoomCamera
    {
        // Rotation around the two axes
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public Vector3 Translation { get; set; }

        // Distance between the target and camera
        public float Distance { get; set; }

        // Distance limits
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }

        // Calculated position and specified target
        public Vector3 Position { get; private set; }
        public Vector3 Center { get; set; }

        public LevelCamera(Vector3 pos, Vector3 center, float minDist, float maxDist, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            Vector3 target = new Vector3(center.X, center.Y, pos.Z);
            float dist = (pos - target).Length();

            // Lock the distance between the min and max values
            this.Distance = MathHelper.Clamp(dist, minDist, maxDist);

            var dir = pos - target;
            dir.Normalize();
            this.Position = target + this.Distance * dir;
            this.Center = center;

            this.Yaw = 0;
            this.MinDistance = minDist;
            this.MaxDistance = maxDist;
        }

        public void Zoom(float distChange)
        {
            this.Distance += distChange;
            this.Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Rotate(float yawChange)
        {
            this.Yaw += yawChange;
        }

        public void Translate(Vector3 posChange)
        {
            this.Translation += posChange;
        }

        public override void Update()
        {
            base.Update();

            // Calculate rotation matrix from rotation values
            Matrix rotation = Matrix.CreateFromYawPitchRoll(0, 0, Yaw);

            // Calculate position
            Vector3 target = new Vector3(Center.X, Center.Y, Position.Z);
            var dir = Position - target;
            dir.Normalize();
            Vector3 translation = this.Distance * dir;
            translation = Vector3.Transform(translation, rotation);
            this.Position = target + translation;
            this.Position += this.Translation;

            // Clear vars that was added to position
            this.Yaw = 0;
            this.Translation = Vector3.Zero;

            // Calculate the up vector from the rotation matrix
            Vector3 up = Vector3.Transform(Vector3.UnitZ, rotation);
            base.View = Matrix.CreateLookAt(this.Position, target, up);
        }
    }

    public class ArcBallCamera : Camera, IZoomCamera
    {
        // Rotation around the two axes
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public Vector3 Translation { get; set; }

        // X axis rotation limits (radians)
        public float MinPitch { get; set; }
        public float MaxPitch { get; set; }

        // Distance between the target and camera
        public float Distance { get; set; }

        // Distance limits
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }

        // Calculated position and specified target
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }

        public bool FixAxis { get; set; }

        public ArcBallCamera(Vector3 pos, Vector3 target, float minPitch, float maxPitch, float minDist, float maxDist, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            float dist = (pos - target).Length();

            // Lock the distance between the min and max values
            this.Distance = MathHelper.Clamp(dist, minDist, maxDist);

            var dir = pos - target;
            dir.Normalize();
            this.Position = target + this.Distance * dir;
            this.Target = target;
            this.Up = Vector3.Cross(dir, Vector3.Cross(Vector3.UnitZ, dir));
            this.Up.Normalize();
            this.MinPitch = minPitch;
            this.MaxPitch = maxPitch;

            // Lock the x axis rotation between the min and max values
            this.Pitch = 0;
            this.Yaw = 0;
            this.MinDistance = minDist;
            this.MaxDistance = maxDist;

            this.FixAxis = true;
        }

        public void Zoom(float distChange)
        {
            this.Distance += distChange;
            this.Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Rotate(float yawChange, float pitchChange)
        {
            this.Yaw += yawChange;
            this.Pitch += -pitchChange;
            this.Pitch = MathHelper.Clamp(Pitch, MinPitch, MaxPitch);
        }

        public void Translate(Vector3 posChange)
        {
            this.Translation += posChange;
        }

        public override void Update()
        {
            base.Update();

            // Calculate rotation matrix from rotation values
            var dir = this.Position - this.Target;
            dir.Normalize();
            var dir1 = this.Up;
            dir1.Normalize();
            var dir2 = Vector3.Cross(dir, dir1);
            dir2.Normalize();
            Matrix rotation1 = Matrix.CreateFromAxisAngle(dir1, Yaw);
            Matrix rotation2 = Matrix.CreateFromAxisAngle(dir2, Pitch);
            Matrix rotation = rotation1 * rotation2;

            // Calculate position
            Vector3 translation = this.Distance * dir;
            translation = Vector3.Transform(translation, rotation);
            this.Position = this.Target + translation;
            this.Position += this.Translation;

            // Clear vars that was added to position
            this.Yaw = 0;
            this.Pitch = 0;
            this.Translation = Vector3.Zero;

            // Calculate the up vector from the rotation matrix 
            Vector3 up0 = FixAxis ? Vector3.UnitZ : this.Up;
            this.Up = Vector3.Transform(up0, rotation);
            base.View = Matrix.CreateLookAt(this.Position, this.Target, this.Up);
        }
    }

    public class ArcBallCamera1 : Camera
    {
        // Rotation around the two axes
        public float RotationX { get; set; }
        public float RotationY { get; set; }

        // Y axis rotation limits (radians)
        public float MinRotationY { get; set; }
        public float MaxRotationY { get; set; }

        // Distance between the target and camera
        public float Distance { get; set; }

        // Distance limits
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }

        // Calculated position and specified target
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; set; }

        public ArcBallCamera1(Vector3 Target, float RotationX, float RotationY, float MinRotationY, float MaxRotationY, float Distance, float MinDistance, float MaxDistance, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Target = Target;
            this.MinRotationY = MinRotationY;
            this.MaxRotationY = MaxRotationY;

            // Lock the y axis rotation between the min and max values
            this.RotationY = MathHelper.Clamp(RotationY, MinRotationY, MaxRotationY);
            this.RotationX = RotationX;
            this.MinDistance = MinDistance;
            this.MaxDistance = MaxDistance;

            // Lock the distance between the min and max values
            this.Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Move(float DistanceChange)
        {
            this.Distance += DistanceChange;
            this.Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Rotate(float RotationXChange, float RotationYChange)
        {
            this.RotationX += RotationXChange;
            this.RotationY += -RotationYChange;
            this.RotationY = MathHelper.Clamp(RotationY, MinRotationY, MaxRotationY);
        }

        public void Translate(Vector3 PositionChange)
        {
            this.Position += PositionChange;
        }

        public override void Update()
        {
            base.Update();

            // Calculate rotation matrix from rotation values
            Matrix rotation = Matrix.CreateFromYawPitchRoll(RotationX, -RotationY, 0);

            // Translate down the Z axis by the desired distance
            // between the camera and object, then rotate that
            // vector to find the camera offset from the target
            Vector3 translation = new Vector3(0, 0, Distance);
            translation = Vector3.Transform(translation, rotation);
            Position = Target + translation;

            // Calculate the up vector from the rotation matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }

    public class ChaseCamera : Camera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }

        public Vector3 FollowTargetPosition { get; private set; }
        public Vector3 FollowTargetRotation { get; private set; }

        public Vector3 PositionOffset { get; set; }
        public Vector3 TargetOffset { get; set; }

        public Vector3 RelativeCameraRotation { get; set; }

        float springiness = .15f;
        public float Springiness
        {
            get { return springiness; }
            set { springiness = MathHelper.Clamp(value, 0, 1); }
        }

        public ChaseCamera(Vector3 PositionOffset, Vector3 TargetOffset, Vector3 RelativeCameraRotation, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.PositionOffset = PositionOffset;
            this.TargetOffset = TargetOffset;
            this.RelativeCameraRotation = RelativeCameraRotation;
        }

        public void Move(Vector3 NewFollowTargetPosition, Vector3 NewFollowTargetRotation)
        {
            this.FollowTargetPosition = NewFollowTargetPosition;
            this.FollowTargetRotation = NewFollowTargetRotation;
        }

        public void Rotate(Vector3 RotationChange)
        {
            this.RelativeCameraRotation += RotationChange;
        }

        public override void Update()
        {
            base.Update();

            // Sum the rotations of the model and the camera to ensure it
            // is rotated to the correct position relative to the model's
            // rotation
            Vector3 combinedRotation = FollowTargetRotation + RelativeCameraRotation;

            // Calculate the rotation matrix for the camera
            Matrix rotation = Matrix.CreateFromYawPitchRoll(combinedRotation.Y, combinedRotation.X, combinedRotation.Z);

            // Calculate the position the camera would be without the spring
            // value, using the rotation matrix and target position
            Vector3 desiredPosition = FollowTargetPosition + Vector3.Transform(PositionOffset, rotation);

            // Interpolate between the current position and desired position
            Position = Vector3.Lerp(Position, desiredPosition, Springiness);

            // Calculate the new target using the rotation matrix
            Target = FollowTargetPosition + Vector3.Transform(TargetOffset, rotation);

            // Obtain the up vector from the matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            // Recalculate the view matrix
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}
