using UnityEngine;
using System.Collections;

namespace CardFramework {

    public class Card : Pheryus.Card {
        public string TexturePath { get; set; }

        public string SourceAssetBundlePath { get; set; }

        public Transform TargetTransform {
            get {
                if (_targetTransform == null) {
                    GameObject gameObject = new GameObject(this.name + "Target");
                    _targetTransform = gameObject.GetComponent<Transform>();
                    _targetTransform.position = transform.position;
                    _targetTransform.SetParent(transform.parent);
                    _targetTransform.forward = transform.forward;
                }
                return _targetTransform;
            }
        }
        private Transform _targetTransform;

        public CardSlot ParentCardSlot { get; set; }

        public int FaceValue { get; set; }

        private float _positionDamp = .2f;

        private float _rotationDamp = .2f;

        public bool returnToStartPosition = true;

        private void Update() {
            if (returnToStartPosition) {
                SmoothToTargetPositionRotation();
            }
        }

        public void SetDamp(float newPositionDamp, float newRotationDamp) {
            _positionDamp = newPositionDamp;
            _rotationDamp = newRotationDamp;
        }

        public bool OnTargetPosition() {
            return TargetTransform.position == transform.position;
        }

        private void SmoothToTargetPositionRotation() {
            if (TargetTransform.position != transform.position || TargetTransform.eulerAngles != transform.eulerAngles) {
                SmoothToPointAndDirection(TargetTransform.position, _positionDamp, TargetTransform.rotation, _rotationDamp);
            }
        }

        public void SetNewTarget (Vector3 pos, Quaternion rotation) {
            TargetTransform.position = pos;
            TargetTransform.rotation = rotation;
        }

        private void SmoothToPointAndDirection(Vector3 point, float moveSmooth, Quaternion rotation, float rotSmooth) {
            transform.position = Vector3.SmoothDamp(transform.position, point, ref _smoothVelocity, moveSmooth);
            if (Vector3.Distance(transform.position, point) < 0.01f) {
                transform.position = point;
            }

            Quaternion newRotation;
            newRotation.x = Mathf.SmoothDamp(transform.rotation.x, rotation.x, ref _smoothRotationVelocity.x, rotSmooth);
            newRotation.y = Mathf.SmoothDamp(transform.rotation.y, rotation.y, ref _smoothRotationVelocity.y, rotSmooth);
            newRotation.z = Mathf.SmoothDamp(transform.rotation.z, rotation.z, ref _smoothRotationVelocity.z, rotSmooth);
            newRotation.w = Mathf.SmoothDamp(transform.rotation.w, rotation.w, ref _smoothRotationVelocity.w, rotSmooth);
            transform.rotation = newRotation;
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