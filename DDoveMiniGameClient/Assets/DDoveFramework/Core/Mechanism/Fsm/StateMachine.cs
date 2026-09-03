using System;
using System.Collections.Generic;

namespace DDoveFramework.Core
{
    public class StateMachine
    {
        private readonly Dictionary<string, object> mBlackboard = new Dictionary<string, object>(100);
        private readonly Dictionary<string, IStateNode> mNodes = new Dictionary<string, IStateNode>(100);
        private IStateNode mCurNode;
        private IStateNode mPreNode;

        public object Owner { get; }

        public string CurrentNode => mCurNode != null ? mCurNode.GetType().FullName : string.Empty;

        public string PreviousNode => mPreNode != null ? mPreNode.GetType().FullName : string.Empty;

        public StateMachine(object owner)
        {
            Owner = owner;
        }

        public void Update()
        {
            mCurNode?.OnUpdate();
        }

        public void Run<TNode>() where TNode : IStateNode
        {
            Run(typeof(TNode).FullName);
        }

        public void Run(Type entryNode)
        {
            Run(entryNode.FullName);
        }

        public void Run(string entryNode)
        {
            mCurNode = TryGetNode(entryNode);
            mPreNode = mCurNode;

            if (mCurNode == null)
            {
                DDoveDebug.LogError("Fsm", ("entryNode", entryNode));
                return;
            }

            mCurNode.OnEnter();
        }

        public void AddNode<TNode>() where TNode : IStateNode
        {
            AddNode(Activator.CreateInstance(typeof(TNode)) as IStateNode);
        }

        public void AddNode(IStateNode stateNode)
        {
            if (stateNode == null)
            {
                throw new ArgumentNullException(nameof(stateNode));
            }

            var nodeName = stateNode.GetType().FullName;
            if (mNodes.ContainsKey(nodeName))
            {
                return;
            }

            stateNode.OnCreate(this);
            mNodes.Add(nodeName, stateNode);
        }

        public void ChangeState<TNode>() where TNode : IStateNode
        {
            ChangeState(typeof(TNode).FullName);
        }

        public void ChangeState(Type nodeType)
        {
            ChangeState(nodeType.FullName);
        }

        public void ChangeState(string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }

            var node = TryGetNode(nodeName);
            if (node == null)
            {
                DDoveDebug.LogError("Fsm", ("nodeName", nodeName));
                return;
            }

            mPreNode = mCurNode;
            mCurNode.OnExit();
            mCurNode = node;
            mCurNode.OnEnter();
        }

        public void SetBlackboardValue(string key, object value)
        {
            mBlackboard[key] = value;
        }

        public object GetBlackboardValue(string key)
        {
            return mBlackboard.TryGetValue(key, out var value) ? value : null;
        }

        private IStateNode TryGetNode(string nodeName)
        {
            mNodes.TryGetValue(nodeName, out var result);
            return result;
        }
    }
}
