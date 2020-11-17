using UnityEngine;
using System.Collections;

namespace CardFramework {

    public class Card : Pheryus.Card {
        public string TexturePath { get; set; }

        public string SourceAssetBundlePath { get; set; }

        public bool HasTarget;

        public Transform TargetTransform {
            get {
                if (_targetTransform == null && HasTarget) {
                    GameObject gameObject = new GameObject(this.name + "Target");
                    _targetTransform = gameObject.GetComponent<Transform>();
                    _targetTransform.position = transform.position;
                    _targetTransform.SetParent(transform.parent);
                    _targetTransform.localRotation = Quaternion.identity;
                    
                    _targetTransform.forward = transform.forward;
                }
                return _targetTransform;
            }
        }
        private Transform _targetTransform;

        public CardSlot ParentCardSlot { get; set; }

        public int FaceValue { get; set; }

        private float _positionDamp = .2f;

        public bool returnToStartPosition = true;

        Quaternion previousRotation;

        private void Update() {
            if (returnToStartPosition && HasTarget) {
                SmoothToTargetPositionRotation();
            }
        }

        public void SetDamp(float newPositionDamp) {
            _positionDamp = newPositionDamp;
        }

        public bool OnTargetPosition() {
            return TargetTransform.position == transform.position;
        }

        private void SmoothToTargetPositionRotation() {
            if (TargetTransform.position != transform.position || TargetTransform.eulerAngles != transform.eulerAngles) {
                SmoothToPointAndDirection(TargetTransform.position, _positionDamp, TargetTransform.rotation);
            }
        }

        public void SetNewTarget (Vector3 pos, Quaternion rotation) {
            TargetTransform.position = pos;
            TargetTransform.rotation = rotation;
        }

        public void SetRotation (Quaternion rotation) {
            previousRotation = TargetTransform.rotation;
            TargetTransform.rotation = rotation;
        }

        public void ReturnToStartRotation() {
            TargetTransform.rotation = previousRotation;
        }

        private void SmoothToPointAndDirection(Vector3 point, float moveSmooth, Quaternion rotation) {
            transform.position = Vector3.SmoothDamp(transform.position, point, ref _smoothVelocity, moveSmooth);
            if (Vector3.Distance(transform.position, point) < 0.005f) {
                transform.position = point;
            }
            float diff = Quaternion.Angle(transform.rotation, rotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation,
                Mathf.Lerp(0, Mathf.Max(diff, 0.1f), 0.1f));
            TestVisibility();
        }

        private Vector3 _smoothVelocity;
        private Vector4 _smoothRotationVelocity;

        private void TestVisibility() {
            float angle = Vector3.Angle(Camera.main.transform.forward, transform.forward);
            if (angle < 90) {
                FrontBecameVisible();
            }
            else {
                FrontBecameHidden();
            }
        }

        public void FrontBecameVisible() {
            GetComponent<Renderer>().material.mainTexture = CardTexture.instance.GetTextureFromCard(cardInfo);
        }

        public void FrontBecameHidden() {
            GetComponent<Renderer>().material.mainTexture = null;
        }
    }

}