using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public class NetworkTransformView : ObservableBehaviour
    {
        public bool SyncPosition = true;
        public bool SyncRotation = true;
        public bool SyncScale = false;
        public Vector3 networkedPosition {
            get; set;
        }
        public Vector3 networkedRotation {
            get; set;
        }
        public Vector3 networkedScale {
            get; set;
        }
        protected override string _payloadHeaderKey => "NETTRANSVIEW69";
        void LateUpdate() {
            if(canWrite) OnSerialize(Message.Create());   
        }
        public override void OnSerialize(Message message, bool overwrite = true) {
            if (SyncPosition)
                message.Add(transform.position);
            if(SyncRotation)
                message.Add(transform.rotation);
            if (SyncScale)
                message.Add(transform.localScale);
            base.OnSerialize(message, overwrite);
        }

        public override void OnDeserialize(Message message) {
            base.OnDeserialize(message);
            if (SyncPosition)
                transform.position=message.GetVector3();
            if (SyncRotation)
                transform.rotation=message.GetQuaternion();
            if (SyncScale)
                transform.localScale = message.GetVector3();
        }
    }
}
