using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Weapons
{
    public enum Axis
    {
        X,
        Y
    }

    public struct TeleportBoundary
    {
        public float Start;
        public float End;
        public PlayerLevelManager From;
        public PlayerLevelManager To;
        public Axis Axis;

        public override string ToString()
        {
            return $"{Start} -> {End}, {From} -> {To}, {Axis}";
        }
    }

    public class Projectile : MonoBehaviour
    {
        private static List<TeleportBoundary> xHeadingLeft;

        private static List<TeleportBoundary> xHeadingRight;

        private static List<TeleportBoundary> yHeadingDown;

        private static List<TeleportBoundary> yHeadingUp;

        public float projectileSpeed = 10.0f;

        public GameObject prefab;

        private TeleportBoundary? _currentBoundary;
        private bool _finishedReceiving;

        private bool _finishedSending;

        private int _frameCount;

        private bool _fromTeleport;

        private float _lastXBoundary = float.NaN;

        private float _lastYBoundary = float.NaN;
        private Vector2 _myLastFrame;

        private PlayerLevelManager _myLevelManager;
        private PlayerLevelManager _other;

        private Vector2 _otherLastFrame;

        private bool _teleported;

        protected Collider2D Collider;

        protected Rigidbody2D Rb;

        private void Start()
        {
            Destroy(gameObject, 10);
            Rb = GetComponent<Rigidbody2D>();
            _teleported = false;
            _frameCount = 0;
            Collider = GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            _frameCount++;
            if (Rb.linearVelocity.y < 0)
            {
                if (!_finishedReceiving && _currentBoundary.HasValue)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;
                    Rb.position = new Vector2(Rb.position.x - xDiff, Rb.position.y);
                    if (Rb.position.y < _currentBoundary.Value.End)
                    {
                        Collider.enabled = true;
                        _finishedReceiving = true;
                        _currentBoundary = null;
                    }
                }
                else if (_teleported && _finishedSending)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;
                    Rb.position = new Vector2(Rb.position.x - xDiff, Rb.position.y);
                }
                else if (_teleported && !_finishedSending && _currentBoundary.HasValue)
                {
                    if (Rb.position.y < _currentBoundary.Value.Start)
                    {
                        _currentBoundary = null;
                        _finishedSending = true;
                        Collider.enabled = true;
                    }
                }


                foreach (var kvp in yHeadingDown)
                    if (Rb.position.y < kvp.Start + 1.0f && Rb.position.y > kvp.End && _finishedReceiving &&
                        !_teleported)
                    {
                        _currentBoundary = kvp;
                        var clone = Instantiate(prefab);
                        clone.transform.position = TransferFrameOfReference(kvp, Rb.position);
                        var proj = clone.GetComponent<Projectile>();
                        proj.prefab = prefab;
                        proj.SetRB(Rb.linearVelocity);
                        proj.SetSender(_currentBoundary.Value, new Vector2(_currentBoundary.Value.From.PosX, 0),
                            new Vector2(_currentBoundary.Value.To.PosX, 0));
                        _other = _currentBoundary.Value.To;
                        _teleported = true;
                        _otherLastFrame = clone.transform.position;
                    }
            }

            else if (Rb.linearVelocity.y > 0)
            {
                if (!_finishedReceiving && _currentBoundary.HasValue)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;

                    Rb.position = new Vector2(Rb.position.x - xDiff, Rb.position.y);
                    if (Rb.position.y > _currentBoundary.Value.End)
                    {
                        _finishedReceiving = true;
                        _currentBoundary = null;
                        Collider.enabled = true;
                    }
                }
                else if (_teleported && _finishedSending)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;
                    Rb.position = new Vector2(Rb.position.x - xDiff, Rb.position.y);
                }
                else if (_teleported && !_finishedSending && _currentBoundary.HasValue)
                {
                    if (Rb.position.y > _currentBoundary.Value.Start)
                    {
                        _currentBoundary = null;
                        _finishedSending = true;
                        Collider.enabled = true;
                    }
                }

                foreach (var kvp in yHeadingUp)
                    if (Rb.position.y > kvp.Start - 1.0f && Rb.position.y < kvp.End && !_teleported &&
                        _finishedReceiving)
                    {
                        _currentBoundary = kvp;
                        var clone = Instantiate(prefab);
                        clone.transform.position = TransferFrameOfReference(kvp, Rb.position);
                        var proj = clone.GetComponent<Projectile>();
                        proj.prefab = prefab;
                        proj.SetRB(Rb.linearVelocity);
                        proj.SetSender(_currentBoundary.Value, new Vector2(_currentBoundary.Value.From.PosX, 0),
                            new Vector2(_currentBoundary.Value.To.PosX, 0));
                        _other = _currentBoundary.Value.To;
                        _otherLastFrame = clone.transform.position;
                        _teleported = true;
                    }
            }

            if (_other) _otherLastFrame.x = _other.PosX;
            if (_myLevelManager) _myLastFrame.x = _myLevelManager.PosX;
        }

        // Called at instantiation when receiving
        public void SetSender(TeleportBoundary boundary, Vector2 senderLastFrame, Vector2 myLastFrame
        )
        {
            _otherLastFrame = new Vector2(senderLastFrame.x, senderLastFrame.y);
            _currentBoundary = boundary;
            _other = boundary.From;
            _myLastFrame = new Vector2(myLastFrame.x, myLastFrame.y);
            _myLevelManager = boundary.To;
            _fromTeleport = true;
            _teleported = false;
        }

        public void MarkInitial()
        {
            _finishedReceiving = true;
        }

        public void SetLevelManager(PlayerLevelManager levelManager)
        {
            _myLevelManager = levelManager;
        }

        private Vector2 TransferFrameOfReference(TeleportBoundary boundary, Vector2 pos)
        {
            if (boundary.Axis == Axis.Y)
            {
                pos.y = boundary.End - (boundary.Start - pos.y);
                var relativeX = boundary.To.PosX - boundary.From.PosX;
                pos.x += relativeX;
            }
            else
            {
                pos.x = boundary.End - (boundary.Start - pos.x);
                var relativeY = boundary.From.transform.position.y - pos.y;
                pos.y = boundary.To.transform.position.y - relativeY;
            }

            return pos;
        }


        public static void SetBoundaries(List<TeleportBoundary> setHeadingLeft, List<TeleportBoundary> setHeadingDown)
        {
            xHeadingLeft = setHeadingLeft;
            xHeadingRight = setHeadingLeft.Select(i =>
            {
                TeleportBoundary boundary;
                boundary.Start = i.End;
                boundary.End = i.Start;
                boundary.Axis = i.Axis;
                boundary.To = i.From;
                boundary.From = i.To;
                return boundary;
            }).ToList();

            yHeadingDown = setHeadingDown;
            yHeadingUp = setHeadingDown.Select(i =>
            {
                TeleportBoundary boundary;
                boundary.Start = i.End;
                boundary.End = i.Start;
                boundary.Axis = i.Axis;
                boundary.To = i.From;
                boundary.From = i.To;
                return boundary;
            }).ToList();
        }

        public void SetRB(Vector2 vel)
        {
            if (!Rb) Rb = GetComponent<Rigidbody2D>();
            Rb.linearVelocity = vel;
        }

        public void InitialRB(Vector2 vel)
        {
            if (!Rb) Rb = GetComponent<Rigidbody2D>();
            Rb.linearVelocity = vel * projectileSpeed;
        }
    }
}