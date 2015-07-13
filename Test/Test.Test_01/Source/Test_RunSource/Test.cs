using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Source_01.Source.Test.Test_RunSource
{
    // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception

    /// <summary>
    /// Stores all relevant information required to generate a proxy in order to communicate with a remote object.
    /// Disconnects the remote object (server) when finalized on local host (client).
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CrossAppDomainObjRef : ObjRef
    {
        /// <summary>
        /// Initializes a new instance of the CrossAppDomainObjRef class to
        /// reference a specified CrossAppDomainObject of a specified System.Type.
        /// </summary>
        /// <param name="instance">The object that the new System.Runtime.Remoting.ObjRef instance will reference.</param>
        /// <param name="requestedType"></param>
        public CrossAppDomainObjRef(CrossAppDomainObject instance, Type requestedType)
            : base(instance, requestedType)
        {
            //Proxy created locally (not remoted), the finalizer is meaningless.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes a new instance of the System.Runtime.Remoting.ObjRef class from
        /// serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination of the exception.</param>
        private CrossAppDomainObjRef(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Debug.Assert(context.State == StreamingContextStates.CrossAppDomain);
            Debug.Assert(IsFromThisProcess());
            Debug.Assert(IsFromThisAppDomain() == false);
            //Increment ref counter
            CrossAppDomainObject remoteObject = (CrossAppDomainObject)GetRealObject(new StreamingContext(StreamingContextStates.CrossAppDomain));
            remoteObject.AppDomainConnect();
        }

        /// <summary>
        /// Disconnects the remote object.
        /// </summary>
        ~CrossAppDomainObjRef()
        {
            Debug.Assert(IsFromThisProcess());
            Debug.Assert(IsFromThisAppDomain() == false);
            //Decrement ref counter
            CrossAppDomainObject remoteObject = (CrossAppDomainObject)GetRealObject(new StreamingContext(StreamingContextStates.CrossAppDomain));
            remoteObject.AppDomainDisconnect();
        }

        /// <summary>
        /// Populates a specified System.Runtime.Serialization.SerializationInfo with
        /// the data needed to serialize the current System.Runtime.Remoting.ObjRef instance.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The contextual information about the source or destination of the serialization.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Debug.Assert(context.State == StreamingContextStates.CrossAppDomain);
            base.GetObjectData(info, context);
            info.SetType(typeof(CrossAppDomainObjRef));
        }
    }

    /// <summary>
    /// Enables access to objects across application domain boundaries.
    /// Contrary to MarshalByRefObject, the lifetime is managed by the client.
    /// </summary>
    public abstract class CrossAppDomainObject : MarshalByRefObject
    {
        /// <summary>
        /// Count of remote references to this object.
        /// </summary>
        [NonSerialized]
        private int refCount;

        /// <summary>
        /// Creates an object that contains all the relevant information required to
        /// generate a proxy used to communicate with a remote object.
        /// </summary>
        /// <param name="requestedType">The System.Type of the object that the new System.Runtime.Remoting.ObjRef will reference.</param>
        /// <returns>Information required to generate a proxy.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override ObjRef CreateObjRef(Type requestedType)
        {
            CrossAppDomainObjRef objRef = new CrossAppDomainObjRef(this, requestedType);
            return objRef;
        }

        /// <summary>
        /// Disables LifeTime service : object has an infinite life time until it's Disconnected.
        /// </summary>
        /// <returns>null.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Connect a proxy to the object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AppDomainConnect()
        {
            int value = Interlocked.Increment(ref refCount);
            Debug.Assert(value > 0);
        }

        /// <summary>
        /// Disconnects a proxy from the object.
        /// When all proxy are disconnected, the object is disconnected from RemotingServices.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AppDomainDisconnect()
        {
            Debug.Assert(refCount > 0);
            if (Interlocked.Decrement(ref refCount) == 0)
                RemotingServices.Disconnect(this);
        }
    }
}
