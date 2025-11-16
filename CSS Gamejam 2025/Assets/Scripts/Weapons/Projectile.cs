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

        public float projectileSpeed = 5.0f;

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

        private Rigidbody2D _rb;

        private bool _teleported;

        private void Start()
        {
            Destroy(gameObject, 10);
            _rb = GetComponent<Rigidbody2D>();
            _teleported = false;
            _frameCount = 0;
        }

        private void FixedUpdate()
        {
            _frameCount++;
            if (_rb.linearVelocity.y < 0)
            {
                if (!_finishedReceiving && _currentBoundary.HasValue)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;
                    Debug.Log($"xDiff {xDiff}");
                    _rb.position = new Vector2(_rb.position.x - xDiff, _rb.position.y);
                    if (_rb.position.y < _currentBoundary.Value.End)
                        _finishedReceiving = true;
                }
                else if (_teleported && _finishedSending)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;
                    _rb.position = new Vector2(_rb.position.x - xDiff, _rb.position.y);
                }
                else if (_teleported && !_finishedSending && _currentBoundary.HasValue)
                {
                    if (_rb.position.y < _currentBoundary.Value.Start)
                    {
                        _currentBoundary = null;
                        _finishedSending = true;
                    }
                }


                foreach (var kvp in yHeadingDown)
                    if (_rb.position.y < kvp.Start + 1.0f && _rb.position.y > kvp.End && _finishedReceiving &&
                        !_teleported)
                    {
                        _currentBoundary = kvp;
                        var clone = Instantiate(_myLevelManager.projectilePrefab);
                        Debug.Log($"Mapped from {_rb.position}");
                        clone.transform.position = TransferFrameOfReference(kvp, _rb.position);
                        Debug.Log($"To {clone.transform.position}");
                        var proj = clone.GetComponent<Projectile>();
                        proj.SetRB(_rb.linearVelocity);
                        proj.SetSender(_currentBoundary.Value, new Vector2(_currentBoundary.Value.From.PosX, 0),
                            new Vector2(_currentBoundary.Value.To.PosX, 0));
                        _other = _currentBoundary.Value.To;
                        _teleported = true;
                        _otherLastFrame = clone.transform.position;
                    }
            }

            else if (_rb.linearVelocity.y > 0)
            {
                if (!_finishedReceiving && _currentBoundary.HasValue)
                {
                    // Adjust pos
                    var xDiff = _other.PosX - _otherLastFrame.x;
                    var myXDiff = _myLevelManager.PosX - _myLastFrame.x;
                    xDiff -= myXDiff;
                    Debug.Log($"xDiff {xDiff}");

                    _rb.position = new Vector2(_rb.position.x - xDiff, _rb.position.y);
                    if (_rb.position.y > _currentBoundary.Value.End)
                    {
                        Debug.Log("Finished receiving");
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
                    Debug.Log($"Running on {_fromTeleport}");
                    _rb.position = new Vector2(_rb.position.x - xDiff, _rb.position.y);
                }
                else if (_teleported && !_finishedSending && _currentBoundary.HasValue)
                {
                    if (_rb.position.y > _currentBoundary.Value.Start)
                    {
                        _currentBoundary = null;
                        _finishedSending = true;
                    }
                }

                foreach (var kvp in yHeadingUp)
                    if (_rb.position.y > kvp.Start - 1.0f && _rb.position.y < kvp.End && !_teleported &&
                        _finishedReceiving)
                    {
                        _currentBoundary = kvp;
                        var clone = Instantiate(_myLevelManager.projectilePrefab);
                        Debug.Log($"Mapped from {_rb.position}");
                        clone.transform.position = TransferFrameOfReference(kvp, _rb.position);
                        Debug.Log($"To {clone.transform.position}");
                        var proj = clone.GetComponent<Projectile>();
                        proj.SetRB(_rb.linearVelocity);
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
            Debug.Log($"Boundary: {boundary.ToString()}");
            if (boundary.Axis == Axis.Y)
            {
                pos.y = boundary.End - (boundary.Start - pos.y);
                var relativeX = boundary.To.PosX - boundary.From.PosX;
                Debug.Log($"{boundary.To == boundary.From}");
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

            setHeadingDown.Select(i => i.ToString()).ToList().ForEach(Debug.Log);
            yHeadingUp.Select(i => i.ToString()).ToList().ForEach(Debug.Log);
        }

        public void SetRB(Vector2 vel)
        {
            if (!_rb) _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = vel;
        }

        public void InitialRB(Vector2 vel)
        {
            if (!_rb) _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = vel * projectileSpeed;
        }
    }
}